using System;
using System.Data;

namespace Sp.Data
{
    public class SpTransaction : IDbTransaction
    {
        private SpConnection _Connection;

        public SpTransaction(SpConnection connection)
        {
            _Connection = connection;
        }

        #region IDbTransaction Members

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public IDbConnection Connection
        {
            get { return _Connection; }
        }

        public IsolationLevel IsolationLevel
        {
            get { throw new NotImplementedException(); }
        }

        public void Rollback()
        {
            
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
