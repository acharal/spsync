using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization;

namespace SpSync.Data.Server
{

    public class SpSyncProvider : KnowledgeSyncProvider
    {

        public override void BeginSession(SyncProviderPosition position, SyncSessionContext syncSessionContext)
        {
            throw new NotImplementedException();
        }

        public override void EndSession(SyncSessionContext syncSessionContext)
        {
            throw new NotImplementedException();
        }

        public override ChangeBatch GetChangeBatch(uint batchSize, SyncKnowledge destinationKnowledge, out object changeDataRetriever)
        {
            throw new NotImplementedException();
        }

        public override FullEnumerationChangeBatch GetFullEnumerationChangeBatch(uint batchSize, SyncId lowerEnumerationBound, SyncKnowledge knowledgeForDataRetrieval, out object changeDataRetriever)
        {
            throw new NotImplementedException();
        }

        public override void GetSyncBatchParameters(out uint batchSize, out SyncKnowledge knowledge)
        {
            throw new NotImplementedException();
        }

        public override SyncIdFormatGroup IdFormats
        {
            get { throw new NotImplementedException(); }
        }

        public override void ProcessChangeBatch(ConflictResolutionPolicy resolutionPolicy, ChangeBatch sourceChanges, object changeDataRetriever, SyncCallbacks syncCallbacks, SyncSessionStatistics sessionStatistics)
        {
            throw new NotImplementedException();
            //DbSyncContext dbSyncContext = changeDataRetriever as DbSyncContext;
            
        }

        public override void ProcessFullEnumerationChangeBatch(ConflictResolutionPolicy resolutionPolicy, FullEnumerationChangeBatch sourceChanges, object changeDataRetriever, SyncCallbacks syncCallbacks, SyncSessionStatistics sessionStatistics)
        {
            throw new NotImplementedException();
        }

    }
}
