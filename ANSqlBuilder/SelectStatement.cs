using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Data.Common;
using System.Data;
using System.IO;
using ANCommon.Sql;


namespace ANSqlBuilder
{
    public class SelectStatement: QueryStatement
    {
        protected int _Top = 0;
        protected bool _IsDistinct;
        protected List<SelectColumn> _Columns;
        protected FromClause _From;
        protected ConditionClause _Where;
        protected List<ISqlExpression> _GroupBy;
        protected ConditionClause _Having;
        protected List<IOrderByExpression> _OrderBy;

        #region Constructor

        public SelectStatement()
        {
            
        }
        public SelectStatement(bool distinct)
        {
            _IsDistinct = distinct;
        }

        public SelectStatement(int top)
        {
            _IsDistinct = false;
            _Top = top;
        }

        public SelectStatement(bool distinct, int top):base()
        {
            _IsDistinct = distinct;
            _Top = top;
        }

  
        #endregion
        #region Properties

        public int Top
        {
            get { return _Top; }
            set
            {
                _Top = value;
            }
        }

        public bool IsDistinct
        {
            get { return _IsDistinct; }
            set { _IsDistinct = value; }
        }

        public List<SelectColumn> Columns
        {
            get
            {
                if (_Columns == null)
                    _Columns = new List<SelectColumn>();
                return _Columns;
            }
            set
            {
                _Columns = value;
            }
        }


        public override void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            sql.Append("SELECT ");
            if (IsDistinct)
                sql.Append("DISTINCT ");

            if (db_target == DbTarget.SqlServer && Top > 0)
                sql.Append(String.Format("TOP {0} ", _Top));

            int count = Columns.Count;
            if (count == 0)
                sql.Append(" * ");
            else
            {
                for (int x = 0; x < count; x++)
                {
                    if(x > 0)
                        sql.Append(",");
                    
                    SelectColumn col = _Columns[x];
                    col.GetSql(db_target,ref sql);
                }
            }
            if (_From != null)
               _From.GetSql(db_target, ref sql);

            if(_Where != null && _Where.Count > 0)
                _Where.GetSql(db_target, ref sql);

            if (_GroupBy != null)
            {
                count = _GroupBy.Count;
                if (count > 0)
                {
                    sql.Append(" GROUP BY ");

                    for (int x = 0; x < count; x++)
                    {
                        ISqlExpression item = _GroupBy[x];

                        if (!item.IsLiteral)
                            sql.Append("(");
                        item.GetSql(db_target, ref sql);
                        if (!item.IsLiteral)
                            sql.Append(")");
                        if (x + 1 < count)
                            sql.Append(", ");
                        else
                            sql.Append(" ");
                    }
                }
            }
            
            if (_Having != null && _Having.Count > 0)
                _Having.GetSql(db_target, ref sql);

            if (_OrderBy != null)
            {
                count = _OrderBy.Count;
                if (count > 0)
                {

                    sql.Append(" ORDER BY ");

                    for (int x = 0; x < count; x++)
                    {
                        IOrderByExpression item = _OrderBy[x];

                        item.GetSql(db_target, ref sql);
                        if (x + 1 < count)
                            sql.Append(",");

                    }
                }        
            }

            if ((db_target == DbTarget.SqlLite || db_target == DbTarget.MySql) && Top > 0)
                sql.Append(String.Format(" LIMIT {0} ", _Top));
        }
        #endregion

        #region Methods

        public SelectStatement Distinct()
        {
            IsDistinct = true;
            return this;
        }

        public SelectStatement SetTop(int top)
        {
            Top = top;
            return this;
        }

        // se oculta este metodo

        public SelectStatement From(ISqlExpression table_source, string alias)
        {
            if (_From == null)
                _From = new FromClause();
            _From.TableSource = table_source;
            _From.Alias = alias.Trim();
            return this;
        }

        public SelectStatement From(string table_source)
        {
            return From(table_source, table_source);
        }

        public SelectStatement From(string table_source, string alias)
        {
            if (_From == null)
                _From = new FromClause();
            _From.TableSource = new SqlLiteral(table_source);
            _From.Alias = alias;
            return this;
        }

        public SelectStatement SetFrom(FromClause from_clause)
        {
            _From = from_clause;
            return this;
        }

        public SelectStatement AddJoin(FromClause.JoinClause join_clause)
        {
            _From.Joins.Add(join_clause);
            return this;
        }

