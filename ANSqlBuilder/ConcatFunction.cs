using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class ConcatFunction:ISqlExpression
    {
        protected List<ISqlExpression> _items = new List<ISqlExpression>();

        private ISqlExpression Separator { get; set;}

        public ConcatFunction()
        {
            
        }

        public ConcatFunction(ISqlExpression separator)
        {
            Separator = separator;
        }

        public ConcatFunction(string separator)
        {
            Separator = new SqlString(separator);
        }

        public ConcatFunction(string expression1, string expression2)
        {
            _items.Add(new SqlLiteral(expression1));
            _items.Add(new SqlLiteral(expression2));
        }

        public ConcatFunction(string expression1, ISqlExpression expression2)
        {
            _items.Add(new SqlLiteral(expression1));
            _items.Add(expression2);
        }

        public ConcatFunction(ISqlExpression expression1, string expression2)
        {
            _items.Add(expression1);
            _items.Add(new SqlLiteral(expression2));
        }

        public ConcatFunction(ISqlExpression expression1, ISqlExpression expression2)
        {
            _items.Add(expression1);
            _items.Add(expression2);
        }

        public ConcatFunction(params ISqlExpression[] expressions)
        {
            for (int i = 0; i < expressions.Length; i++)
            {
                _items.Add(expressions[i]);
            }
        }

        public ConcatFunction Append(string expression)
        {
            _items.Add(new SqlLiteral(expression));
            return this;
        }

        public ConcatFunction Append(ISqlExpression expression)
        {
            _items.Add(expression);
            return this;
        }

        public bool IsLiteral
        {
            get { return false; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            int count = _items.Count;
            string concatOperator = " + ";
            if (db_target == DbTarget.MySql)
            {
                sql.Append("CONCAT(");
                concatOperator = ",";
            }
            for (int x = 0; x < count; x++)
            {
                if (!_items[x].IsLiteral)
                    sql.Append("(");
                _items[x].GetSql(db_target, ref sql);
                if (!_items[x].IsLiteral)
                    sql.Append(")");
                if (x + 1 < count)
                {
                    
                    if (Separator != null)
                    {
                        sql.Append(concatOperator);
                        Separator.GetSql(db_target, ref sql);
                    }
                    sql.Append(concatOperator);
                }
            }
            if (db_target == DbTarget.MySql)
            {
                sql.Append(")");
            }

        }
    }
}
