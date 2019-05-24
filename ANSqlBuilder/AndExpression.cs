using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class AndExpression: ISqlExpression
    {
        protected ISqlExpression _expression1;
        protected ISqlExpression _expression2;

        public AndExpression(ISqlExpression expression1, ISqlExpression expression2)
        {
            _expression1 = expression1;
            _expression2 = expression2;
        }

        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            if (!_expression1.IsLiteral)
                sql.Append("(");

            _expression1.GetSql(db_target, ref sql);

            if (!_expression1.IsLiteral)
                sql.Append(")");

            sql.Append(" AND ");

            if (!_expression2.IsLiteral)
                sql.Append("(");

            _expression2.GetSql(db_target, ref sql);

            if (!_expression2.IsLiteral)
                sql.Append(")");
        }
    }
}
