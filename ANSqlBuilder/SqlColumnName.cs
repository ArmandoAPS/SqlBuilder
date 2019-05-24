using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;


namespace ANSqlBuilder
{
    public class SqlColumnName:ISqlExpression
    {
        public SqlName Name { get; set; }
        protected SqlName TableAlias { get; set; }

        public SqlColumnName(string name)
        {
            Name = new SqlName(name);
        }

        public SqlColumnName(string table_alias, string name)
        {
            TableAlias = new SqlName(table_alias);
            Name = new SqlName(name);
        }

        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            if (TableAlias != null)
            {
                TableAlias.GetSql(db_target, ref sql);
                sql.Append(".");
            }
            Name.GetSql(db_target, ref sql);
        }

    }
}
