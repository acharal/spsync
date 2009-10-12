using System;
using System.Collections.Generic;

namespace SpServerSync.Data
{
    public class SyncDataColumnCollection : IEnumerable<string>
    {
        private IList<string> _projectionColumns;

        public SyncDataColumnCollection()
        {
            _projectionColumns = new List<string>();
        }

        public void Add(string columnName)
        {
            if (!_projectionColumns.Contains(columnName))
                _projectionColumns.Add(columnName);
        }

        public void AddRange(string[] columnNames)
        {
            foreach (string column in columnNames)
                this.Add(column);
        }

        public void Clear()
        {
            _projectionColumns.Clear();
        }

        public void RemoveAt(int index)
        {
            _projectionColumns.RemoveAt(index);
        }

        public void Remove(string columnName)
        {
            _projectionColumns.Remove(columnName);
        }

        public int Count
        { 
            get { return _projectionColumns.Count; } 
        }

        public string this[int index]
        {
            get { return _projectionColumns[index]; }
        }

        public bool Contains(string columnName)
        {
            return _projectionColumns.Contains(columnName);
        }

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return _projectionColumns.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _projectionColumns.GetEnumerator();
        }

        #endregion
    }
}
