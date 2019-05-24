using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;


namespace ANSqlBuilder
{
    public class Condition : ICondition
    {
        protected ISqlExpression _LeftOperand;
        protected Comparisons _Comparison;
        protected ISqlExpression _RightOperand;
        protected bool _Negation;
        protected ConditionType _ConditionType;

        #region Constructors

        public Condition(ISqlExpression left_operand, Comparisons comparison, ISqlExpression right_operand, bool negation)
        {
            _LeftOperand = left_operand;
            _Comparison = comparison;
            _RightOperand = right_operand;
            _Negation = negation;
        }

        public Condition(ISqlExpression left_operand, Comparisons comparison, string right_operand, bool negation) :
            this(left_operand, comparison, new SqlColumnName(right_operand), negation)
        {
        }

        public Condition(string left_operand, Comparisons comparison, ISqlExpression right_operand, bool negation) :
            this(new SqlColumnName(left_operand), comparison, right_operand, negation)
        {
        }

        public Condition(string left_operand, Comparisons comparison, string right_operand, bool negation) :
            this(new SqlColumnName(left_operand), comparison, new SqlColumnName(right_operand), negation)
        {
        }

        public Condition(ISqlExpression left_operand, Comparisons comparison, ISqlExpression right_operand) :
            this(left_operand, comparison, right_operand, false)
        {
        }

        public Condition(ISqlExpression left_operand, Comparisons comparison, string right_operand) :
            this(left_operand, comparison, new SqlColumnName(right_operand), false)
        {
        }

        public Condition(string left_operand, Comparisons comparison, ISqlExpression right_operand) :
            this(new SqlColumnName(left_operand), comparison, right_operand, false)
        {
        }

        public Condition(string left_operand, Comparisons comparison, string right_operand) :
            this(new SqlColumnName(left_operand), comparison, new SqlColumnName(right_operand), false)
        {
        }



        #endregion Constructors

        #region Methods
        public Condition Not()
        {
            Negation = true;
            return this;
        }
        #endregion

        #region Statics Methods

        // IsEqual Operator

        public static Condition IsEqual(ISqlExpression a, ISqlExpression b)
        {
            return new Condition(a, Comparisons.Equals, b);
        }

        public static Condition IsEqual(ISqlExpression a, string b)
        {
            return IsEqual(a, new SqlColumnName(b));
        }

        public static Condition IsEqual(string a, ISqlExpression b)
        {
            return IsEqual(new SqlColumnName(a), b);
        }

        public static Condition IsEqual(string a, string b)
        {
            return IsEqual(new SqlColumnName(a), new SqlColumnName(b));
        }

        // IsNotEqual Operator

        public static Condition IsNotEqual(ISqlExpression a, ISqlExpression b)
        {
            return new Condition(a, Comparisons.NotEquals, b);
        }

        public static Condition IsNotEqual(ISqlExpression a, string b)
        {
            return IsNotEqual(a, new SqlColumnName(b));
        }

        public static Condition IsNotEqual(string a, ISqlExpression b)
        {
            return IsNotEqual(new SqlColumnName(a), b);
        }

        public static Condition IsNotEqual(string a, string b)
        {
            return IsNotEqual(new SqlColumnName(a), new SqlColumnName(b));
        }

        // IsGreaterThan Operator

        public static Condition IsGreaterThan(ISqlExpression a, ISqlExpression b)
        {
            return new Condition(a, Comparisons.GreaterThan, b);
        }

        public static Condition IsGreaterThan(ISqlExpression a, string b)
        {
            return IsGreaterThan(a, new SqlColumnName(b));
        }

        public static Condition IsGreaterThan(string a, ISqlExpression b)
        {
            return IsGreaterThan(new SqlColumnName(a), b);
        }

        public static Condition IsGreaterThan(string a, string b)
        {
            return IsGreaterThan(new SqlColumnName(a), new SqlColumnName(b));
        }

        // IsGreaterOrEquals Operator

        public static Condition IsGreaterOrEqual(ISqlExpression a, ISqlExpression b)
        {
            return new Condition(a, Comparisons.GreaterOrEquals, b);
        }

        public static Condition IsGreaterOrEqual(ISqlExpression a, string b)
        {
            return IsGreaterOrEqual(a, new SqlColumnName(b));
        }

        public static Condition IsGreaterOrEqual(string a, ISqlExpression b)
        {
            return IsGreaterOrEqual(new SqlColumnName(a), b);
        }

        public static Condition IsGreaterOrEqual(string a, string b)
        {
            return IsGreaterOrEqual(new SqlColumnName(a), new SqlColumnName(b));
        }

        // IsLessThan Operator

        public static Condition IsLessThan(ISqlExpression a, ISqlExpression b)
        {
            return new Condition(a, Comparisons.LessThan, b);
        }

        public static Condition IsLessThan(ISqlExpression a, string b)
        {
            return IsLessThan(a, new SqlColumnName(b));
        }

        public static Condition IsLessThan(string a, ISqlExpression b)
        {
            return IsLessThan(new SqlColumnName(a), b);
        }

        public static Condition IsLessThan(string a, string b)
        {
            return IsLessThan(new SqlColumnName(a), new SqlColumnName(b));
        }

        // IsLessOrEquals Operator