        public SelectStatement InnerJoin(ISqlExpression table_source, string alias, ICondition search_condition)
        {
            _From.InnerJoin(table_source, alias, search_condition);
            return this;
        }

        public SelectStatement InnerJoin(string table_name, ICondition search_condition)
        {
            _From.InnerJoin(new SqlLiteral(table_name), table_name, search_condition);
            return this;
        }

        public SelectStatement InnerJoin(string table_name, string alias, ICondition search_condition)
        {
            _From.InnerJoin(new SqlLiteral(table_name), alias, search_condition);
            return this;
        }

        public SelectStatement LeftJoin(ISqlExpression table_source, string alias, ICondition search_condition)
        {
            _From.LeftJoin(table_source, alias, search_condition);
            return this;
        }

        public SelectStatement LeftJoin(string table_name, ICondition search_condition)
        {
            _From.LeftJoin(new SqlLiteral(table_name), table_name, search_condition);
            return this;
        }

        public SelectStatement LeftJoin(string table_source, string alias, ICondition search_condition)
        {
            _From.LeftJoin(new SqlLiteral(table_source), alias, search_condition);
            return this;
        }

        public SelectStatement RightJoin(ISqlExpression table_source, string alias, ICondition search_condition)
        {
            _From.RightJoin(table_source, alias, search_condition);
            return this;
        }

        public SelectStatement RightJoin(string table_name, ICondition search_condition)
        {
            _From.RightJoin(new SqlLiteral(table_name), table_name, search_condition);
            return this;
        }

        public SelectStatement RightJoin(string table_source, string alias, ICondition search_condition)
        {
            _From.RightJoin(new SqlLiteral(table_source), alias, search_condition);
            return this;
        }

        public SelectStatement Where(ICondition condition)
        {
            if (condition != null)
            {
                if (_Where == null)
                    _Where = new ConditionClause("WHERE");
                _Where.And(condition);
            }
            return this;
        }

        public SelectStatement WhereNot(ICondition condition)
        {
            if (condition == null)
                return this;

            condition.Negation = true;
            return Where(condition);

        }

        /* WhereIsNull */
        public SelectStatement WhereIsNull(string a)
        {
            return Where(Condition.IsNull(new SqlColumnName(a)));
        }

        public SelectStatement WhereIsNull(ISqlExpression a)
        {
            return Where(Condition.IsNull(a));
        }

        public SelectStatement WhereNotIsNull(string a)
        {
            return WhereNot(Condition.IsNull(new SqlColumnName(a)));
        }

        public SelectStatement WhereNotIsNull(ISqlExpression a)
        {
            return WhereNot(Condition.IsNull(a));
        }

        public SelectStatement WhereIsNullOrEmpty(ISqlExpression a)
        {
            return Where(Conditions.IsNull(a).OrIsEqual(a, SqlString.Empty));
        }

        public SelectStatement WhereNotIsNullOrEmpty(string a)
        {
            var expression = new SqlColumnName(a);
            return WhereNot(Conditions.IsNull(expression).OrIsEqual(expression,SqlString.Empty));
        }

        /* WhereIsEqual */
        public SelectStatement WhereIsEqual(ISqlExpression a, ISqlExpression b)
        {
            return Where(Condition.IsEqual(a,b));
        }

        public SelectStatement WhereIsEqual(ISqlExpression a, string b)
        {
            return Where(Condition.IsEqual(a, b));
        }

        public SelectStatement WhereIsEqual(string a, ISqlExpression b)
        {
            return Where(Condition.IsEqual(a, b));
        }

        public SelectStatement WhereIsEqual(string a, string b)
        {
            return Where(Condition.IsEqual(a, b));
        }

        /* WhereIsNotEqual */
        public SelectStatement WhereIsNotEqual(ISqlExpression a, ISqlExpression b)
        {
            return WhereNot(Condition.IsEqual(a, b));
        }

        public SelectStatement WhereIsNotEqual(ISqlExpression a, string b)
        {
            return WhereNot(Condition.IsEqual(a, b));
        }

        public SelectStatement WhereIsNotEqual(string a, ISqlExpression b)
        {
            return WhereNot(Condition.IsEqual(a, b));
        }

        public SelectStatement WhereIsNotEqual(string a, string b)
        {
            return WhereNot(Condition.IsEqual(a, b));
        }

        /* WhereIsIn */
        public SelectStatement WhereIsIn(ISqlExpression a, ISqlExpression b)
        {
            return Where(Condition.IsIn(a, b));
        }


