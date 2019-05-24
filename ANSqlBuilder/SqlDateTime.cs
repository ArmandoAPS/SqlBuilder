using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class SqlDateTime: ISqlExpression
    {
        DateTime _Value;
        public SqlDateTime(DateTime value)
        {
            _Value = value;
        }

        public DateTime Value
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
            sql.Append("'");
            sql.Append(_Value.ToString("yyyy-MM-dd HH:mm:ss"));
            sql.Append("'");
        }
    }
}
