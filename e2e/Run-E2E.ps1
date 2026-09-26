<#
.SYNOPSIS
End-to-end test of OneNote Duplicates Remover against a temporary local notebook.

.DESCRIPTION
Creates a notebook under -Root with duplicate pages in three folders, then drives the app through
UI Automation: scan, default keep order, Top/Up/Down/Bottom, "Select extra copies", the keep-one-copy
guard, removal with its confirmation and HTML report, and a rescan. Afterwards it checks OneNote
directly and closes and deletes the notebook.

Needs the OneNote desktop app and a built app (Debug x64 by default). The scan reads every open
notebook, but only pages in the temporary notebook are ever removed: before Remove is clicked, every
checked page must live under the notebook folder or the run aborts. The removal report opens in the
default browser. Leave the app window alone while the script runs.

.EXAMPLE
powershell -ExecutionPolicy Bypass -File e2e\Run-E2E.ps1
#>
param(
    [string]$Exe = (Join-Path $PSScriptRoot '..\bin\x64\Debug\net10.0-windows\OneNoteDuplicatesRemover.exe'),
    [string]$Root = (Join-Path ([IO.Path]::GetTempPath()) 'onenote-duplicates-remover-e2e')
)
$Exe = [IO.Path]::GetFullPath($Exe)
New-Item -ItemType Directory -Force $Root | Out-Null
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
$AE = [System.Windows.Automation.AutomationElement]
$Scope = [System.Windows.Automation.TreeScope]
$True_ = [System.Windows.Automation.Condition]::TrueCondition

$results = New-Object System.Collections.Generic.List[string]
$failures = 0
function Check([string]$name, [bool]$ok, [string]$detail = '') {
    $script:results.Add(('{0}  {1}{2}' -f ($(if ($ok) { 'PASS' } else { 'FAIL' })), $name, $(if ($detail) { "  -- $detail" } else { '' })))
    if (-not $ok) { $script:failures++ }
}

$token = [Guid]::NewGuid().ToString('N').Substring(0, 8)
$nbDir = Join-Path $Root "DupTest-$token"
$runDir = Join-Path $Root "e2e-run-$token"
New-Item -ItemType Directory -Force $runDir | Out-Null
$ns = 'http://schemas.microsoft.com/office/onenote/2013/onenote'

