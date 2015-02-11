Onenote-duplicates-remover
==========================
A tool to de-duplicate OneNote pages.

Requirements
------------
* Microsoft Office OneNote (should be installed)
* .NET framework 4

Description
-----------
* Duplicated OneNote pages are not easily detected by file-level duplication detectors because there are varying attributes such as 'GUID' or 'Last Modified' in the internal file format. This tool ignores those attributes, calculates a hash of each OneNote page based on its content and group them.

Usage
-----
* I have tested many cases such as password protected pages, pages located in a cloud service like SkyDrive (now, OneDrive) and pages shared via networks. For my personal usage, I removed 4,000 duplicated pages out of 16,000 pages. However, I can't guarantee that this tool works for every corner case. I strongly recommend to backup data before you proceed any removal operation.
* Selection preference means that this tool will choose the 'survivor' page based on the priority you set.
* If you check 'Navigate the highlighted page automatically', OneNote will open the highlighted(or selected) page.

Disclaimer
----------
**PLEASE BACKUP YOUR ONENOTE FILES BEFORE PROCEEDING ANY REMOVAL OPERATION **
* What this program does is to **remove** the duplicated OneNote pages by calculating a hash value based on its content. Any hash algorithm can generate the same value for the 'different' inputs; the probability is extremely low but not zero.
* You should be aware of possible software bugs and the limitation of this approach that will cause unexpected data loss.

Download
--------
* I can't provide every updated version for various OneNote versions (32bit, 64bit; 2007, 2010, 2012, 2013).
* Here are the pre-compiled binary versions, but these are out-dated. The most stable way is to build this program by yourself.
* [office 12 (2007)](https://github.com/relue2718/onenote-duplicates-remover/blob/master/publish/12.zip)
* [office 14 (2010)](https://github.com/relue2718/onenote-duplicates-remover/blob/master/publish/14.zip)
* [office 15 (2012)](https://github.com/relue2718/onenote-duplicates-remover/blob/master/publish/15.zip)
