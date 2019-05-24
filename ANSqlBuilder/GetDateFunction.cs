using System;
using System.Text;
using ANCommon.Sql;


namespace ANSqlBuilder
{
    public class GetDateFunction:ISqlExpression
    {

        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            switch (db_target)
            {
                case DbTarget.SqlServer:
                    sql.Append("GETDATE()");
                    break;

                case DbTarget.MySql:
                    sql.Append("DATE");
                    break;

                case DbTarget.SqlLite:
                    sql.Append("CURRENT_TIMESTAMP");
                    break;
            }
            
        }
    }
}
