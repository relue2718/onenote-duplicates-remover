using System.Drawing;
using System.Windows.Forms;

namespace OneNoteDuplicatesRemover
{
    // Shared by the main window and the About dialog. Honor Windows high contrast.
    internal static class AppTheme
    {
        internal static Color Background => SystemInformation.HighContrast ? SystemColors.Control : Color.FromArgb(245, 245, 250);
        internal static Color Surface => SystemInformation.HighContrast ? SystemColors.Window : Color.White;
        internal static Color Text => SystemInformation.HighContrast ? SystemColors.WindowText : Color.FromArgb(35, 32, 48);
        internal static Color Muted => SystemInformation.HighContrast ? SystemColors.WindowText : Color.FromArgb(102, 98, 117);
        internal static Color Accent => SystemInformation.HighContrast ? SystemColors.Highlight : Color.FromArgb(112, 66, 181);
        internal static Color Border => SystemInformation.HighContrast ? SystemColors.WindowText : Color.FromArgb(224, 221, 234);
        internal static Color Danger => SystemInformation.HighContrast ? SystemColors.WindowText : Color.FromArgb(174, 42, 55);
        internal static readonly Font BodyFont = new Font("Segoe UI", 10F);
        internal static readonly Font HeadingFont = new Font("Segoe UI", 24F, FontStyle.Bold);
        internal static readonly Font SectionFont = new Font("Segoe UI", 12F, FontStyle.Bold);
        internal static readonly Font NumberFont = new Font("Segoe UI", 24F, FontStyle.Bold);

        internal static Label Label(string text, Font font = null, Color? color = null)
        {
            return new Label
            {
                Text = text, AutoSize = true, Font = font ?? BodyFont,
                ForeColor = color ?? Text, BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 8), UseMnemonic = false
            };
        }

        internal static Button Button(string text, bool primary = false, bool destructive = false)
        {
            var button = new Button
            {
                Text = text, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new Size(0, 38), Padding = new Padding(12, 6, 12, 6),
                Margin = new Padding(0, 0, 8, 0), FlatStyle = FlatStyle.Flat, Font = BodyFont,
                BackColor = primary ? Accent : Surface,
                ForeColor = primary ? SystemColors.HighlightText : destructive ? Danger : Text,
                UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderSize = primary ? 0 : 1;
            button.FlatAppearance.BorderColor = destructive ? Danger : Border;
            if (!SystemInformation.HighContrast)
            {
                button.FlatAppearance.MouseOverBackColor = primary ? Color.FromArgb(91, 48, 155) : Background;
                button.FlatAppearance.MouseDownBackColor = primary ? Color.FromArgb(76, 37, 134) : Color.FromArgb(235, 231, 244);
            }
            if (primary)
                button.EnabledChanged += (sender, e) => button.BackColor = button.Enabled ? Accent : Background;
            return button;
        }

        internal static TableLayoutPanel Stack()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, Margin = Padding.Empty, Padding = Padding.Empty
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            return panel;
        }

        internal static FlowLayoutPanel Actions()
        {
            return new FlowLayoutPanel
            {
                AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill, Margin = Padding.Empty, Padding = Padding.Empty, WrapContents = true
            };
        }

        internal static Panel Card()
        {
            return new Panel
            {
                Dock = DockStyle.Fill, BackColor = Surface, Padding = new Padding(20), Margin = Padding.Empty
            };
        }
    }
}
