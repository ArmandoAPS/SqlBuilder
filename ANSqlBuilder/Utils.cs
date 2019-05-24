using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    internal class Utils
    {

        public static string FormatName(string name, DbTarget db_target)
        {
            string format = "{0}";
                if (db_target == DbTarget.SqlServer)
                    format = "[{0}]";
                else if (db_target == DbTarget.MySql)
                    format = "`{0}`";
            
         
            return String.Format(format, name);
        }

        internal static DbColumnType GetDbColumnType(Type type)
        {
            DbColumnType dataType;
            switch (type.FullName)
            {
                case "System.Byte":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                    dataType = DbColumnType.Number;
                    break;

                case "System.DateTime":
                    dataType = DbColumnType.Date;
                    break;

                case "System.Boolean":
                    dataType = DbColumnType.Boolean;
                    break;

                case "System.VarChar":
                case "System.Char":
                    dataType = DbColumnType.String;
                    break;

                default:
                    dataType = DbColumnType.String;
                    break;
            }
            return dataType;
        }

    }
}
