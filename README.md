# SharePoint Synchronization Adapters


This implementation provides SyncAdapters which can synchronize SharePoint Lists to local SQLCe databases.
The implementation of conflict resolution, initiation of synchronization et al, depends on Microsoft Sync Framework.

## Features

* Map SharePoint fields to database fields.
* Convert field values to database fields.
* Bidirectional mode. Can download changes from SharePoint and vise versa.
* Can sync multiple lists/tables.
* Can be easily setup using the same paradigm from synchronizating Client-Server SQL.

