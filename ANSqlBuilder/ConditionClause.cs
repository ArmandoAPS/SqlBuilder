using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class ConditionClause : ISqlExpression
    {
        protected string _Name;
        protected List<ICondition> _Conditions;

        #region Constructor
        public ConditionClause(string name)
        {
            _Name = name;
            _Conditions = new List<ICondition>();
        }
        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return (_Conditions == null ? 0 : _Conditions.Count);
            }
        }

        public bool IsLiteral
        {
            get { return false; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            int count = this.Count;
            if (count == 0)
                return;
           
            sql.Append(" ").Append(_Name).Append(" ");
            for (int x = 0; x < count; x++)
            {
                ICondition cond = _Conditions[x];
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

        #endregion

        #region Method

        public ConditionClause And(ICondition search_condition)
        {
            if (_Conditions == null)
                _Conditions = new List<ICondition>();
            search_condition.ConditionType = ConditionType.And;
            _Conditions.Add(search_condition);
            return this;
        }

        public ConditionClause Or(ICondition search_condition)
        {
            if (_Conditions == null)
                _Conditions = new List<ICondition>();

            search_condition.ConditionType = ConditionType.Or;
            _Conditions.Add(search_condition);
            return this;
        }

        #endregion
    }
}
