using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;


namespace ANSqlBuilder
{
    public class InsertStatement : NonQueryStatement
    {
        protected string _TableName;
        protected List<SqlNameValuePair> _Columns;
        protected bool _HasIdentity;
        protected string _IdentityColumnName;

        #region Constructors
        public InsertStatement(string table_name)
        {
            TableName = table_name;
        }

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

        public string IdentityColumnName
        {
            get { return _IdentityColumnName;  }
            set { _IdentityColumnName = value; }
        }

        public List<SqlNameValuePair> Columns
        {
            get
            {
                if (_Columns == null)
                    _Columns = new List<SqlNameValuePair>();
                return _Columns;
            }
            set
            {
                _Columns = value;
            }
        }

        public override void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            sql.Append("INSERT INTO ");

            sql.Append(Utils.FormatName(TableName, db_target));

            sql.Append(" ");

            if (_Columns != null && _Columns.Count > 0)
            {
                sql.Append("(");
                int colCount = _Columns.Count;
                for (int x = 0; x < colCount; x++)
                {
                    sql.Append(Utils.FormatName(_Columns[x].Name,db_target));
                    if (x + 1 < colCount)
                        sql.Append(",");
                }
                sql.Append(") VALUES(");

                for (int x = 0; x < colCount; x++)
                {
                    _Columns[x].Value.GetSql(db_target,ref sql);
                    if (x + 1 < colCount)
                        sql.Append(",");
                }
                sql.Append(")");
            }
           
        }
        #endregion Properties

        #region Methods

        public InsertStatement Column(string name, string value)
        {
            Columns.Add(new SqlNameValuePair(name, value));
            return this;
        }

        public InsertStatement Column(string name, ISqlExpression value)
        {
            Columns.Add(new SqlNameValuePair(name, value));
            return this;
        }

        public InsertStatement Column(string name, DateTime value)
        {
            Columns.Add(new SqlNameValuePair(name, value));
            return this;
        }

        public int Execute(out object last_inserted_id)
        {
            last_inserted_id = 0;
            using (DbHelper helper = new DbHelper(ConnectionStringName))
            {
                var sql = new StringBuilder();
                this.GetSql(DbTarget,ref sql);
                switch (this.DbTarget)
                {
                    case DbTarget.SqlServer:
                        sql.Append(";SELECT @@IDENTITY");
                        last_inserted_id = helper.ExecuteScalar(sql.ToString());
                        break;
                    case DbTarget.MySql:
                        last_inserted_id = helper.ExecuteNonQueryAndScalar(sql.ToString(), "SELECT LAST_INSERT_ID()");
                        break;
                    case DbTarget.SqlLite:
                        sql.Append(";SELECT LAST_INSERT_ROWID()");
                        last_inserted_id = helper.ExecuteScalar(sql.ToString());
                        break;
                }
                
            }
          
            return Int32.Parse(last_inserted_id.ToString());
           
        }
        #endregion 
    }
}