        public SelectStatement WhereIsIn(string a, ISqlExpression b)
        {
            return Where(Condition.IsIn(a, b));
        }


        /* WhereNotIsIn */
        public SelectStatement WhereNotIsIn(ISqlExpression a, ISqlExpression b)
        {
            return WhereNot(Condition.IsIn(a, b));
        }


        public SelectStatement WhereNotIsIn(string a, ISqlExpression b)
        {
            return WhereNot(Condition.IsIn(a, b));
        }
 

        //public SelectStatement Where(ISqlExpression left_operand, Comparisons comparison, ISqlExpression right_operand)
        //{
        //    return Where(new Condition(left_operand, comparison, right_operand));
        //}

        //public SelectStatement Where(string left_operand, Comparisons comparison, ISqlExpression right_operand)
        //{
        //    return Where(new Condition(left_operand, comparison, right_operand));
            
        //}

        //public SelectStatement Where(ISqlExpression left_operand, Comparisons comparison, string right_operand)
        //{
        //    return Where(new Condition(left_operand, comparison, right_operand));
            
        //}

        //public SelectStatement Where(string left_operand, Comparisons comparison, string right_operand)
        //{
        //    return Where(new Condition(left_operand,comparison, right_operand));
            
        //}
        
        //public SelectStatement WhereNot(ISqlExpression left_operand, Comparisons comparison, ISqlExpression right_operand)
        //{
        //    return Where(new Condition(left_operand,comparison, right_operand,true));
            
        //}

        //public SelectStatement WhereNot(string left_operand, Comparisons comparison, ISqlExpression right_operand)
        //{
        //    return Where(new Condition(left_operand, comparison, right_operand,true));
           
        //}

        //public SelectStatement WhereNot(ISqlExpression left_operand, Comparisons comparison, string right_operand)
        //{
        //    return Where(new Condition(left_operand,comparison, right_operand, true));
            
        //}

        //public SelectStatement WhereNot(string left_operand, Comparisons comparison, string right_operand)
        //{
        //    return Where(new Condition(left_operand,comparison, right_operand,true));
            
        //}


        public SelectStatement WhereExists(ISqlExpression expression)
        {
            return Where(new ExistsCondition(expression));
        }

        public SelectStatement WhereNotExists(ISqlExpression expression)
        {
            return WhereNot(new ExistsCondition(expression));
        }


        public SelectStatement Having(ICondition condition)
        {
            if (_Having == null)
                _Having = new ConditionClause("HAVING");
            _Having.And(condition);
            return this;
        }

        public SelectStatement Column(ISqlExpression expression, string alias)
        {
            Columns.Add(new SelectColumn(expression, alias));
            return this;
        }

        public SelectStatement Column(string column_name, string alias)
        {
            Columns.Add(new SelectColumn(column_name, alias));
            return this;
        }

        public SelectStatement Column(ISqlExpression expression)
        {
            Columns.Add(new SelectColumn(expression));
            return this;
        }

        public SelectStatement Column(string column_name)
        {
            return Column(column_name, column_name);
        }

        public SelectStatement Column(string table_alias, string column_name, string alias)
        {
            Columns.Add(new SelectColumn(new SqlColumnName(table_alias, column_name), alias));
            return this;
        }

        public SelectStatement SumColumn(ISqlExpression expression, string alias)
        {
            return AggregateColumn(AggregateFunctionType.Sum, expression, alias);
        }

        public SelectStatement SumColumn(string column_name, string alias)
        {
            return AggregateColumn(AggregateFunctionType.Sum, column_name, alias);
        }

        public SelectStatement AvgColumn(ISqlExpression expression, string alias)
        {
            return AggregateColumn(AggregateFunctionType.Avg, expression, alias);
        }

        public SelectStatement AvgColumn(string column_name, string alias)
        {
            return AggregateColumn(AggregateFunctionType.Avg, column_name, alias);
        }

        public SelectStatement MaxColumn(ISqlExpression expression, string alias)
        {
            return AggregateColumn(AggregateFunctionType.Max, expression, alias);
        }

        public SelectStatement MaxColumn(string column_name, string alias)
        {
            return AggregateColumn(AggregateFunctionType.Max, column_name, alias);
        }

        public SelectStatement MinColumn(ISqlExpression expression, string alias)
        {
            return AggregateColumn(AggregateFunctionType.Min, expression, alias);
        }

        public SelectStatement MinColumn(string column_name, string alias)
        {
            return AggregateColumn(AggregateFunctionType.Min, column_name, alias);
        }

