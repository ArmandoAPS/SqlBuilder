using System;
using System.Collections.Generic;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class SqlNumber: ISqlExpression
    {
        protected object _Value;
        protected byte _DecimalPlaces = 0; 


        public static SqlNumber CERO
        {
            get
            {
                return new SqlNumber(0);
            }
        }


        public static SqlNumber ONE
        {
            get
            {
                return new SqlNumber(1);
            }
        }
        public static SqlNumber TWO
        {
            get
            {
                return new SqlNumber(2);
            }
        }
        public static SqlNumber THREE
        {
            get
            {
                return new SqlNumber(3);
            }
        }

        public SqlNumber(Byte value)
        {
            _Value = value;
        }

        public SqlNumber(Int16 value)
        {
            _Value = value;
        }

        public SqlNumber(Int32 value)
        {
            _Value = value;
        }

        public SqlNumber(Int64 value)
        {
            _Value = value;
        }

        public SqlNumber(Decimal value)
        {
            _Value = value;
        }

        public SqlNumber(Decimal value, byte decimal_places)
        {
            _Value = value;
            _DecimalPlaces = decimal_places;
        }

        public SqlNumber(Double value)
        {
            _Value = value;
        }

        public SqlNumber(float value)
        {
            _Value = value;
        }

        public SqlNumber(string value)
        {
            _Value = int.Parse(value);
        }

        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target,ref StringBuilder sql)
        {
            sql.Append(_Value.ToString());
        }
    }
}
