using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;


namespace ANSqlBuilder
{
    public class OrderByExpression : IOrderByExpression
    {
        protected ISqlExpression _Expression;
        protected SortType _SortType;

        public OrderByExpression(ISqlExpression expression, SortType sort_type)
        {
            _Expression = expression;
            _SortType = sort_type;
        }

        public OrderByExpression(string expression, SortType sort_type)
        {
            _Expression = new SqlColumnName(expression);
            _SortType = sort_type;
        }

        public OrderByExpression(string expression)
        {
            _Expression = new SqlColumnName(expression);
            _SortType = SortType.Ascending;
        }

        public ISqlExpression Expression
        {
            get { return _Expression; }
            set { _Expression = value; }
        }

        public SortType SortType
        {
            get { return _SortType; }
            set { _SortType = value; }
        }

        public void GetSql(DbTarget db_target,ref StringBuilder sql)
        {

            _Expression.GetSql(db_target, ref sql);
            sql.Append((this.SortType == SortType.Ascending) ? " ASC" : " DESC");
        }
    }
}