# ---------- OneNote setup (COM) ----------
$on = New-Object -ComObject OneNote.Application
function PagesXml() { $x = ''; $on.GetHierarchy('', 4, [ref]$x); [xml]$x }   # 4 = hsPages
function OutsidePageIds() {
    $doc = PagesXml
    $m = New-Object Xml.XmlNamespaceManager($doc.NameTable); $m.AddNamespace('one', $ns)
    @($doc.SelectNodes('//one:Notebook', $m) | Where-Object { $_.path.TrimEnd('\') -ne $nbDir } |
      ForEach-Object { $_.SelectNodes('.//one:Page', $m) } | ForEach-Object { $_.ID } | Sort-Object)
}
function NewPage([string]$sectionId, [string]$title, [string]$body) {
    $pageId = ''; $on.CreateNewPage($sectionId, [ref]$pageId, 0)
    # One:T holds an HTML fragment, so literal markup characters in the title must be escaped.
    $safeTitle = $title.Replace('&', '&amp;').Replace('<', '&lt;').Replace('>', '&gt;')
    $xml = "<?xml version=`"1.0`"?><one:Page xmlns:one=`"$ns`" ID=`"$pageId`"><one:Title><one:OE><one:T><![CDATA[$safeTitle]]></one:T></one:OE></one:Title><one:Outline><one:OEChildren><one:OE><one:T><![CDATA[$body]]></one:T></one:OE></one:OEChildren></one:Outline></one:Page>"
    $on.UpdatePageContent($xml)
    return $pageId
}

$outsideBefore = OutsidePageIds
$nbId = ''; $on.OpenHierarchy($nbDir, '', [ref]$nbId, 1)            # 1 = cftNotebook
$sgArchive = ''; $on.OpenHierarchy('Archive', $nbId, [ref]$sgArchive, 2)   # 2 = cftFolder (section group)
$sgWork = ''; $on.OpenHierarchy('Work', $nbId, [ref]$sgWork, 2)
$secRoot = ''; $on.OpenHierarchy('Inbox.one', $nbId, [ref]$secRoot, 3)     # 3 = cftSection
$secArchive = ''; $on.OpenHierarchy('Old.one', $sgArchive, [ref]$secArchive, 3)
$secWork = ''; $on.OpenHierarchy('Current.one', $sgWork, [ref]$secWork, 3)

$g1 = "DupTest One $token"; $g2 = "DupTest Two $token"; $g4 = "<b>Tom & Jerry</b> 회의록 $token"; $g3 = "DupTest Unique $token"
$p = @{}
$p.g1Root = NewPage $secRoot $g1 "one $token"; $p.g1Archive = NewPage $secArchive $g1 "one $token"; $p.g1Work = NewPage $secWork $g1 "one $token"
$p.g2Work1 = NewPage $secWork $g2 "two $token"; $p.g2Work2 = NewPage $secWork $g2 "two $token"; $p.g2Archive = NewPage $secArchive $g2 "two $token"
$p.g4Root = NewPage $secRoot $g4 "four $token"; $p.g4Work = NewPage $secWork $g4 "four $token"
$p.g3 = NewPage $secRoot $g3 "unique $token"
$locRoot = $nbDir; $locArchive = Join-Path $nbDir 'Archive'; $locWork = Join-Path $nbDir 'Work'
Check 'setup: test notebook created' ($nbId -ne '' -and (Test-Path $nbDir)) $nbDir

# ---------- UI helpers ----------
Add-Type -Namespace E2E -Name Win32 -MemberDefinition '[DllImport("user32.dll")] public static extern bool PostMessage(System.IntPtr hWnd, uint msg, System.IntPtr w, System.IntPtr l);'
# Clicks by posting BM_CLICK. UIA Invoke on a button that opens a modal dialog does not return until the
# dialog closes, and every later UIA call from this process queues behind it.
# BM_CLICK is ignored by a button in a dialog that is not the active window, so answer a message box by
# sending WM_COMMAND with the button ID (IDYES = 6, IDNO = 7) to the dialog itself.
function AnswerDialog($dialog, [int]$buttonId) { [void][E2E.Win32]::PostMessage([IntPtr]$dialog.Current.NativeWindowHandle, 0x0111, [IntPtr]$buttonId, [IntPtr]::Zero); Start-Sleep -Milliseconds 600 }
function PostClick($element) { [void][E2E.Win32]::PostMessage([IntPtr]$element.Current.NativeWindowHandle, 0x00F5, [IntPtr]::Zero, [IntPtr]::Zero); Start-Sleep -Milliseconds 600 }
function Find($root, $prop, $value) { $root.FindFirst($Scope::Descendants, (New-Object System.Windows.Automation.PropertyCondition($prop, $value))) }
function Button($win, $name) { $b = Find $win $AE::NameProperty $name; if (-not $b) { throw "button not found: $name" }; $b }
function Click($win, $name) { (Button $win $name).GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke(); Start-Sleep -Milliseconds 600 }
function Texts($win) { @($win.FindAll($Scope::Descendants, $True_) | ForEach-Object { $_.Current.Name } | Where-Object { $_ }) }
function Status($win) { Texts $win | Where-Object { $_ -match '^(Scan (complete|finished|failed|cancelled)|Ready to scan|Reading|Scanning|Removing|Removal|Unable|Connecting)' } | Select-Object -First 1 }
function Hint($win) { $x = Texts $win; @(($x | Where-Object { $_ -match ' selected · ' }), ($x | Where-Object { $_ -cmatch '^Keep at least one copy in every group' }), ($x | Where-Object { $_ -match '^Select copies to remove' })) | Where-Object { $_ } | Select-Object -First 1 }
function WaitStatus($win, $pattern, $sec = 120) { $sw = [Diagnostics.Stopwatch]::StartNew(); do { Start-Sleep -Milliseconds 700; $s = Status $win } while ($s -notmatch $pattern -and $sw.Elapsed.TotalSeconds -lt $sec); $s }
function ListItems($win) { $l = Find $win $AE::AutomationIdProperty 'listBoxPathPreference'; @($l.FindAll($Scope::Children, $True_) | ForEach-Object { $_ }) }
function Locations($win) { @(ListItems $win | ForEach-Object { $_.Current.Name }) }
function SelectLocation($win, $name) { $i = ListItems $win | Where-Object { $_.Current.Name -eq $name } | Select-Object -First 1; $i.GetCurrentPattern([System.Windows.Automation.SelectionItemPattern]::Pattern).Select(); Start-Sleep -Milliseconds 400 }
function PageNodes($win) {
    $tree = Find $win $AE::AutomationIdProperty 'treeViewHierarchy'
    foreach ($g in $tree.FindAll($Scope::Children, $True_)) {
        foreach ($n in $g.FindAll($Scope::Children, $True_)) {
            $tp = $null; [void]$n.TryGetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern, [ref]$tp)
            [pscustomobject]@{ Group = $g.Current.Name; Text = $n.Current.Name; Checked = ($tp -and $tp.Current.ToggleState -eq 'On'); Node = $n; Toggle = $tp }
        }
    }
}
function Groups($win) { $tree = Find $win $AE::AutomationIdProperty 'treeViewHierarchy'; @($tree.FindAll($Scope::Children, $True_) | ForEach-Object { $_.Current.Name }) }
# A page node reads "<title> — <section path>"; the section path identifies the location.
function In($node, $title, $loc) { $node.Text.StartsWith("$title — ") -and ([IO.Path]::GetDirectoryName($node.Text.Substring($title.Length + 3)) -eq $loc) }
function CheckedOf($nodes, $title, $loc) { @($nodes | Where-Object { In $_ $title $loc } | ForEach-Object { $_.Checked }) }
function SetChecked($node, [bool]$on) { if ($node.Checked -ne $on) { $node.Toggle.Toggle(); Start-Sleep -Milliseconds 150 } }

$proc = $null
try {
    $proc = Start-Process $Exe -WorkingDirectory $runDir -PassThru
    $win = $null
    for ($i = 0; $i -lt 60 -and -not $win; $i++) { Start-Sleep -Milliseconds 500; $win = $AE::RootElement.FindFirst($Scope::Children, (New-Object System.Windows.Automation.PropertyCondition($AE::ProcessIdProperty, $proc.Id))) }
    if (-not $win) { throw 'main window not found' }
    [void](WaitStatus $win '^Ready to scan' 30)

    # 1. Scan
    Click $win 'Scan notebooks'
    $s = WaitStatus $win '^Scan (complete|finished|failed)'
    Check 'scan completes' ($s -eq 'Scan complete') $s
    $groups = Groups $win
    $nodes = @(PageNodes $win)
    Check 'scan: group of 3 copies across root/Archive/Work' (@($groups | Where-Object { $_ -like "*$g1 · 3 copies" }).Count -eq 1)
    Check 'scan: group with two copies in the same location' (@($groups | Where-Object { $_ -like "*$g2 · 3 copies" }).Count -eq 1)
    Check 'scan: special-character title group' (@($groups | Where-Object { $_ -like "*$g4 · 2 copies" }).Count -eq 1) ($groups -join ' | ')
    Check 'scan: unique page is not listed' (@($groups | Where-Object { $_ -like "*$g3*" }).Count -eq 0)

    # 2. Default order: local folders alphabetical (root < Archive < Work), after any cloud folders
    $loc = Locations $win
    $iRoot = [array]::IndexOf($loc, $locRoot); $iArchive = [array]::IndexOf($loc, $locArchive); $iWork = [array]::IndexOf($loc, $locWork)
    Check 'default order: root < Archive < Work' ($iRoot -ge 0 -and $iRoot -lt $iArchive -and $iArchive -lt $iWork) ($loc -join ' | ')
    $cloud = @($loc | ForEach-Object { $_.StartsWith('https:') })
    Check 'default order: cloud folders first' (-not ($cloud -join ',' -match 'False,True')) ($loc -join ' | ')

    # 3. Select extra copies with the default order
    Click $win 'Select extra copies'
    $nodes = @(PageNodes $win)
    Check 'default: g1 keeps root copy' ((CheckedOf $nodes $g1 $locRoot) -join ',' -eq 'False' -and (CheckedOf $nodes $g1 $locArchive) -join ',' -eq 'True' -and (CheckedOf $nodes $g1 $locWork) -join ',' -eq 'True')
    Check 'default: g2 keeps Archive copy' ((CheckedOf $nodes $g2 $locArchive) -join ',' -eq 'False' -and (CheckedOf $nodes $g2 $locWork) -join ',' -eq 'True,True')
    Check 'default: g4 keeps root copy' ((CheckedOf $nodes $g4 $locRoot) -join ',' -eq 'False' -and (CheckedOf $nodes $g4 $locWork) -join ',' -eq 'True')
    $groupCount = $groups.Count; $pageCount = $nodes.Count
    Check 'hint counts after select' ((Hint $win) -eq ('{0:N0} selected · {1:N0} kept in these groups' -f ($pageCount - $groupCount), $groupCount)) (Hint $win)

    # 4. Reorder with every button
    SelectLocation $win $locWork
    Click $win 'Move location to top'
    Check 'Top moves Work to the top' ((Locations $win)[0] -eq $locWork) ((Locations $win) -join ' | ')
    Click $win 'Move location down'
    Check 'Down moves Work to index 1' ((Locations $win)[1] -eq $locWork)
    Click $win 'Move location to bottom'
    $l = Locations $win; Check 'Bottom moves Work to the end' ($l[$l.Count - 1] -eq $locWork)
    Click $win 'Move location up'
    $l = Locations $win; Check 'Up moves Work one step up' ($l[$l.Count - 2] -eq $locWork)
    Check 'moves keep every location exactly once' ((($l | Sort-Object) -join '|') -eq (($loc | Sort-Object) -join '|'))
    SelectLocation $win $locWork
    Click $win 'Move location to top'

    # 5. Select again with Work preferred
    Click $win 'Select extra copies'
    $nodes = @(PageNodes $win)
    Check 'Work first: g1 keeps Work copy' ((CheckedOf $nodes $g1 $locWork) -join ',' -eq 'False' -and (CheckedOf $nodes $g1 $locRoot) -join ',' -eq 'True' -and (CheckedOf $nodes $g1 $locArchive) -join ',' -eq 'True')
    Check 'Work first: g2 keeps one Work copy' (@(CheckedOf $nodes $g2 $locWork | Where-Object { -not $_ }).Count -eq 1 -and (CheckedOf $nodes $g2 $locArchive) -join ',' -eq 'True')
    Check 'Work first: g4 keeps Work copy' ((CheckedOf $nodes $g4 $locWork) -join ',' -eq 'False' -and (CheckedOf $nodes $g4 $locRoot) -join ',' -eq 'True')

    # 6. Guard: checking every copy of a group disables Remove
    $removeButton = Button $win 'Remove selected'
    Check 'Remove enabled while each group keeps a copy' ($removeButton.Current.IsEnabled)
    foreach ($n in @($nodes | Where-Object { In $_ $g1 $locWork })) { SetChecked $n $true }
    Start-Sleep -Milliseconds 400
    Check 'guard: hint warns when a group has no copy left' ((Hint $win) -cmatch '^Keep at least one copy in every group') (Hint $win)
    Check 'guard: Remove disabled' (-not (Button $win 'Remove selected').Current.IsEnabled)
    Click $win 'Clear selection'
    Check 'Clear selection unchecks everything' (@(PageNodes $win | Where-Object { $_.Checked }).Count -eq 0)

    # 7. Remove test pages only: Work-preferred selection, restricted to the test notebook
    Click $win 'Select extra copies'
    foreach ($n in @(PageNodes $win | Where-Object { $_.Checked -and -not $_.Text.Contains($nbDir) })) { SetChecked $n $false }
    $toRemove = @(PageNodes $win | Where-Object { $_.Checked })
    $outside = @($toRemove | Where-Object { -not $_.Text.Contains(" — $nbDir\") })
    if ($outside.Count -gt 0 -or $toRemove.Count -ne 5) { throw "SAFETY ABORT: $($toRemove.Count) checked, $($outside.Count) outside the test notebook" }
    Check 'removal targets: 5 test pages, none outside the test notebook' $true
    PostClick (Button $win 'Remove selected')
    $dialog = $null
    for ($i = 0; $i -lt 20 -and -not $dialog; $i++) { Start-Sleep -Milliseconds 300; $dialog = $AE::RootElement.FindFirst($Scope::Children, (New-Object System.Windows.Automation.AndCondition((New-Object System.Windows.Automation.PropertyCondition($AE::ProcessIdProperty, $proc.Id)), (New-Object System.Windows.Automation.PropertyCondition($AE::ClassNameProperty, '#32770'))))) }
    if (-not $dialog) { $dialog = Find $win $AE::ClassNameProperty '#32770' }
    Check 'confirmation dialog lists the count' ($dialog -and ((Texts $dialog) -join ' ') -match 'The number of the selected pages: 5') ((Texts $dialog) -join ' ')
    if (-not $dialog) { throw 'confirmation dialog not found' }
    AnswerDialog $dialog 6
    $s = WaitStatus $win '^Removal (finished|cancelled)'
    Check 'removal finishes' ($s -eq 'Removal finished · see report') $s

    # 8. Report
    $report = Get-ChildItem $runDir -Filter 'report-*.html' | Select-Object -First 1
    $html = if ($report) { [IO.File]::ReadAllText($report.FullName, [Text.Encoding]::UTF8) } else { '' }
    Check 'report written' ($null -ne $report)
    Check 'report: 5 removed, 0 failed' ($html.Contains('Removed pages (Count: 5)') -and $html.Contains('Pages that could not be removed (Count: 0)'))
    Check 'report: columns' ($html.Contains('<th>Page title</th><th>Location</th><th>Page ID</th>'))
    Check 'report: title encoded, Korean intact' ($html.Contains("&lt;b&gt;Tom &amp; Jerry&lt;/b&gt; 회의록 $token") -and -not $html.Contains('<b>Tom'))
    Check 'report: UTF-8 declared' ($html.Contains('<meta charset="utf-8">'))
    Check 'report: lists the removed IDs' ((@($p.g1Root, $p.g1Archive, $p.g4Root) | Where-Object { $html.Contains($_) }).Count -eq 3)

    # 9. OneNote state
    $doc = PagesXml
    $m = New-Object Xml.XmlNamespaceManager($doc.NameTable); $m.AddNamespace('one', $ns)
    $live = @($doc.SelectNodes('//one:Notebook', $m) | Where-Object { $_.path.TrimEnd('\') -eq $nbDir } | ForEach-Object { $_.SelectNodes('.//one:Page', $m) } |
              Where-Object { $_.ParentNode.isDeletedPages -ne 'true' } | ForEach-Object { $_.ID })
    # Work was moved to the top, so each group keeps its Work copy (g2 keeps one of its two Work copies).
    $g2Kept = @(@($p.g2Work1, $p.g2Work2) | Where-Object { $live -contains $_ })
    $expectedLive = @($p.g1Work, $p.g4Work, $p.g3) + $g2Kept
    Check 'OneNote: exactly the kept pages remain' ($g2Kept.Count -eq 1 -and $live.Count -eq 4 -and (@($expectedLive | Where-Object { $live -contains $_ }).Count -eq 4)) ($live -join ',')
    Check 'OneNote: removed pages are gone' (@(@($p.g1Root, $p.g1Archive, $p.g4Root, $p.g2Archive) | Where-Object { $live -contains $_ }).Count -eq 0)
    $outsideAfter = OutsidePageIds
    Check 'OneNote: pages outside the test notebook unchanged' (($outsideBefore -join ',') -eq ($outsideAfter -join ',')) "$($outsideBefore.Count) before / $($outsideAfter.Count) after"

    # 10. Rescan: the test duplicates are resolved and deleted pages are ignored
    Click $win 'Scan notebooks'
    $s = WaitStatus $win '^Scan (complete|finished|failed)'
    Check 'rescan: no test duplicates remain' (@(Groups $win | Where-Object { $_ -match $token }).Count -eq 0) ((Groups $win) -join ' | ')
}
catch {
    Check "unexpected error: $($_.Exception.Message)" $false
}
finally {
    if ($proc -and -not $proc.HasExited) { [void]$proc.CloseMainWindow(); if (-not $proc.WaitForExit(15000)) { Stop-Process -Id $proc.Id -Force } }
    if ($proc) { Check 'app exits cleanly' ($proc.ExitCode -eq 0) "exit code $($proc.ExitCode)" }
    $log = Get-ChildItem $runDir -Filter 'log-*.log' | Select-Object -First 1
    if ($log) { $bad = @(Select-String -Path $log.FullName -Pattern '\[(WARNING|ERROR|EXCEPTION)\]'); Check 'log has no warnings or errors' ($bad.Count -eq 0) (($bad | Select-Object -First 3 | ForEach-Object { $_.Line }) -join ' / ') }
    if ($nbId) { try { $on.CloseNotebook($nbId) } catch { Check "cleanup: close notebook ($($_.Exception.Message))" $false } }
    [void][System.Runtime.InteropServices.Marshal]::FinalReleaseComObject($on)
    Start-Sleep -Seconds 2
    Remove-Item -Recurse -Force $nbDir -ErrorAction SilentlyContinue
    Check 'cleanup: test notebook closed and deleted' (-not (Test-Path $nbDir))
    $results
    "----"
    "failures: $failures"
    "run folder: $runDir"
}
