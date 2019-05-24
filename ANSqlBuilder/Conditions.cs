using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class Conditions : ICondition
    {
        protected ConditionType _ConditionType;
        protected bool _Negation;
        protected List<ICondition> _ConditionList ;

        #region Constructor
        public Conditions()
        {
            if (_ConditionList == null)
                _ConditionList = new List<ICondition>();
        }
        public Conditions(ICondition search_condition)
        {
            if (_ConditionList == null)
                _ConditionList = new List<ICondition>();
            _ConditionList.Add(search_condition);
        }


        public Conditions(ISqlExpression left_operand, Comparisons comparison, ISqlExpression right_operand, bool negation)
        {
            if (_ConditionList == null)
                _ConditionList = new List<ICondition>();
            _ConditionList.Add(new Condition(left_operand, comparison, right_operand, negation));
        }

        public Conditions(ISqlExpression left_operand, Comparisons comparison, string right_operand, bool negation) :
            this(left_operand, comparison, new SqlColumnName(right_operand), negation)
        {
        }

        public Conditions(string left_operand, Comparisons comparison, ISqlExpression right_operand, bool negation) :
            this(new SqlColumnName(left_operand), comparison, right_operand, negation)
        {
        }

        public Conditions(string left_operand, Comparisons comparison, string right_operand,bool negation) :
            this(new SqlColumnName(left_operand),comparison,new SqlColumnName(right_operand),negation)
        {
        }

        public Conditions(ISqlExpression left_operand, Comparisons comparison, ISqlExpression right_operand) :
            this(left_operand, comparison, right_operand, false)
        {
        }

        public Conditions(ISqlExpression left_operand, Comparisons comparison, string right_operand) :
            this(left_operand, comparison, new SqlColumnName(right_operand), false)
        {
        }

        public Conditions(string left_operand, Comparisons comparison, ISqlExpression right_operand) :
            this(new SqlColumnName(left_operand), comparison, right_operand, false)
        {
        }

        public Conditions(string left_operand, Comparisons comparison, string right_operand) :
            this(new SqlColumnName(left_operand), comparison, new SqlColumnName(right_operand), false)
        {
        }
        #endregion

        #region Methods
        public Conditions Not()
        {
            Negation = true;
            return this;
        }
        #endregion

        public int Count
        {
            get { return (_ConditionList == null ? 0 : _ConditionList.Count); }
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

        public Conditions And(ICondition search_condition)
        {
            search_condition.ConditionType = ConditionType.And;
            _ConditionList.Add(search_condition);
            return this;
        }

        public Conditions AndIsEqual(ISqlExpression a, ISqlExpression b)
        {
            var search_condition = Condition.IsEqual(a, b);
            search_condition.ConditionType = ConditionType.And;
            _ConditionList.Add(search_condition);
            return this;
        }

        public Conditions AndIsEqual(string a, ISqlExpression b)
        {
            var search_condition = Condition.IsEqual(a, b);
            search_condition.ConditionType = ConditionType.And;
            _ConditionList.Add(search_condition);
            return this;
        }

        public Conditions AndIsEqual(ISqlExpression a, string b)
        {
            var search_condition = Condition.IsEqual(a, b);
            search_condition.ConditionType = ConditionType.And;
            _ConditionList.Add(search_condition);
            return this;
        }

        public Conditions AndIsEqual(ICondition condition)
        {
            _ConditionList.Add(condition);
            return this;
        }

        public Conditions Or(ICondition search_condition)
        {
            search_condition.ConditionType = ConditionType.Or;
            _ConditionList.Add(search_condition);
            return this;
        }

        public Conditions OrIsEqual(ISqlExpression a, ISqlExpression b)
        {
            var search_condition = Condition.IsEqual(a, b);
            search_condition.ConditionType = ConditionType.Or;
            _ConditionList.Add(search_condition);
            return this;
        }

        public Conditions OrIsEqual(string a, ISqlExpression b)
        {
            var search_condition = Condition.IsEqual(a, b);
            search_condition.ConditionType = ConditionType.Or;
            _ConditionList.Add(search_condition);
            return this;
        }

        public Conditions OrIsEqual(ISqlExpression a, string b)
        {
            var search_condition = Condition.IsEqual(a, b);
            search_condition.ConditionType = ConditionType.Or;
            _ConditionList.Add(search_condition);
            return this;
        }

        public Conditions OrIsEqual(string a, string b)
        {
            var search_condition = Condition.IsEqual(a, b);
            search_condition.ConditionType = ConditionType.Or;
            _ConditionList.Add(search_condition);
            return this;
        }



        // IsNull Operator
        public Conditions AndIsNull(ISqlExpression a)
        {
            return new Conditions(a, Comparisons.IsNull, SqlLiteral.Null);
        }

        public Conditions AndIsNotNull(ISqlExpression a)
        {
            return new Conditions(a, Comparisons.IsNull, SqlLiteral.Null).Not();
        }

        public Conditions AndIsNull(string a)
        {
            return AndIsNull(new SqlColumnName(a));
        }

        public Conditions AndIsNotNull(string a)
        {
            return AndIsNotNull(new SqlColumnName(a));
        }



        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            int count = _ConditionList.Count;

            if (count == 0)
                return;

            if (Negation)
                sql.Append(" NOT ");

            sql.Append("(");
            for (int x = 0; x < count; x++)
            {
                // concat operator
                if (x > 0)
                {
                    if (_ConditionList[x].ConditionType == ConditionType.And)
                        sql.Append(" AND ");
                    else if (_ConditionList[x].ConditionType == ConditionType.Or)
                        sql.Append(" OR ");
                }
                _ConditionList[x].GetSql(db_target,ref sql);
            }
            sql.Append(")");
         }

        #region Statics Methods
        
        // IsEqual Operator

        public static Conditions IsEqual(ISqlExpression a, ISqlExpression b)
        {
            return new Conditions(a, Comparisons.Equals, b);
        }

        public static Conditions IsEqual(ISqlExpression a, string b)
        {
            return IsEqual(a, new SqlColumnName(b));
        }

        public static Conditions IsEqual(string a, ISqlExpression b)
        {
            return IsEqual(new SqlColumnName(a), b);
        }

        public static Conditions IsEqual(string a, string b)
        {
            return IsEqual(new SqlColumnName(a), new SqlColumnName(b));
        }

        // IsGreaterThan Operator

        public static Conditions IsGreaterThan(ISqlExpression a, ISqlExpression b)
        {
            return new Conditions(a, Comparisons.GreaterThan, b);
        }

        public static Conditions IsGreaterThan(ISqlExpression a, string b)
        {
            return IsGreaterThan(a, new SqlColumnName(b));
        }

        public static Conditions IsGreaterThan(string a, ISqlExpression b)
        {
            return IsGreaterThan(new SqlColumnName(a), b);
        }

        public static Conditions IsGreaterThan(string a, string b)
        {
            return IsGreaterThan(new SqlColumnName(a), new SqlColumnName(b));
        }

        // IsGreaterOrEquals Operator

        public static Conditions IsGreaterOrEqual(ISqlExpression a, ISqlExpression b)
        {
            return new Conditions(a, Comparisons.GreaterOrEquals, b);
        }

        public static Conditions IsGreaterOrEqual(ISqlExpression a, string b)
        {
            return IsGreaterOrEqual(a, new SqlColumnName(b));
        }

        public static Conditions IsGreaterOrEqual(string a, ISqlExpression b)
        {
            return IsGreaterOrEqual(new SqlColumnName(a), b);
        }

        public static Conditions IsGreaterOrEqual(string a, string b)
        {
            return IsGreaterOrEqual(new SqlColumnName(a), new SqlColumnName(b));
        }

        // IsLessThan Operator

        public static Conditions IsLessThan(ISqlExpression a, ISqlExpression b)
        {
            return new Conditions(a, Comparisons.LessThan, b);
        }

        public static Conditions IsLessThan(ISqlExpression a, string b)
        {
            return IsLessThan(a, new SqlColumnName(b));
        }

        public static Conditions IsLessThan(string a, ISqlExpression b)
        {
            return IsLessThan(new SqlColumnName(a), b);
        }

        public static Conditions IsLessThan(string a, string b)
        {
            return IsLessThan(new SqlColumnName(a), new SqlColumnName(b));
        }

        // IsLessOrEquals Operator

        public static Conditions IsLessOrEqual(ISqlExpression a, ISqlExpression b)
        {
            return new Conditions(a, Comparisons.LessOrEquals, b);
        }

        public static Conditions IsLessOrEqual(ISqlExpression a, string b)
        {
            return IsLessOrEqual(a, new SqlColumnName(b));
        }

        public static Conditions IsLessOrEqual(string a, ISqlExpression b)
        {
            return IsLessOrEqual(new SqlColumnName(a), b);
        }

        public static Conditions IsLessOrEqual(string a, string b)
        {
            return IsLessOrEqual(new SqlColumnName(a), new SqlColumnName(b));
        }


        // IsLike Operator
        public static Conditions IsLike(ISqlExpression a, string b)
        {
            return new Conditions(a, Comparisons.Like, new SqlString(b));
        }

        public static Conditions IsLike(string a, string b)
        {
            return IsLike(new SqlColumnName(a), b);
        }

        // In Operator
        public static Conditions IsIn(ISqlExpression a, ISqlExpression b)
        {
            return new Conditions(a, Comparisons.In, b);
        }

        public static Conditions IsIn(ISqlExpression a, string b)
        {
            return IsIn(a, new SqlColumnName(b));
        }

        public static Conditions IsIn(string a, ISqlExpression b)
        {
            return IsIn(new SqlColumnName(a), b);
        }

        public static Conditions IsIn(string a, string b)
        {
            return IsIn(new SqlColumnName(a), new SqlLiteral(b));
        }

        public static Conditions IsIn(string a, int[] items)
        {
            var sb = new StringBuilder();
            for (var x = 0; x < items.Length; x++)
            {
                if (x > 0)
                    sb.Append(", ");
                sb.Append(items[x]);
            }
            return IsIn(a, new SqlLiteral(sb.ToString()));
        }

        public static Conditions IsIn(string a, string[] items)
        {
            var sb = new StringBuilder();
            for (var x = 0; x < items.Length; x++)
            {
                if (x > 0)
                    sb.Append(", ");
                sb.Append("'");
                sb.Append(items[x].Replace("'", "''"));
                sb.Append("'");
            }
            return IsIn(a, new SqlLiteral(sb.ToString()));
        }

        // IsNull Operator
        public static Conditions IsNull(ISqlExpression a)
        {
            return new Conditions(a, Comparisons.IsNull, SqlLiteral.Null);
        }


        public static Conditions IsNull(string a)
        {
            return IsNull(new SqlColumnName(a));
        }

        // Between Operator
        public static Conditions Between(ISqlExpression a, ISqlExpression b)
        {
            return new Conditions(a, Comparisons.Between, b);
        }

        public static Conditions Between(ISqlExpression a, string b)
        {
            return Between(a, new SqlColumnName(b));
        }

        public static Conditions Between(string a, ISqlExpression b)
        {
            return Between(new SqlColumnName(a), b);
        }

        public static Conditions Between(string a, string b)
        {
            return Between(new SqlColumnName(a), new SqlColumnName(b));
        }
        
        #endregion

    }
}
