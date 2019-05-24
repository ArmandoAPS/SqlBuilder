using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;


namespace ANSqlBuilder
{
    public class CaseStatement: ISqlExpression
    {
        #region Properties
        protected ISqlExpression _InputExpression;
        protected ISqlExpression _ElseResultExpression;
        protected List<WhenExpression> _WhenExpressions;

        public ISqlExpression InputExpression
        {
            get { return _InputExpression; }
            set { _InputExpression = value; }
        }

        public List<WhenExpression> WhenExpressions
        {
            get { return _WhenExpressions; }
            set { _WhenExpressions = value; }
        }

        public ISqlExpression ElseResultExpression
        {
            get { return _ElseResultExpression; }
            set { _ElseResultExpression = value; }
        }

        #endregion
        public CaseStatement()
        {
        }

        public CaseStatement(ISqlExpression input_expression)
        {
            _InputExpression = input_expression;
            _WhenExpressions = new List<WhenExpression>();
        }

        public CaseStatement When(ISqlExpression conditional_expression, ISqlExpression then_expression)
        {
            _WhenExpressions.Add(new WhenExpression(conditional_expression, then_expression));
            return this;
        }

        public CaseStatement When(string conditional_expression, ISqlExpression then_expression)
        {
            _WhenExpressions.Add(new WhenExpression(new SqlLiteral(conditional_expression), then_expression));
            return this;
        }

        public CaseStatement When(ISqlExpression conditional_expression, string then_expression)
        {
            _WhenExpressions.Add(new WhenExpression(conditional_expression, new SqlLiteral(then_expression)));
            return this;
        }

        public CaseStatement When(string conditional_expression, string then_expression)
        {
            _WhenExpressions.Add(new WhenExpression(new SqlLiteral(conditional_expression), new SqlLiteral(then_expression)));
            return this;
        }

        public CaseStatement Else(ISqlExpression else_expression)
        {
            _ElseResultExpression = else_expression;
            return this;
        }

        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            sql.Append("CASE ");
            if (!_InputExpression.IsLiteral)
                sql.Append("(");
            _InputExpression.GetSql(db_target, ref sql);
            if (!_InputExpression.IsLiteral)
                sql.Append(")");
            foreach(WhenExpression we in _WhenExpressions)
            {
                sql.Append(" WHEN ");
                we.BooleanExpression.GetSql(db_target, ref sql);
                sql.Append(" THEN ");
                if (!we.ResultExpression.IsLiteral)
                    sql.Append("(");
                we.ResultExpression.GetSql(db_target, ref sql);
                if (!we.ResultExpression.IsLiteral)
                    sql.Append(")");
            }
            if (_ElseResultExpression != null)
            {
                sql.Append(" ELSE ");
                if (!_ElseResultExpression.IsLiteral)
                    sql.Append("(");
                _ElseResultExpression.GetSql(db_target, ref sql);
                if (!_ElseResultExpression.IsLiteral)
                    sql.Append(")");
            }
            sql.Append(" END ");
 
        }

    }
}
