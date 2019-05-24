using System;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class IsNullFunction : ISqlExpression
    {
        protected ISqlExpression _expression;
        protected ISqlExpression _on_null;


        public IsNullFunction(string expression, string on_null)
        {
            _expression = new SqlLiteral(expression);
            _on_null = new SqlString(on_null);
        }

        public IsNullFunction(string expression, ISqlExpression on_null)
        {
            _expression = new SqlLiteral(expression);
            _on_null = on_null;
        }

        public IsNullFunction(ISqlExpression expression, string on_null)
        {
            _expression = expression;
            _on_null = new SqlLiteral(on_null);
        }

        public IsNullFunction(ISqlExpression expression, ISqlExpression on_null)
        {
            _expression = expression;
            _on_null = on_null;
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
                    sql.Append("ISNULL(");
                    break;
                case DbTarget.MySql:
                    sql.Append("IFNULL(");
                    break;
                case DbTarget.SqlLite:
                    sql.Append("IFNULL(");
                    break;
            }
            if(!_expression.IsLiteral)
                sql.Append("(");
            _expression.GetSql(db_target, ref sql);
            if (!_expression.IsLiteral)
                sql.Append(")");
            sql.Append(",");
            if (!_on_null.IsLiteral)
                sql.Append("(");
            _on_null.GetSql(db_target, ref sql);
            if (!_on_null.IsLiteral)
                sql.Append(")");
            sql.Append(")");
        }
    }
}
