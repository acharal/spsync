# SharePoint Synchronization Adapters


This implementation provides SyncAdapters which can synchronize SharePoint Lists to local SQLCe databases.
The implementation of conflict resolution, initiation of synchronization et al, depends on Microsoft Sync Framework.

## Features

* Map SharePoint fields to database fields.
* Convert field values to database fields.
* Bidirectional mode. Can download changes from SharePoint and vise versa.
* Can sync multiple lists/tables.
* Can be easily setup using the same paradigm from synchronizating Client-Server SQL.
* Supports compilation for Windows Phone

## Compilation

In order to compile the source you must open the Visual Studio 2010 project and compile.
You must have installed the
* Microsoft Sync Framework DLLs.
* .NET Framework 3.5 or above.

If you want to compile the code for Windows Phone you must have installed the appropriate Mobile SDK
along with the .NET Compact Framework and Sync Framework for Windows Phone.

