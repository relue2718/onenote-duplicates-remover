OneNote Duplicates Remover
==========================
A simple tool to remove the duplicated Microsoft OneNote pages.

Requirements
------------
* Microsoft Office OneNote
* .NET framework 4 

Why do I need this tool?
-----------
* Duplicated OneNote pages are not easily detected by file-level duplication detectors because there are varying attributes such as 'objectId' or 'lastModifiedTime' in the OneNote file format. This tool ignores those attributes and calculates a hash value of each OneNote page based on its contents.

Usage
-----
* Click '*Scan Duplicates*' and click '*Select all except one*' button once the scanning process is done.
* You may change the selection preference by changing the order of paths. (See the '*path preference*' area)
* If you check '*Navigate the highlighted page*', OneNote will open the selected page automatically. It will help you review the selected page.

Screenshot
----------
![screenshot](https://raw.githubusercontent.com/relue2718/onenote-duplicates-remover/master/screenshot/1.png)

Disclaimer
----------
* **PLEASE BACKUP YOUR ONENOTE FILES BEFORE YOU PROCEED ANY REMOVAL OPERATION**
* I've tested many cases such as password protected pages, pages located in OneDrive or shared via networks. For my personal usage, I could remove 4,000 duplicated pages out of 16,000 pages. However, I can't guarantee that this tool works for every corner case. I strongly recommend to backup data before you proceed any removal operation.
* The algorithm to detect duplicates is based on the property of uniqueness of hashing algorithm (MD5). However, there is a chance to have the same hash value for different contents. The probability is extremely low, but not zero. You should be aware of the limitation of this approach that can cause unexpected data loss.

Issues
--------
![screenshot](https://raw.githubusercontent.com/relue2718/onenote-duplicates-remover/master/screenshot/2.png)

* You should not run this program in the multiple computers. Let's say that you have two computers, A and B. If you run this program in the computer A and delete the duplicated page A', just sync notes with the computer B. The sync process will delete the page A' in the computer B as well. However, if you run this program in the computer B and delete the page A, it may cause an unwanted result because the sync process will consider that you wanted to delete the page A and the page A' and apply this changes (removal operations on the both pages) in the both computers. (DATA LOSS)
* Again, run this program **only once** and rely on the OneNote's synchronization!

Download
--------
* [OneNoteDuplicatesRemover.exe (Executable File; 32bit; Onenote 2013)](https://github.com/relue2718/onenote-duplicates-remover/raw/master/executable/OneNoteDuplicatesRemover.exe)
* If you can't run the executable file, you may build the program by yourself. Please understand me that I can't provide every version that works for various environments.

Need help!
----------
* Please let me know if there is a way to do lazy binding for a COM type library, especially for the OneNote type library. I've tried several methods, but those methods won't work for the OneNote type library.
