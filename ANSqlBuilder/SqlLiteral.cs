using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class SqlLiteral:ISqlExpression
    {
        public static SqlLiteral Null = new SqlLiteral("null");
        public static SqlLiteral CountAll = new SqlLiteral("COUNT(*)");

       
        protected string _Text;

        public SqlLiteral(string text)
        {
            _Text = text;
        }

        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }


        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            sql.Append(Text);
        }
    }
}
