using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class OrderByExpressions : IOrderByExpression
    {
        protected List<OrderByExpression> _list = new List<OrderByExpression>();

        public OrderByExpressions()
        {
        }

        public OrderByExpressions(ISqlExpression expression, SortType sort_type)
        {
            _list.Add(new OrderByExpression(expression, sort_type));
        }

        public OrderByExpressions(OrderByExpression order_by_expression)
        {
            _list.Add(order_by_expression);
        }

        public OrderByExpressions(string expression, SortType sort_type)
        {
            _list.Add(new OrderByExpression(expression, sort_type));
        }

        public void Add(OrderByExpression order_by_expression)
        {
            _list.Add(order_by_expression);
        }

        public OrderByExpressions Add(ISqlExpression expression, SortType sort_type)
        {
            _list.Add(new OrderByExpression(expression,sort_type));
            return this;
        }

        public OrderByExpressions Add(string expression, SortType sort_type)
        {
            _list.Add(new OrderByExpression(expression, sort_type));
            return this;
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {

            for (int x = 0; x < _list.Count; x++)
            {
                if (x > 0)
                    sql.Append(",");
                _list[x].GetSql(db_target, ref sql);

            }

        }
    }
}
