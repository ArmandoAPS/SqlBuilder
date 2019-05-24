using System;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class SqlNull:ISqlExpression
    {
        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            sql.Append("NULL");
        }
    }
}
