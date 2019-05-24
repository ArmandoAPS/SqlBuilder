using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class DaysDiffFunction : ISqlExpression
    {
        protected ISqlExpression _StartDate;
        protected ISqlExpression _EndDate;

        public DaysDiffFunction(ISqlExpression start_date, ISqlExpression end_date)
        {
            _StartDate = start_date;
            _EndDate = end_date;
        }

        public DaysDiffFunction(ISqlExpression start_date, string end_date)
        {
            _StartDate = start_date;
            _EndDate = new SqlName(end_date);
        }

        public DaysDiffFunction(string start_date, ISqlExpression end_date)
        {
            _StartDate = new SqlName(start_date);
            _EndDate = end_date;
        }

        public DaysDiffFunction(string start_date, string end_date)
        {
            _StartDate = new SqlName(start_date);
            _EndDate = new SqlName(end_date);
        }

        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            if (db_target == DbTarget.SqlServer)
            {
                sql.Append("DATEDIFF(day,");
                if (!_StartDate.IsLiteral)
                    sql.Append("(");
                _StartDate.GetSql(db_target, ref sql);
                if (!_StartDate.IsLiteral)
                    sql.Append(")");
                sql.Append(",");
                if (!_EndDate.IsLiteral)
                    sql.Append("(");
                _EndDate.GetSql(db_target, ref sql);
                if (!_EndDate.IsLiteral)
                    sql.Append(")");
                sql.Append(")");
            }
            else if (db_target == DbTarget.MySql)
            {
                sql.Append("DATEDIFF(");
                if (!_StartDate.IsLiteral)
                    sql.Append("(");
                _StartDate.GetSql(db_target, ref sql);
                if (!_StartDate.IsLiteral)
                    sql.Append(")");
                sql.Append(",");
                if (!_EndDate.IsLiteral)
                    sql.Append("(");
                _EndDate.GetSql(db_target, ref sql);
                if (!_EndDate.IsLiteral)
                    sql.Append(")");
                sql.Append(")");
            }
            else if (db_target == DbTarget.SqlLite)
            {
                sql.Append("julianday(");

                if (!_EndDate.IsLiteral)
                    sql.Append("(");
                _EndDate.GetSql(db_target, ref sql);
                if (!_EndDate.IsLiteral)
                    sql.Append(")");

                sql.Append(") - julianday(");

                if (!_StartDate.IsLiteral)
                    sql.Append("(");
                _StartDate.GetSql(db_target, ref sql);
                if (!_StartDate.IsLiteral)
                    sql.Append(")");

                sql.Append(")");
            }



        }
    }
}
