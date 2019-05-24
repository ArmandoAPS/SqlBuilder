using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;


namespace ANSqlBuilder
{
    public class UpdateStatement : NonQueryStatement
    {
        protected string _TableName;
        protected List<SqlNameValuePair> _Columns;
        protected ConditionClause _Where;

        public UpdateStatement(string table_name)
        {
            _TableName = table_name;
        }

        #region Properties

        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                _TableName = value;
            }
        }

        public List<SqlNameValuePair> Columns
        {
            get
            {
                if (_Columns == null)
                    _Columns = new List<SqlNameValuePair>();
                return _Columns;
            }
            set
            {
                _Columns = value;
            }
        }



        public override void GetSql(DbTarget db_target,ref StringBuilder sql)
        {
            sql.Append(" UPDATE ");
            sql.Append(Utils.FormatName(TableName, db_target));
            sql.Append(" SET ");
            if (_Columns != null && _Columns.Count > 0)
            {
                int colCount = _Columns.Count;
                for (int x = 0; x < colCount; x++)
                {
                    sql.Append(Utils.FormatName(_Columns[x].Name, db_target));
                    sql.Append(" = ");
                    _Columns[x].Value.GetSql(db_target,ref sql);
                    if (x + 1 < colCount)
                        sql.Append(",");
                }
            }
            sql.Append(" ");
            if (_Where != null && _Where.Count > 0)
               _Where.GetSql(db_target,ref sql);
        }
        #endregion Properties

        #region Methods
        public UpdateStatement Where(ICondition condition)
        {
            if (_Where == null)
                _Where = new ConditionClause("WHERE");
            _Where.And(condition);
            return this;
        }

        

        public UpdateStatement Column(string name, string value)
        {
            Columns.Add(new SqlNameValuePair(name, value));
            return this;
        }

        public UpdateStatement Column(string name, ISqlExpression value)
        {
            Columns.Add(new SqlNameValuePair(name, value));
            return this;
        }

        public UpdateStatement Column(string name, DateTime value)
        {
            Columns.Add(new SqlNameValuePair(name, value));
            return this;
        }

        public UpdateStatement WhereNot(ICondition condition)
        {
            if (condition == null)
                return this;

            condition.Negation = true;
            return Where(condition);

        }

        /* WhereIsNull */
        public UpdateStatement WhereIsNull(string a)
        {
            return Where(Condition.IsNull(new SqlColumnName(a)));
        }

        public UpdateStatement WhereIsNull(ISqlExpression a)
        {
            return Where(Condition.IsNull(a));
        }

        public UpdateStatement WhereNotIsNull(string a)
        {
            return WhereNot(Condition.IsNull(new SqlColumnName(a)));
        }

        public UpdateStatement WhereNotIsNull(ISqlExpression a)
        {
            return WhereNot(Condition.IsNull(a));
        }

        /* WhereIsEqual */
        public UpdateStatement WhereIsEqual(ISqlExpression a, ISqlExpression b)
        {
            return Where(Condition.IsEqual(a, b));
        }

        public UpdateStatement WhereIsEqual(ISqlExpression a, string b)
        {
            return Where(Condition.IsEqual(a, b));
        }

        public UpdateStatement WhereIsEqual(string a, ISqlExpression b)
        {
            return Where(Condition.IsEqual(a, b));
        }

        public UpdateStatement WhereIsEqual(string a, string b)
        {
            return Where(Condition.IsEqual(a, b));
        }

        /* WhereIsNotEqual */
        public UpdateStatement WhereIsNotEqual(ISqlExpression a, ISqlExpression b)
        {
            return WhereNot(Condition.IsEqual(a, b));
        }

        public UpdateStatement WhereIsNotEqual(ISqlExpression a, string b)
        {
            return WhereNot(Condition.IsEqual(a, b));
        }

        public UpdateStatement WhereIsNotEqual(string a, ISqlExpression b)
        {
            return WhereNot(Condition.IsEqual(a, b));
        }

        public UpdateStatement WhereIsNotEqual(string a, string b)
        {
            return WhereNot(Condition.IsEqual(a, b));
        }

        /* WhereIsIn */
        public UpdateStatement WhereIsIn(ISqlExpression a, ISqlExpression b)
        {
            return Where(Condition.IsIn(a, b));
        }


        public UpdateStatement WhereIsIn(string a, ISqlExpression b)
        {
            return Where(Condition.IsIn(a, b));
        }


        /* WhereNotIsIn */
        public UpdateStatement WhereNotIsIn(ISqlExpression a, ISqlExpression b)
        {
            return WhereNot(Condition.IsIn(a, b));
        }

        public UpdateStatement WhereNotIsIn(string a, ISqlExpression b)
        {
            return WhereNot(Condition.IsIn(a, b));
        }


        public UpdateStatement WhereExists(ISqlExpression expression)
        {
            return Where(new ExistsCondition(expression));
        }

        public UpdateStatement WhereNotExists(ISqlExpression expression)
        {
            return WhereNot(new ExistsCondition(expression));
        }



        #endregion
    }
}
