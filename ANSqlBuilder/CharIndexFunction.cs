using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;


namespace ANSqlBuilder
{
    public class CharIndexFunction : ISqlExpression
    {
        private ISqlExpression _expressionToFind;
        private ISqlExpression _expressionToSearch;
        private int _startLocation;


        public CharIndexFunction(string expressionToFind, string expressionToSearch, int startLocation)
        {
            _expressionToFind = new SqlString(expressionToFind);
            _expressionToSearch = new SqlString(expressionToSearch);
            _startLocation = startLocation;
        }

        public CharIndexFunction(string expressionToFind, ISqlExpression expressionToSearch, int startLocation)
        {
            _expressionToFind = new SqlString(expressionToFind);
            _expressionToSearch = expressionToSearch;
            _startLocation = startLocation;
        }

        public CharIndexFunction(ISqlExpression expressionToFind, string expressionToSearch, int startLocation)
        {
            _expressionToFind = expressionToFind;
            _expressionToSearch = new SqlString(expressionToSearch);
            _startLocation = startLocation;
        }

        public CharIndexFunction(ISqlExpression expressionToFind, ISqlExpression expressionToSearch, int startLocation)
        {
            _expressionToFind = expressionToFind;
            _expressionToSearch = expressionToSearch;
            _startLocation = startLocation;
        }


        public bool IsLiteral
        {
            get { return false; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            if (db_target == DbTarget.SqlServer)
            {
                sql.Append("CHARINDEX(");
                if (!_expressionToFind.IsLiteral)
                    sql.Append("(");
                _expressionToFind.GetSql(db_target, ref sql);
                if (!_expressionToFind.IsLiteral)
                    sql.Append(")");
                sql.Append(",");
                if (!_expressionToSearch.IsLiteral)
                    sql.Append("(");
                _expressionToSearch.GetSql(db_target, ref sql);
                if (!_expressionToSearch.IsLiteral)
                    sql.Append(")");
            }
            else
            {
                sql.Append("LOCATE(");
                if (!_expressionToSearch.IsLiteral)
                    sql.Append("(");
                _expressionToSearch.GetSql(db_target, ref sql);
                if (!_expressionToSearch.IsLiteral)
                    sql.Append(")");
                sql.Append(",");
                if (!_expressionToFind.IsLiteral)
                    sql.Append("(");
                _expressionToFind.GetSql(db_target, ref sql);
                if (!_expressionToFind.IsLiteral)
                    sql.Append(")");
            }

            sql.Append(",");
            sql.Append(_startLocation.ToString());
            sql.Append(")");
        }
    }
}
