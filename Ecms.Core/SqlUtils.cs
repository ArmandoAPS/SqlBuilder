using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ANCommon.Sql;
using ANMappings;
using ANSqlBuilder;
using System.Globalization;

namespace Ecms.Core
{
    public class SqlUtils
    {


        public static string GetCreateStatements(IEnumerable<IMapping> mappings)
        {
            var rel = mappings.Where(x => x.Associations.Count > 0);
            return null;
        }

        public static string GetCreateStatement(IMapping mapping)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[{0}]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1) BEGIN DROP TABLE [dbo].[{0}] END\n", mapping.TableName);
            sb.AppendFormat("CREATE TABLE [dbo].[{0}] (\n", mapping.TableName);

            int r = 0;
            var pkeys = new List<IColumnMapping>();

            foreach (IColumnMapping map in mapping.Columns)
            {

                var dbType = GetDbType(ReflectionUtils.GetMemberUnderlyingType(map.Property));

                if (r > 0)
                    sb.Append(",\n");

                if (dbType.IndexOf("char") >= 0)
                {
                    int len = map.MaxLength;

                    sb.AppendFormat("\t[{0}] [{1}]({2})", map.ColumnName, dbType, len == 0 ? "MAX": len.ToString());
                }

                else if (dbType == "decimal")
                    sb.AppendFormat("\t[{0}] [{1}]({2},{3})", map.ColumnName, dbType, map.Presicion, map.Scale);

                else
                    sb.AppendFormat("\t[{0}] [{1}]", map.ColumnName, dbType);

                if (map.IsDbGenerated)
                    sb.Append(" IDENTITY(1,1)");

                if (!map.IsNullable)
                    sb.Append(" NOT NULL");

                if (map.IsPrimaryKey)
                    pkeys.Add(map);

                r++;
            }

            if (pkeys.Count > 0)
            {
                sb.AppendFormat(", CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED (\n", mapping.TableName);
                int c = 0;
                foreach (IColumnMapping pkey in pkeys)
                {
                    if (c > 0)
                        sb.Append(",\n");
                    sb.AppendFormat("[{0}] ASC", pkey.ColumnName);
                    c++;
                }
                sb.Append("\n) ON [PRIMARY]\n");
            }

