using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;


namespace ANSqlBuilder
{
    public class SqlString: ISqlExpression
    {
        public static SqlString Empty = new SqlString("");

        string _Value;

        public SqlString(string value)
        {
            _Value = value;
        }

         public string Value
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
            if (_Value == null)
                sql.Append("NULL");
            else
            {
                sql.Append("'");
                sql.Append(_Value.Replace("'", "''"));
                sql.Append("'");
            }
        }
    }

    public class SqlStringArray : ISqlExpression
    {
        string[] _Value;

        public SqlStringArray(string[] value)
        {
            _Value = value;
        }

        public string[] Value
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
            for (var x = 0; x < _Value.Length; x++ )
            {
                if (x > 0)
                    sql.Append(",");            
                sql.Append("'");
                sql.Append(_Value[x].Replace("'", "''"));
                sql.Append("'");
            }

        }
    }
}
