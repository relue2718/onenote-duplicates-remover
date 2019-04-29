OneNote Duplicates Remover
==========================
A simple tool that removes duplicated pages for Microsoft OneNote

Requirements
------------
* Microsoft Office OneNote
* .NET framework 4.5

Why do I need this tool?
-----------
* Some duplicated OneNote pages cannot be detected by file-level duplicate removers because internal attributes such as 'objectId' and 'lastModifiedTime' are different.
* This tool calculates a hash value for each OneNote page by ignoring those attributes.

Screenshot
----------
![screenshot](https://raw.githubusercontent.com/relue2718/onenote-duplicates-remover/master/screenshot/1.png)

Disclaimer
----------
* **PLEASE BACKUP YOUR ONENOTE FILES BEFORE YOU PROCEED ANY REMOVAL OPERATION**
* I cannot guarantee that this tool does not have a bug for every corner case.
* I strongly recommend to backup notebook files before you proceed any removal operation.
* This tool does not use an exact match due to the attribute issue and uses a hash function (SHA256) for grouping duplicates. There is a very rare chance to have the same hash value for different contents. The hash collision can cause unexpected data loss.

Known Issues
------------
![screenshot](https://raw.githubusercontent.com/relue2718/onenote-duplicates-remover/master/screenshot/2.png)

* You should not run this program in the multiple computers. Let's say you have two computers, A and B. If you run this program in the computer A and delete the duplicated page A', and sync notes with the computer B. The sync process will delete the page A' in the computer B as well. However, if you run this program in the computer B and somehow delete the page A, both pages A and A' will be deleted (data loss).
* Please run this tool in only one computer and make sure the sync process is finished.