            sb.Append("\n) ON [PRIMARY]\n");
            return sb.ToString();
        }

        public static string GetDbType(Type property_type)
        {
            if (property_type.IsGenericType && property_type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (property_type.FullName.IndexOf("System.Int") > 0)
                    return "int";
            }

            if (property_type == typeof(byte))
                return "tinyint";

            if (property_type == typeof(int))
                return "int";

            if (property_type == typeof(Int64))
                return "bigint";

            if (property_type == typeof(decimal))
                return "decimal";

            if (property_type == typeof(DateTime) || property_type == typeof(DateTime?))
                return "datetime";

            if (property_type == typeof(bool))
                return "bit";

            if (property_type == typeof(Guid))
                return "uniqueidentifier";

            return "varchar";
        }

        public static Guid ToGuid(long value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        public static InsertStatement CreateInsertStatement(IMapping mapping, bool identityInsert = false)
        {
            InsertStatement statement = new InsertStatement(mapping.TableName);
            
            statement.IdentityColumnName = String.Empty;

            foreach (var item in mapping.Columns)
            {
                if (item.IsDbGenerated && !identityInsert)
                {
                    statement.IdentityColumnName = item.Name;
                }
                else
                {
                    var pinfo = (PropertyInfo)item.Property;
                    SqlParameter prm;
                    if (pinfo.PropertyType.IsGenericType && pinfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        prm = statement.CreateParameter(item.Name, pinfo.PropertyType.GetGenericArguments()[0]);
                    else
                        prm = statement.CreateParameter(item.Name, pinfo.PropertyType);

                    if (item.IsNullable && item.DefaultValue != null && prm.Value == null)
                        prm.Value = item.DefaultValue;

                    statement.Column(item.ColumnName, prm);
                }
            }

            return statement;
        }

        public static UpdateStatement CreateUpdateStatement(IMapping mapping)
        {
            UpdateStatement statement = new UpdateStatement(mapping.TableName);

            foreach (var item in mapping.Columns)
            {
                var pinfo = (PropertyInfo)item.Property;
                SqlParameter prm;
                if (!item.IsDbGenerated && !item.IsPrimaryKey)
                {
                    if (pinfo.PropertyType.IsGenericType && pinfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        prm = statement.CreateParameter(item.Name, pinfo.PropertyType.GetGenericArguments()[0]);
                    else
                        prm = statement.CreateParameter(item.Name, pinfo.PropertyType);
                    statement.Column(item.ColumnName, prm);
                }
                else
                {
                    prm = statement.CreateParameter(item.Name, pinfo.PropertyType);
                    statement.Where(new Condition(item.ColumnName, Comparisons.Equals, prm));
                }
            }

            return statement;
        }


        public static DeleteStatement CreateDeleteStatement(IMapping mapping)
        {
            return CreateDeleteStatement(mapping, false);
        }

        public static DeleteStatement CreateDeleteStatement(IMapping mapping, bool include_all_fields)
        {
            DeleteStatement statement = new DeleteStatement(mapping.TableName);

            foreach (var item in mapping.Columns)
            {
                if (include_all_fields || (include_all_fields && item.IsPrimaryKey))
                {
                    var pinfo = (PropertyInfo)item.Property;
                    SqlParameter prm = statement.CreateParameter(item.Name, pinfo.PropertyType);
                    statement.Where(new Condition(item.ColumnName, Comparisons.Equals, prm));
                }
            }

            return statement;
        }

        public static SelectStatement CreateSelectStatement(IMapping mapping)
        {
            var colNames = new List<string>();

            var baseSelect = new SelectStatement();
            baseSelect.From(mapping.TableName, mapping.EntityName);

            for (var x = 0; x < mapping.Columns.Count; x++)
            {
                var col = mapping.Columns[x];
                if (col.Name.IndexOf(".") < 0)
                    baseSelect.Column(new SqlColumnName(mapping.EntityName, col.ColumnName), col.Name);
            }

            var _associations = mapping.Associations.Where(x => x.GetType().Name == "MakeReferenceToMapping`2");

            foreach (var item in _associations)
            {
                var omapping = MappingFactory.GetMapping(item.OtherMappingType);

                // extra columns for criteria support
                for (var x = 0; x < omapping.Columns.Count; x++)
                {
                    var col = omapping.Columns[x];

                    string colName = item.Name + "." + col.Name;
                    if (colNames.Contains(colName))
                        continue;
                    else
                        colNames.Add(colName);

                    baseSelect.Column(new SqlColumnName(item.Name, col.ColumnName), item.Name + "." + col.Name);
                }

                var keyName = item.ThisKeys[0];
                var otherKeyName = item.OtherKeys[0];
                var tcol = mapping.Columns.Where(x => x.Name == keyName).Single();
                var ocol = omapping.Columns.Where(x => x.Name == otherKeyName).Single();
                if (tcol.IsNullable)
                    baseSelect.LeftJoin(omapping.TableName, item.Name, Condition.IsEqual(new SqlColumnName(mapping.EntityName, tcol.ColumnName), new SqlColumnName(item.Name, ocol.ColumnName)));
                else
                    baseSelect.InnerJoin(omapping.TableName, item.Name, Condition.IsEqual(new SqlColumnName(mapping.EntityName, tcol.ColumnName), new SqlColumnName(item.Name, ocol.ColumnName)));


            }

            colNames = new List<string>();
            var select = new SelectStatement();

            select.From(baseSelect, mapping.EntityName);

            for (var x = 0; x < mapping.Columns.Count; x++)
            {
                string colName = mapping.Columns[x].Name;
                colNames.Add(colName);
                select.Column(new SqlColumnName(mapping.Columns[x].Name), colName);
            }

            foreach (var belong in _associations)
            {
                var omapping = MappingFactory.GetMapping(belong.OtherMappingType);
                for (var x = 0; x < omapping.Columns.Count; x++)
                {
                    string colName = belong.Name + "." + omapping.Columns[x].Name;
                    if (colNames.Contains(colName))
                        continue;
                    else
                        colNames.Add(colName);
                    select.Column(new SqlColumnName(colName), colName);
                }
            }

            return select;
        }
    }
}
