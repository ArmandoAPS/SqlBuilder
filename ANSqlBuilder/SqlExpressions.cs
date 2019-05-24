using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class SqlExpressions: List<ISqlExpression>, ISqlExpression
    {
        public bool IsLiteral
        {
            get { return false; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            int count = this.Count;
            if (count == 0)
                return;

            for (int x = 0; x < count; x++)
            {
                this[x].GetSql(db_target,ref sql);
                if (x + 1 < count)
                    sql.Append(",");
            }
        }
    }
}
