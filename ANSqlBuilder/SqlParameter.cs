using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class SqlParameter : ISqlExpression
    {
        protected string _Name;
        protected object _Value;
        protected DbColumnType _DataType;

        public SqlParameter(string name)
        {
            _Name = name;
            _DataType = DbColumnType.String;
        }

        public SqlParameter(string name, DbColumnType data_type)
        {
            _Name = name;
            _DataType = data_type;
        }

        public SqlParameter(string name, DbColumnType data_type, object value)
        {
            _Name = name;
            _DataType = data_type;
            _Value = value;
        }

        public SqlParameter(string name, Type system_type)
        {
            _Name = name;
            _DataType = Utils.GetDbColumnType(system_type);
        }

        public SqlParameter(string name, Type system_type, object value)
        {
            _Name = name;
            _DataType = Utils.GetDbColumnType(system_type);
            _Value = value;
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public DbColumnType DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
        }

        public Object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }


        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            if (_Value == null || _Value == Convert.DBNull)
                sql.Append("NULL");
            else
            {
                switch (_DataType)
                {
                    case DbColumnType.String:
                        sql.Append("'");
                        sql.Append(_Value.ToString().Replace("'", "''"));
                        sql.Append("'");
                        break;

                    case DbColumnType.Number:
                        sql.Append(_Value.ToString());
                        break;

                    case DbColumnType.Date:
                        sql.Append("'");
                        sql.Append(((DateTime)_Value).ToString("yyyy-MM-dd HH:mm:ss"));
                        sql.Append("'");
                        break;

                    case DbColumnType.Boolean:
                        sql.Append(((Boolean)_Value) ? "1" : "0");
                        break;

                    case DbColumnType.Binary:
                        sql.Append("'");

                        sql.Append("'");
                        break;

                }
            }
        }
    }

    /*public class SqlParameterCollection : System.Collections.CollectionBase
    {
        
    }*/

}
