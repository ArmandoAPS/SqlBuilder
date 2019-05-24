using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public abstract class NonQueryStatement: SqlStatement
    {
        //public string IdentityColumnName { get; set; }

        public NonQueryStatement():base()
        {
            
        }

        public NonQueryStatement(string connection_string_name):base(connection_string_name)
        {
            _ConnectionStringName = connection_string_name;
        }

        public virtual int Execute()
        {
            using (DbHelper helper = new DbHelper(ConnectionStringName))
            {
                StringBuilder sql = new StringBuilder();
                GetSql(DbTarget, ref sql);
                return helper.ExecuteNonQuery(sql.ToString());
            }
        }

    }
}
