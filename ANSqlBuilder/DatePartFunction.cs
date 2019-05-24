using System;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class DatePartFunction : ISqlExpression
    {
        protected DatePart _DatePart;
        protected ISqlExpression _Date;

        public DatePartFunction(DatePart date_part, DateTime date)
        {
            _DatePart = date_part;
            _Date = new SqlDateTime(date);
        }

        public DatePartFunction(DatePart date_part, ISqlExpression date)
        {
            _DatePart = date_part;
            _Date = date;
        }

        public DatePartFunction(DatePart date_part, string date)
        {
            _DatePart = date_part;
            _Date = new SqlLiteral(date);
        }
        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            if (db_target == DbTarget.SqlServer)
            {
                if(_DatePart == DatePart.Date)
                {
                    sql.Append("CONVERT(VARCHAR(10),");
                    if (!_Date.IsLiteral)
                        sql.Append("('");
                    _Date.GetSql(db_target, ref sql);
                    if (!_Date.IsLiteral)
                        sql.Append("')");
                    sql.Append(",126)");
                }
                else if (_DatePart == DatePart.Time)
                {
                    sql.Append("CONVERT(VARCHAR(8),");
                    if (!_Date.IsLiteral)
                        sql.Append("('");
                    _Date.GetSql(db_target, ref sql);
                    if (!_Date.IsLiteral)
                        sql.Append("')");
                    sql.Append(",108)");
                }
                else 
                {
                    sql.Append("DATEPART(");
                    sql.Append((new string[] {"dd","dw","dy","hh","mi","mm","ss","ww","yyyy"})[(int)_DatePart]);
                    sql.Append(",");
                    if (!_Date.IsLiteral)
                        sql.Append("('");
                    _Date.GetSql(db_target, ref sql);
                    if (!_Date.IsLiteral)
                        sql.Append("')");
                    sql.Append(")");
                }
            }
            else if (db_target == DbTarget.MySql)
            {
                sql.Append("DATE_FORMAT('");
                sql.Append((new string[] { "%d", "%w", "%j", "%H", "%i", "%m", "%S", "%U", "%Y", "%Y-%m-%d", "%H:%i:%S" })[(int)_DatePart]);
                sql.Append("',");
                if (!_Date.IsLiteral)
                    sql.Append("(");
                _Date.GetSql(db_target, ref sql);
                if (!_Date.IsLiteral)
                    sql.Append(")");
                sql.Append(")");
            }
            else if (db_target == DbTarget.SqlLite)
            {
                sql.Append("DATETIME(");
                if (!_Date.IsLiteral)
                    sql.Append("('");
                _Date.GetSql(db_target, ref sql);
                sql.Append(",'");
                if (!_Date.IsLiteral)
                    sql.Append("')");
                sql.Append((new string[] { "%d", "%w", "%j", "%H", "%M", "%m", "%S", "%W", "%Y", "%Y-%m-%d", "%H:%M:%S" })[(int)_DatePart]);
                sql.Append("')");
            }
        }
    }
}
