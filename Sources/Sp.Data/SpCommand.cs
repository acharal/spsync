using System;
using System.Data;

namespace Sp.Data
{
    /// <summary>
    /// Abstracts a Sharepoint command of retrieving data
    /// </summary>
    public class SpCommand : IDbCommand
    {
        #region IDbCommand Members

        /// <summary>
        /// Cancels the execution of the command
        /// </summary>
        public void Cancel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public string CommandText
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the timeout of the execution of the command
        /// </summary>
        public int CommandTimeout
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandType CommandType
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbConnection Connection
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter CreateParameter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader(CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the collection of the parameters for the command
        /// </summary>
        public IDataParameterCollection Parameters
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Prepares the command in order to be executed faster
        /// </summary>
        public void Prepare()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction Transaction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
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
