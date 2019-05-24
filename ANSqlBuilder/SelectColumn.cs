using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class SelectColumn
    {
        protected ISqlExpression _Expression;
        protected SqlName _Alias;
        protected AggregateFunctionType _AggregateFunctionType;
        protected SortType _SortType;
        protected int _SortOrder;
        protected bool _GroupBy;
        
        public SelectColumn(ISqlExpression expression, string alias)
        {
            _Expression = expression;
            _Alias = new SqlName(alias);
        }

        public SelectColumn(string name, string alias)
        {
            _Expression = new SqlLiteral(name);
            _Alias = new SqlName(alias);
        }

        public SelectColumn(string name)
        {
            _Expression = new SqlLiteral(name);
        }

        public SelectColumn(ISqlExpression expression)
        {
            _Expression = expression;
        }

        public ISqlExpression Expression
        {
            get { return _Expression; }
            set { _Expression = value; }
        }

        public SqlName Alias
        {
            get { return _Alias; }
            set { _Alias = value; }
        }


        public AggregateFunctionType AggregateFunctionType
        {
            get { return _AggregateFunctionType; }
            set { _AggregateFunctionType = value; }
        }

        public SortType SortType
        {
            get { return _SortType; }
            set { _SortType = value; }
        }

        public int SortOrder
        {
            get { return _SortOrder; }
            set { _SortOrder = value; }
        }

        public bool GroupBy
        {
            get { return _GroupBy; }
            set { _GroupBy = value; }
        }

        public bool IsLiteral
        {
            get { return false; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            if (!Expression.IsLiteral)
                sql.Append("(");
 
            Expression.GetSql(db_target,ref sql);
            if (!Expression.IsLiteral)
                sql.Append(")");
            sql.Append(" ");
            if (Alias != null)
            {
                sql.Append(" as ");
                Alias.GetSql(db_target, ref sql);
            }
        }
    }
}
