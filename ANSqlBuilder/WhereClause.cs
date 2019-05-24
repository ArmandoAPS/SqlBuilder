using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class WhereClause:ISqlExpression
    {
        protected List<ICondition> _Conditions;

        public List<ICondition> SearchConditions
        {
            get
            {
                if (_Conditions == null)
                    _Conditions = new List<ICondition>();
                return _Conditions;
            }
            set
            {
                _Conditions = value;
            }
        }


        public bool IsLiteral
        {
            get { return false; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            if (_Conditions == null) 
                return;
            
            int count = _Conditions.Count;

            if (count == 0)
                return;

            sql.Append(" WHERE ");


            for (int x = 0; x < count; x++)
            {
                ICondition cond = _Conditions[x];

                if (cond.Negation)
                    sql.Append(" NOT ");
                // concat operator
                if (x > 0)
                {
                    if (cond.ConditionType == ConditionType.And)
                        sql.Append(" AND ");
                    else if (cond.ConditionType == ConditionType.Or)
                        sql.Append(" OR ");
                }
         
                cond.GetSql(db_target, ref sql);
    
            }
        }

    }
}