        public static Condition IsLessOrEqual(ISqlExpression a, ISqlExpression b)
        {
            return new Condition(a, Comparisons.LessOrEquals, b);
        }

        public static Condition IsLessOrEqual(ISqlExpression a, string b)
        {
            return IsLessOrEqual(a, new SqlColumnName(b));
        }

        public static Condition IsLessOrEqual(string a, ISqlExpression b)
        {
            return IsLessOrEqual(new SqlColumnName(a), b);
        }

        public static Condition IsLessOrEqual(string a, string b)
        {
            return IsLessOrEqual(new SqlColumnName(a), new SqlColumnName(b));
        }


        // IsLike Operator
        public static Condition IsLike(ISqlExpression a, string b)
        {
            return new Condition(a, Comparisons.Like, new SqlString(b));
        }

        public static Condition IsLike(string a, string b)
        {
            return new Condition(new SqlColumnName(a), Comparisons.Like, new SqlString(b));
        }

        // In Operator
        public static Condition IsIn(ISqlExpression a, ISqlExpression b)
        {
            return new Condition(a, Comparisons.In, b);
        }

        public static Condition IsIn(ISqlExpression a, string b)
        {
            return IsIn(a, new SqlColumnName(b));
        }

        public static Condition IsIn(string a, ISqlExpression b)
        {
            return IsIn(new SqlColumnName(a), b);
        }

        public static Condition IsIn(string a, string b)
        {
            return IsIn(new SqlColumnName(a), new SqlColumnName(b));
        }

        public static Condition IsIn(string a, int[] items)
        {
            var sb = new StringBuilder();
            for (var x = 0; x < items.Length; x++)
            {
                if (x > 0)
                    sb.Append(", ");
                sb.Append(items[x]);
            }
            return IsIn(new SqlColumnName(a), new SqlLiteral(sb.ToString()));
        }

        public static Condition IsIn(string a, string[] items)
        {
            return IsIn(new SqlColumnName(a), new SqlStringArray(items));
        }

        // IsNull Operator
        public static Condition IsNull(ISqlExpression a)
        {
            return new Condition(a, Comparisons.IsNull, SqlLiteral.Null);
        }


        public static Condition IsNull(string a)
        {
            return IsNull(new SqlColumnName(a));
        }

        // IsNull Operator
        public static Condition IsNotNull(ISqlExpression a)
        {
            return new Condition(a, Comparisons.IsNull, SqlLiteral.Null).Not();
        }


        public static Condition IsNotNull(string a)
        {
            return IsNull(new SqlColumnName(a)).Not();
        }

        // Between Operator
        public static Condition Between(ISqlExpression a, ISqlExpression b, ISqlExpression c)
        {
            return new Condition(a, Comparisons.Between, new AndExpression(b, c));
        }

        public static Condition Between(ISqlExpression a, string b, string c)
        {
            return Between(a, new SqlColumnName(b), new SqlColumnName(c));
        }

        public static Condition Between(string a, ISqlExpression b, ISqlExpression c)
        {
            return Between(new SqlColumnName(a), b, c);
        }

        public static Condition Between(string a, string b, string c)
        {
            return Between(new SqlColumnName(a), new SqlColumnName(b), new SqlColumnName(c));
        }


        #endregion

        #region Properties
        public ISqlExpression LeftOperand
        {
            get { return _LeftOperand; }
            set { _LeftOperand = value; }
        }

        public Comparisons Comparison
        {
            get { return _Comparison; }
            set { _Comparison = value; }
        }

        public ISqlExpression RightOperand
        {
            get { return _RightOperand; }
            set { _RightOperand = value; }
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


        public bool IsLiteral
        {
            get { return false; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            sql.Append("(");
            if (Negation)
                sql.Append(" NOT ");
            LeftOperand.GetSql(db_target, ref sql);
            sql.Append(" ");
            if (_Comparison == Comparisons.IsNull)
                sql.Append(" IS NULL");
            else if (_Comparison == Comparisons.In)
            {
                sql.Append(" IN (");
                if (!RightOperand.IsLiteral)
                    sql.Append("(");
                RightOperand.GetSql(db_target, ref sql);
                if (!RightOperand.IsLiteral)
                    sql.Append(")");
                sql.Append(")");
            }

            else
            {

                switch (_Comparison)
                {
                    case Comparisons.Equals:
                        sql.Append(" = ");
                        break;

                    case Comparisons.NotEquals:
                        sql.Append(" <> ");
                        break;

                    case Comparisons.GreaterThan:
                        sql.Append(" > ");
                        break;

                    case Comparisons.GreaterOrEquals:
                        sql.Append(" >= ");
                        break;

                    case Comparisons.LessThan:
                        sql.Append(" < ");
                        break;

                    case Comparisons.LessOrEquals:
                        sql.Append(" <= ");
                        break;

                    case Comparisons.Like:
                        sql.Append(" LIKE ");
                        break;

                    case Comparisons.Between:
                        sql.Append(" BETWEEN ");

                        break;
                }
                if (!RightOperand.IsLiteral)
                    sql.Append("(");
                RightOperand.GetSql(db_target, ref sql);
                if (!RightOperand.IsLiteral)
                    sql.Append(")");

            }
            sql.Append(")");
        }

        #endregion Properties
    }
}
