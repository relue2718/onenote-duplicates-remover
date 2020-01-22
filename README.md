OneNote Duplicates Remover
==========================
Remove OneNote page duplicates

Requirements
------------
* Microsoft Office OneNote
* .NET framework 4.5

Why do I need this tool?
------------------------
* Some OneNote page duplicates cannot be detected by file-level duplicate removers.

How does this tool work?
------------------------
* It calculates a hash value of the page content except for some irrelevant attributes such as 'objectId' (UUID) and 'lastModifiedTime'.

Screenshot
----------
![screenshot](https://raw.githubusercontent.com/relue2718/onenote-duplicates-remover/master/screenshot/1.png)

Disclaimer
----------
* It is strongly recommended to back up the files before you proceed any removal operation.
* There is a very rare chance to get the same hash value (SHA256) for different contents. The hash collision may cause unexpected data loss.

Potential Issues
----------------
![screenshot](https://raw.githubusercontent.com/relue2718/onenote-duplicates-remover/master/screenshot/2.png)

* You should not run this program on multiple computers. Let's say you have two computers A and B running this tool. You have deleted the page A' on the computer A and have deleted the page A on the computer B. The sync process will end up deleting all pages (data loss).

Downloads
---------
* [setup.exe](https://github.com/relue2718/onenote-duplicates-remover/releases/download/v1.0.1.11/setup.exe)
