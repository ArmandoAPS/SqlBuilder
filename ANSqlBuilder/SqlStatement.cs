using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;


namespace ANSqlBuilder
{
    public abstract class SqlStatement : ISqlExpression
    {
        protected DbTarget _DbTarget;
        protected string _ConnectionStringName;
        protected List<SqlParameter> _parameters;

        protected SqlStatement()
        {
            ConnectionStringName = System.Configuration.ConfigurationManager.AppSettings["DBCSN"] ?? "SqlServer";
            _parameters = new List<SqlParameter>();
        }

        protected SqlStatement(string connection_string_name)
        {
            ConnectionStringName = connection_string_name;
            _parameters = new List<SqlParameter>();
        }



        #region Parameters

        public void SetParamValue(string name, object value)
        {
            SqlParameter prm = _parameters.FirstOrDefault(x => x.Name == name);
            if (prm == null)
                throw new Exception(String.Format("The parameter {0} is not defined.", name));

            prm.Value = value;
        }
        
        public SqlParameter CreateParameter(string name)
        {
            return CreateParameter(name, DbColumnType.String, null);
        }

        public SqlParameter CreateParameter(string name, DbColumnType data_type)
        {
            return CreateParameter(name, data_type, null);
        }

        public SqlParameter CreateParameter(string name, Type system_type)
        {
            return CreateParameter(name, Utils.GetDbColumnType(system_type));
        }

        public SqlParameter CreateParameter(string name, Type system_type, object value)
        {
            return CreateParameter(name, Utils.GetDbColumnType(system_type), value);
        }

        public SqlParameter CreateParameter(string name, DbColumnType data_type, object value)
        {
            if (_parameters == null)
                _parameters = new List<SqlParameter>();

            SqlParameter prm = _parameters.FirstOrDefault(x => x.Name == name);
            if(prm == null)
            {
                prm = new SqlParameter(name, data_type, value);
                _parameters.Add(prm);
            }
            return prm;
        }
        #endregion


        public DbTarget DbTarget { get { return _DbTarget; } set { _DbTarget = value; } }

        public string ConnectionStringName
        {
            get
            {
                if (_ConnectionStringName == null)
                {
                    _ConnectionStringName = System.Configuration.ConfigurationManager.AppSettings["DBCSN"] ??
                                            "SqlServer";

                    string providerName = _ConnectionStringName.ToUpper();
                    if (providerName == "MYSQL")
                        _DbTarget = DbTarget.MySql;
                    else if (providerName == "SQLITE")
                        _DbTarget = DbTarget.SqlLite;
                    else
                        _DbTarget = DbTarget.SqlServer;
                }

                return _ConnectionStringName;
            }
            set
            {
                _ConnectionStringName = value;

                var providerName = _ConnectionStringName.ToUpper();
                if (providerName == "MYSQL")
                    _DbTarget = DbTarget.MySql;
                else if (providerName == "SQLITE")
                    _DbTarget = DbTarget.SqlLite;
                else
                    _DbTarget = DbTarget.SqlServer;
            }
        }

        public bool IsLiteral
        {
            get { return false; }
        }

        public string Sql
        {
            get
            {
                var sql = new StringBuilder();
                GetSql(_DbTarget,ref sql);
                return sql.ToString();
            }
        }

        public virtual void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
        }


    }
}
