using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Sp.Data
{
    public class SpConnection : IDbConnection
    {
        private ConnectionState _state;
        private string _connectionString;
        private int _connectionTimeout;

        #region IDbConnection Members

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return new SpTransaction(this);
        }

        public IDbTransaction BeginTransaction()
        {
            return new SpTransaction(this);
        }

        public void ChangeDatabase(string databaseName)
        {
            
        }

        public void Close()
        {
            if (_state == ConnectionState.Closed)
                throw new InvalidOperationException("Connection is already closed");

            _state = ConnectionState.Closed;
        }

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        public int ConnectionTimeout
        {
            get { return _connectionTimeout; }
        }

        public IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public string Database
        {
            get { throw new NotImplementedException(); }
        }

        public void Open()
        {
            _state = ConnectionState.Open;
        }

        public ConnectionState State
        {
            get { return _state;  }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
