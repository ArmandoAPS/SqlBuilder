using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class SqlNameValuePair
    {
        #region Constructors
        public SqlNameValuePair(string name, ISqlExpression value)
        {
            _Name = name;
            _Value = value;
        }

        public SqlNameValuePair(string name, string value)
            : this(name, new SqlString(value))
        {
        }

        public SqlNameValuePair(string name, DateTime value)
            : this(name, new SqlDateTime(value))
        {
           
        }

        #endregion

        #region Properties

        protected string _Name;
        protected ISqlExpression _Value;
        

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }


        public ISqlExpression Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        #endregion
    }
}
