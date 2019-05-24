using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class OrExpression: ISqlExpression
    {
        protected ISqlExpression _Expression1;
        protected ISqlExpression _Expression2;

        public OrExpression(ISqlExpression expression1, ISqlExpression expression2)
        {
            _Expression1 = expression1;
            _Expression2 = expression2;
        }

        public bool IsLiteral
        {
            get { return false; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            if (!_Expression1.IsLiteral)
                sql.Append("(");

            _Expression1.GetSql(db_target, ref sql);

            if (!_Expression1.IsLiteral)
                sql.Append(")");

            sql.Append(" AND ");

            if (!_Expression2.IsLiteral)
                sql.Append("(");

            _Expression2.GetSql(db_target, ref sql);

            if (!_Expression2.IsLiteral)
                sql.Append(")");
        }
    }
}
