using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANCommon.Sql
{
    public interface ISqlExpressionBase
    {
        void GetSql(DbTarget db_target, ref StringBuilder sql);
    }

    public interface ISqlExpression : ISqlExpressionBase
    {
        bool IsLiteral { get; }
    }

    public interface ICondition : ISqlExpressionBase
    {
        ConditionType ConditionType { get; set; }
        bool Negation { get; set; }
    }

    public interface IOrderByExpression : ISqlExpressionBase
    {

    }
}
