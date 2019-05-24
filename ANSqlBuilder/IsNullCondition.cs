using System;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class IsNullCondition: ICondition
    {
        private bool _Negation;
        private ConditionType _ConditionType;
        private ISqlExpression _expression;

        public IsNullCondition(ISqlExpression expression)
        {
            _expression = expression;
        }

        public bool IsLiteral
        {
            get { return true; }
        }

        public bool Negation
        {
            get { return _Negation; }
            set { _Negation = value; }
        }

        public ConditionType ConditionType
        {
            get { return _ConditionType; }
            set { _ConditionType = value; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            _expression.GetSql(db_target, ref sql);
            sql.Append(" IS ");
            if (Negation)
                sql.Append("NOT ");
            sql.Append("NULL");

        }
    }
}
