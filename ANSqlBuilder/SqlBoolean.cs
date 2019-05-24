using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class SqlBoolean:ISqlExpression
    {
        public static SqlBoolean TRUE
        {
            get { return new SqlBoolean(true); } 
        }

        public static SqlBoolean FALSE
        {
            get { return new SqlBoolean(false);  }
        }

        Boolean _Value;
        public SqlBoolean(Boolean value)
        {
            _Value = value;
        }

        public Boolean Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target,ref StringBuilder sql)
        {
            sql.Append(_Value ? "1" : "0");
        }
    }
}
