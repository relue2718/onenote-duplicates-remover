OneNote Duplicates Remover
==========================

Overview
---
OneNote Duplicates Remover is a Windows application designed to identify and remove duplicate OneNote pages, which are often missed by traditional file-level duplicate removers.

Requirements
---
* Microsoft Office OneNote
* .NET framework 4.5

Why Use This Tool?
---
* Traditional duplicate removers may not detect duplicate OneNote pages because they focus on file-level hashes. This tool addresses that limitation by comparing the content of OneNote pages to find duplicates.

How It Works
---
* It calculates a hash value for the content of each OneNote page, ignoring irrelevant attributes like 'objectId' (UUID) and 'lastModifiedTime'. This ensures that duplicates are detected based on the actual content, not just metadata.

Screenshot
---
![screenshot](https://raw.githubusercontent.com/relue2718/onenote-duplicates-remover/master/screenshot/1.png)

Disclaimer
---
* It is strongly recommended to back up your files before proceeding with any removal operations.
* There is a very rare chance of generating the same hash value (SHA256) for different contents. This hash collision may cause unexpected data loss.

Potential Issues
---
![screenshot](https://raw.githubusercontent.com/relue2718/onenote-duplicates-remover/master/screenshot/2.png)

* Do not run this program on multiple computers simultaneously. For example, if you have two computers, A and B, both running this tool, and you delete page A' on computer A and page A on computer B, the sync process will result in deleting all pages, leading to data loss.

Downloads
---
* [setup.exe](https://github.com/relue2718/onenote-duplicates-remover/releases/download/v1.0.1.11/setup.exe)
