using System;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class ExistsCondition:ICondition
    {
        protected ISqlExpression _expression;

        public ExistsCondition(ISqlExpression expression)
        {
            _expression = expression;
        }

        public bool IsLiteral
        {
            get { return true; }
        }

        public bool Negation { get; set; }

        public ConditionType ConditionType { get; set; }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            if (Negation)
                sql.Append("NOT ");
            sql.Append("EXISTS(");
            _expression.GetSql(db_target, ref sql);
            sql.Append(")");

        }
    }
}
