using System;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class SubstringFunction : ISqlExpression
    {

        protected ISqlExpression _Expression;
        protected int _Position;
        protected int _Length;

        public SubstringFunction(string expression, int position, int length)
        {
            _Expression = new SqlLiteral(expression);
            _Position = position;
            _Length = length;

        }

        public SubstringFunction(ISqlExpression expression, int position, int length)
        {
            _Expression = expression;
            _Position = position;
            _Length = length;
        }

        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            switch (db_target)
            {
                case DbTarget.SqlServer:
                    sql.Append("SUBSTRING(");
                    break;
                case DbTarget.MySql:
                    sql.Append("SUBSTRING(");
                    break;
                case DbTarget.SqlLite:
                    sql.Append("SUBSTR(");
                    break;
            }
            if (!_Expression.IsLiteral)
                sql.Append("(");
            _Expression.GetSql(db_target, ref sql);
            if (!_Expression.IsLiteral)
                sql.Append(")");
            sql.Append(_Position.ToString());
            sql.Append(",");
            sql.Append(_Length.ToString());
            sql.Append(")");

        }
    }
}
