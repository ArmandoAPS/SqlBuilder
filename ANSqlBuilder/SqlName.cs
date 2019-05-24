using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class SqlName: ISqlExpression
    {
        protected string _Text;

        public SqlName(string text)
        {
            _Text = text;
        }

        public string Text
        {
            get { return _Text; }
        }
        
        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            if (db_target == DbTarget.SqlServer)
                sql.Append("[");
            else if (db_target == DbTarget.MySql)
                sql.Append("`");
            else if (db_target == DbTarget.SqlLite)
                sql.Append("\"");
            sql.Append(_Text);

            if (db_target == DbTarget.SqlServer)
                sql.Append("]");
            else if (db_target == DbTarget.MySql)
                sql.Append("`");
            else if (db_target == DbTarget.SqlLite)
                sql.Append("\"");

        }

    }
}
