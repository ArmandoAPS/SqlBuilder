using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class FromClause : ISqlExpression
    {
        protected ISqlExpression _TableSource;
        protected string _Alias;
        protected List<JoinClause> _Joins;

        #region Constructors
        public FromClause()
        {
        }

        public FromClause(ISqlExpression table_source)
        {
            _TableSource = table_source;
        }

        public FromClause(string table_name)
        {
            _TableSource = new SqlLiteral(table_name);
        }
        #endregion Constructor

        #region Properties
        public ISqlExpression TableSource
        {
            get { return _TableSource; }
            set { _TableSource = value; }
        }


        public string Alias
        {
            get { return _Alias; }
            set { _Alias = value; }
        }

        public List<JoinClause> Joins
        {
            get
            {
                if (_Joins == null)
                    _Joins = new List<JoinClause>();
                return _Joins;
            }
            set
            {
                _Joins = value;
            }
        }


        public bool IsLiteral
        {
            get { return false; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
                sql.Append(" FROM ");
                if (!TableSource.IsLiteral)
                    sql.Append("(");
                TableSource.GetSql(db_target, ref sql);
                if (!TableSource.IsLiteral)
                    sql.Append(")");
                sql.Append(" ");
                if (Alias != null && Alias.Trim() != "")
                    sql.Append(Utils.FormatName(Alias,db_target));

                sql.Append(" ");
                foreach (JoinClause join in Joins)
                {
                    join.GetSql(db_target,ref sql);
                    sql.Append(" ");
                }

        }
        #endregion Properties

        #region Methods
        public FromClause SetTableSource(string table_name,string alias)
        {
            TableSource = new SqlLiteral(table_name);
            Alias = alias;
            return this;
        }

        public FromClause SetTableSource(ISqlExpression table_source, string alias)
        {
            TableSource = table_source;
            Alias = alias;
            return this;
        }        

        public FromClause InnerJoin(ISqlExpression table_source, string alias, ICondition search_condition)
        {
            Joins.Add(new JoinClause(table_source,alias,search_condition,JoinType.Inner));
            return this;
        }

        public FromClause InnerJoin(string table_name, ICondition search_condition)
        {
            Joins.Add(new JoinClause(new SqlLiteral(table_name), table_name, search_condition, JoinType.Inner));
            return this;
        }

        public FromClause InnerJoin(string table_source, string alias, ICondition search_condition)
        {
            Joins.Add(new JoinClause(new SqlLiteral(table_source), alias, search_condition, JoinType.Inner));
            return this;
        }

        public FromClause LeftJoin(ISqlExpression table_source, string alias, ICondition search_condition)
        {
            Joins.Add(new JoinClause(table_source, alias, search_condition, JoinType.Left));
            return this;
        }

        public FromClause LeftJoin(string table_name, ICondition search_condition)
        {
            Joins.Add(new JoinClause(new SqlLiteral(table_name), table_name, search_condition, JoinType.Left));
            return this;
        }

        public FromClause LeftJoin(string table_source, string alias, ICondition search_condition)
        {
            Joins.Add(new JoinClause(new SqlLiteral(table_source), alias, search_condition, JoinType.Left));
            return this;
        }

        public FromClause RightJoin(ISqlExpression table_source, string alias, ICondition search_condition)
        {
            Joins.Add(new JoinClause(table_source, alias, search_condition, JoinType.Right));
            return this;
        }

        public FromClause RightJoin(string table_name, ICondition search_condition)
        {
            Joins.Add(new JoinClause(new SqlLiteral(table_name), table_name, search_condition, JoinType.Right));
            return this;
        }

        public FromClause RightJoin(string table_source, string alias, ICondition search_condition)
        {
            Joins.Add(new JoinClause(new SqlLiteral(table_source), alias, search_condition, JoinType.Right));
            return this;
        }
        #endregion

        #region Inner Classes
        public class JoinClause
        {
            protected ISqlExpression _TableSource;
            protected string _Alias;
            protected ICondition _Condition;
            protected JoinType _Type;


            public JoinClause(ISqlExpression table_source, string alias, ICondition condition, JoinType join_type)
            {
                TableSource = table_source;
                Alias = alias;
                Condition = condition;
                Type = join_type;
            }

            public ISqlExpression TableSource
            {
                get { return _TableSource; }
                set { _TableSource = value; }
            }

            public string Alias
            {
                get { return _Alias; }
                set { _Alias = value; }
            }

            public ICondition Condition
            {
                get { return _Condition; }
                set { _Condition = value; }
            }

            public JoinType Type
            {
                get { return _Type; }
                set { _Type = value; }
            }


            public bool IsLiteral
            {
                get { return false; }
            }

            public void GetSql(DbTarget db_target,ref StringBuilder sql)
            {
                switch (this.Type)
                {
                    case JoinType.Inner:
                        sql.Append("INNER JOIN ");
                        break;

                    case JoinType.Left:
                        sql.Append("LEFT JOIN ");
                        break;

                    case JoinType.Right:
                        sql.Append("RIGHT JOIN ");
                        break;
                }
                if (!TableSource.IsLiteral)
                    sql.Append("(");

                TableSource.GetSql(db_target, ref sql);

                if (!TableSource.IsLiteral)
                    sql.Append(")");
                if (Alias != null && Alias.Trim() != "")
                {
                    sql.Append(" ");
                    sql.Append(Utils.FormatName(Alias, db_target));
                    sql.Append(" ");
                }
                sql.Append(" ON (");
                Condition.GetSql(db_target,ref sql);
                sql.Append(")");
            }

        }
        #endregion

    }
        
}
