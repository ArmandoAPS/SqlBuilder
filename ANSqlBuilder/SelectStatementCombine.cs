using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class SelectStatementCombine: QueryStatement
    {
        protected List<SelectStatement> _Selects;
        protected CombineType _CombineType;
        protected List<IOrderByExpression> _OrderBy;

        public SelectStatementCombine Add(SelectStatement select_statement)
        {
            if (_Selects == null)
                _Selects = new List<SelectStatement>();

            _Selects.Add(select_statement);
            return this;
        }

        public SelectStatementCombine(CombineType combine_type)
        {
            _CombineType = combine_type;
        }

        public CombineType CombineType
        {
            get { return _CombineType; }
            set { _CombineType = value; }
        }

 
        public override List<string> GetColumnsAlias()
        {
            List<string> alias = new List<string>();
            int count = _Selects[0].Columns.Count;
            for (int x = 0; x < count; x++)
                alias.Add(_Selects[0].Columns[x].Alias.Text);
            return alias;
        }

        public SelectStatementCombine OrderBy(OrderByExpression expression)
        {
            if (_OrderBy == null)
                _OrderBy = new List<IOrderByExpression>();
            _OrderBy.Add(expression);
            return this;
        }

        public SelectStatementCombine OrderBy(string expression, SortType sort_type)
        {
            if (_OrderBy == null)
                _OrderBy = new List<IOrderByExpression>();
            _OrderBy.Add(new OrderByExpression(expression, sort_type));
            return this;
        }

        public SelectStatementCombine OrderBy(IOrderByExpression order_by_expression)
        {
            if (_OrderBy == null)
                _OrderBy = new List<IOrderByExpression>();
            _OrderBy.Add(order_by_expression);
            return this;
        }

        public SelectStatementCombine OrderBy(ISqlExpression expression, SortType sort_type)
        {
            if (_OrderBy == null)
                _OrderBy = new List<IOrderByExpression>();
            _OrderBy.Add(new OrderByExpression(expression, sort_type));
            return this;
        }

        public override void GetSql(DbTarget db_target,ref StringBuilder sql)
        {
            if (_Selects == null)
                return ;
            
            int count = _Selects.Count;
            string combine = "";
            switch (_CombineType)
            {
                case CombineType.Union:
                    combine = " UNION ";
                    break;

                case CombineType.UnionAll:
                    combine = " UNION ALL ";
                    break;

                case CombineType.Intersect:
                    combine = " INTERSECT ";
                    break;

                case CombineType.Except:
                    combine = " EXCEPT ";
                    break;

                case CombineType.Minus:
                    combine = " MINUS ";
                    break;
            }
            for (int x = 0; x < count; x++)
            {
                _Selects[x].GetSql(db_target, ref sql);
                if (x + 1 < count)
                    sql.Append(combine);
            }

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

        }


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
                Expressions.Add(new OrderByExpression(expression, SortType.Ascending));
                return this;
            }

            public OrderByClause Ascending(string expression)
            {
                Expressions.Add(new OrderByExpression(expression, SortType.Ascending));
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
                    return;

                sql.Append(" ORDER BY ");

                for (int x = 0; x < Count; x++)
                {
                    OrderByExpression item = _Expressions[x];

                    item.GetSql(db_target, ref sql);
                    if (x + 1 < Count)
                        sql.Append(", ");
                    else
                        sql.Append(" ");
                }

            }

        }

    }
}
