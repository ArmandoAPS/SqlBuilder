using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class InsertSelectStatament: NonQueryStatement
    {
        protected string _TableName;
        protected List<string> _Columns;
        protected SelectStatement _Select;

        #region Constructors
         

        #endregion

        #region Properties

        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                _TableName = value;
            }
        }

        public List<string> Columns
        {
            get
            {
                if(_Columns == null)
                    _Columns = new List<string>();
                return _Columns;
            }
            set
            {
                _Columns = value;
            }
        }

        public SelectStatement Select
        {
            get
            {
                return _Select;
            }
            set
            {
                _Select = value;
            }
        }

        public override void GetSql(DbTarget db_target,ref StringBuilder sql)
        {
            sql.Append(" INSERT INTO ");

            sql.Append(Utils.FormatName(TableName,db_target));

            sql.Append(" ");

            if (_Columns != null && _Columns.Count > 0)
            {
                sql.Append("(");
                int colCount = _Columns.Count;
                for (int x = 0; x < colCount; x++)
                {
                    sql.Append(Utils.FormatName(_Columns[x],db_target));
                    if (x + 1 < colCount)
                        sql.Append(", ");
                }
                sql.Append(") ");
            }
            
            Select.GetSql(db_target,ref sql);
        }
        #endregion Properties

        #region Methods

        #endregion
    }
}