        public SelectStatement CountColumn(string column_name, string alias)
        {
            return AggregateColumn(AggregateFunctionType.Count, column_name, alias);
        }

        public SelectStatement CountColumn()
        {
            return AggregateColumn(AggregateFunctionType.Count, new SqlLiteral("*"), "count");
        }

        private SelectStatement AggregateColumn(AggregateFunctionType aggregate_function_type, ISqlExpression expression,string alias)
        {
            Columns.Add(new SelectColumn(new AggregateFunction(aggregate_function_type, expression),alias));
            return this;
        }

        private SelectStatement AggregateColumn(AggregateFunctionType aggregate_function_type, string column_name,string alias)
        {
            Columns.Add(new SelectColumn(new AggregateFunction(aggregate_function_type, column_name),alias));
            return this;
        }

        public SelectStatement GroupBy(ISqlExpression expression)
        {
            if (expression == null)
                return this;

            if (_GroupBy == null)
                _GroupBy = new List<ISqlExpression>();
            _GroupBy.Add(expression);
            return this;
        }

        public SelectStatement GroupBy(string expression)
        {
            if (expression == null)
                return this;

            if (_GroupBy == null)
                _GroupBy = new List<ISqlExpression>();
            _GroupBy.Add(new SqlLiteral(expression));
            return this;
        }

        public SelectStatement OrderBy(OrderByExpression expression)
        {
            if (expression == null)
                return this;

            if (_OrderBy == null)
                _OrderBy = new List<IOrderByExpression>();
            _OrderBy.Add(expression);
            return this;
        }

        public SelectStatement OrderBy(string expression, SortType sort_type)
        {
            if (_OrderBy == null)
                _OrderBy = new List<IOrderByExpression>();
            _OrderBy.Add(new OrderByExpression(expression, sort_type));
            return this;
        }

        public SelectStatement OrderBy(IOrderByExpression order_by_expression)
        {
            if (order_by_expression == null)
                return this;

            if (_OrderBy == null)
                _OrderBy = new List<IOrderByExpression>();
            _OrderBy.Add(order_by_expression);
            return this;
        }

        public SelectStatement OrderBy(ISqlExpression expression, SortType sort_type)
        {
            if (expression == null)
                return this;

            if (_OrderBy == null)
                _OrderBy = new List<IOrderByExpression>();
            _OrderBy.Add(new OrderByExpression(expression, sort_type));
            return this;
        }


        public object ExecuteScalar()
        {
            using (DbHelper helper = new DbHelper(this.ConnectionStringName))
            {
                StringBuilder sql = new StringBuilder();
                GetSql(DbTarget, ref sql);
                return helper.ExecuteScalar(sql.ToString());
            }
        }

        public override List<string> GetColumnsAlias()
        {
            List<string> alias = new List<string>();
            int count = Columns.Count;
            for (int x = 0; x < count; x++)
                alias.Add(Columns[x].Alias.Text);
            return alias;
        }

        #endregion

        #region Inner Classes


        public class OrderByClause
        {
            protected List<OrderByExpression> _Expressions;

            public List<OrderByExpression> Expressions
            {
                get
                {
                    if (_Expressions == null)
                        _Expressions = new List<OrderByExpression>();
                    return _Expressions;
                }
                set
                {
                    _Expressions = value;
                }
            }
            public OrderByClause Ascending(ISqlExpression expression)
            {
                Expressions.Add(new OrderByExpression(expression,SortType.Ascending));
                return this;
            }

            public OrderByClause Ascending(string expression)
            {
                Expressions.Add(new OrderByExpression(expression,SortType.Ascending));
                return this;
            }

            public OrderByClause Descending(ISqlExpression expression)
            {
                Expressions.Add(new OrderByExpression(expression, SortType.Descending));
                return this;
            }

            public OrderByClause Descending(string expression)
            {
                Expressions.Add(new OrderByExpression(expression, SortType.Descending));
                return this;
            }


            public void GetSql(DbTarget db_target, ref StringBuilder sql)
            {
                int Count = Expressions.Count;
                if (Count <= 0)
                    return ;

                sql.Append(" ORDER BY ");

                for (int x = 0; x < Count; x++)
                {
                    OrderByExpression item = _Expressions[x];

                    item.GetSql(db_target,ref sql);
                    if (x + 1 < Count)
                        sql.Append(", ");
                    else
                        sql.Append(" ");
                }

            }

        }



        #endregion


    }
}
