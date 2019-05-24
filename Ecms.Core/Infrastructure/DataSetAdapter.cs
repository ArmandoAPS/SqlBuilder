using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using ANCommon.Sql;
using ANCommon.DataAccess;
using ANMappings;
using ANSqlBuilder;
using System.IO;
using Ecms.Core.Domain.Model;


namespace Ecms.Core.Infrastructure
{
    public class DataSetAdapter<TEntity, TKey, TMappingInfo> : DataSet, IDataSetAdapter
        where TEntity : IEntity<TKey>, new()
        where TMappingInfo : Mapping<TEntity>, new()
    {
        protected string _connectionStringName;
        protected Dictionary<string, Condition> DetailsFilters;
        public bool IgnoreMemoFields { get; set; }


        public DataSetAdapter()
            : base()
        {
            _connectionStringName = System.Configuration.ConfigurationManager.AppSettings["DBCSN"] ?? "SqlServer";
            AddMasterDataTable();
            DetailsFilters = new Dictionary<string, Condition>();
        }

        public DataSetAdapter(string connection_string_name)
            : base()
        {
            _connectionStringName = connection_string_name;
            AddMasterDataTable();
            DetailsFilters = new Dictionary<string, Condition>();
        }

        public Dictionary<string, string> GetItemsFromRelatedTable(string table_name)
        {
            var reader = GetRelatedTableReader(table_name);
            if (reader == null)
                return null;

            var items = new Dictionary<string, string>();
            while (reader.Read())
                items.Add(reader.GetValue(0).ToString(), reader.GetValue(1).ToString());

            return items;

        }

        public string GetItemsStringFromRelatedTable(string table_name)
        {
            var reader = GetRelatedTableReader(table_name);
            if (reader == null)
                return null;

            var items = new StringBuilder();
            int c = 0;
            while (reader.Read())
            {
                if (c > 0)
                    items.Append(";");
                items.Append(reader.GetValue(0).ToString());
                items.Append(",");
                items.Append(reader.GetValue(1).ToString().Replace(",", " ").Replace(";", " "));
                c++;
            }

            return items.ToString();
        }

        private IDataReader GetRelatedTableReader(string table_name)
        {
            var metadata = (TMappingInfo)MappingFactory.GetMapping(typeof(TMappingInfo));
            var item = metadata.Associations.Where(x => x.Name == table_name).FirstOrDefault();

            if (item == null)
                return null;

            var omapping = MappingFactory.GetMapping(item.OtherMappingType);

            var qry = (new SelectStatement())
                .Column(omapping.Columns.Where(x => x.Name == item.OtherKeys[0]).Single().ColumnName)
                .From(omapping.TableName);

            var lblCount = item.OtherLabels.Count;

            if (lblCount == 0)
                return null;
            else
            {
                if (lblCount == 1)
                    qry.Column(omapping.Columns.Where(x => x.Name == item.OtherLabels[0]).Single().ColumnName);
                else
                {
                    var lblConcat = new ConcatFunction(" | ");
                    foreach (var lbl in item.OtherLabels)
                        lblConcat.Append(new SqlColumnName(omapping.Columns.Where(x => x.Name == lbl).Single().ColumnName));
                    qry.Column(lblConcat);
                }
            }

            return qry.ExecuteReader();
        }

        protected void AddMasterDataTable()
        {
            var metadata = (TMappingInfo)MappingFactory.GetMapping(typeof(TMappingInfo));

            // Add main data table
            AddDataTable(metadata.TableName, metadata.EntityName, metadata);
        }

        protected void AddDetailsDataTables()
        {
            var metadata = (TMappingInfo)MappingFactory.GetMapping(typeof(TMappingInfo));
            AddDetailsDataTables(metadata);
        }

        protected void AddDetailsDataTables(TMappingInfo mapping_info)
        {
            // Add details data tables

            var items = mapping_info.Associations;
            foreach (var item in items)
            {
                var mapping = MappingFactory.GetMapping(item.OtherMappingType);
                var tableName = mapping.TableName;
                var tableAlias = item.Name;
                var associationName = item.GetType().Name;

                AddDataTable(tableName, tableAlias, mapping);

                var thisColumns = new DataColumn[item.ThisKeys.Count];
                var otherColumns = new DataColumn[item.OtherKeys.Count];

                for (int z = 0; z < item.ThisKeys.Count; z++)
                    thisColumns[z] = this.Tables[0].Columns[item.ThisKeys[z]];

                for (int z = 0; z < item.OtherKeys.Count; z++)
                {
                    var key = item.OtherKeys[z];
                    var ccol = this.Tables[tableAlias].Columns[item.OtherKeys[z]];

                    otherColumns[z] = ccol;
                }

                if (associationName.Contains("Has"))
                {
                    this.Tables[tableAlias].ExtendedProperties["IsChild"] = true;
                    this.Relations.Add(new DataRelation(tableAlias, thisColumns, otherColumns));
                }


            }
        }

        private void AddDataTable(string table_name, string table_alias, IMapping mapping)
        {
            if (this.Tables.Contains(table_alias))
                return;

            var table = new DataTable(table_alias);
            var keys = new List<DataColumn>();

            this.Tables.Add(table);
            var columns = table.Columns;
            var select = (new SelectStatement()).From(table_name, "t0");
            table.ExtendedProperties["Storage"] = table_name;
            foreach (var item in mapping.Columns)
            {
                if (IgnoreMemoFields && item.IsMemo)
                    continue;

                var pinfo = (PropertyInfo)item.Property;
                DataColumn column;
                if (pinfo.PropertyType.IsGenericType && pinfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    column = columns.Add(item.Name, pinfo.PropertyType.GetGenericArguments()[0]);
                else if (pinfo.PropertyType.IsEnum)
                    column = columns.Add(item.Name, typeof(string));
                else
                    column = columns.Add(item.Name, pinfo.PropertyType);
                select.Column(new SqlColumnName("t0", item.ColumnName), item.Name);

                if (item.IsDbGenerated)
                {
                    column.AutoIncrement = true;
                    column.AutoIncrementSeed = 1;
                    column.AutoIncrementStep = 1;
                    column.Unique = true;
                }
                if (item.IsPrimaryKey)
                {
                    column.ExtendedProperties["PrimaryKey"] = true;
                    keys.Add(column);
                }
                column.AllowDBNull = item.IsNullable;
                //if (item.IsNullable && item.DefaultValue != null)
                //    column.DefaultValue = item.DefaultValue;

                if (!item.IsDbGenerated)
                {
                    if (item.DefaultFunctionValue == null)
                        column.DefaultValue = item.DefaultValue;
                    else 
                        column.DefaultValue = item.DefaultFunctionValue();
                }

                column.ExtendedProperties["Storage"] = item.ColumnName;
                if (item.IsMemo)
                    column.ExtendedProperties["Memo"] = true;

            }
            table.ExtendedProperties["SelectStatement"] = select;

            if (keys.Count > 0)
                table.PrimaryKey = keys.ToArray();


            /* Procedimiento para añadir las columnas descriptivas de las columnas foreanas */
            var items = mapping.Associations.Where(x => x.GetType().Name == "MakeReferenceToMapping`2" || x.GetType().Name == "HasOneMapping`2");
            if (items.Count() == 0)
                return;

            //Debes poner las otras columnas con el nombre fisico alias el nombre de la propiedad
            int t = 1;
            string tp = "t" + t;
            foreach (var item in items)
            {
                var omapping = MappingFactory.GetMapping(item.OtherMappingType);

                // extra columns for criteria support
                for (var x = 0; x < omapping.Columns.Count; x++)
                {
                    var col = omapping.Columns[x];
                    select.Column(new SqlColumnName(tp, col.ColumnName), item.Name + "." + col.Name);
                }

                /* build joins */

                //for (var k = 0; k < item.ThisKeys.Count; k++ )
                //{
                var keyName = item.ThisKeys[0];
                var otherKeyName = item.OtherKeys[0];
                var tcol = mapping.Columns.Where(x => x.Name == keyName).Single();
                var ocol = omapping.Columns.Where(x => x.Name == otherKeyName).Single();
                if (tcol.IsNullable || item.GetType().Name == "HasOneMapping`2")
                    select.LeftJoin(omapping.TableName, tp, Condition.IsEqual(new SqlColumnName("t0", tcol.ColumnName), new SqlColumnName(tp, ocol.ColumnName)));
                else
                    select.InnerJoin(omapping.TableName, tp, Condition.IsEqual(new SqlColumnName("t0", tcol.ColumnName), new SqlColumnName(tp, ocol.ColumnName)));
                //}
                /* build foreign row name */
                var lblCount = item.OtherLabels.Count;
                if (lblCount > 0)
                {
                    var lbl0 = item.OtherLabels[0];
                    //var colName = omapping.Columns.Where(x => x.Name == lbl0).Single().ColumnName;

                    ISqlExpression lblExpr;
                    if (lblCount == 1)
                        lblExpr = new SqlColumnName(item.Name + "." + item.OtherLabels[0]);
                    else
                    {
                        var lblConcat = new ConcatFunction(" | ");
                        foreach (var lbl in item.OtherLabels)
                            lblConcat.Append(new SqlColumnName(item.Name, lbl));
                        lblExpr = lblConcat;
                    }
                    var column = columns.Add(item.Name, typeof(string));
                    column.ExtendedProperties["SelectExpression"] = lblExpr;
                    column.ExtendedProperties["NonStorable"] = true;
                }

                // Include OtherColumns in DataTable
                foreach (string ocn in item.OtherColumns)
                {
                    var column = columns.Add(omapping.EntityName + "." + ocn, typeof(string));
                    column.ExtendedProperties["NonStorable"] = true;
                }

                t++;
                tp = "t" + t;
            }
        }

        public DataSet GetDataSet()
        {
            return (DataSet)this;
        }

        #region IDataSetAdapter Members



        protected virtual SelectStatement GetSelectCountStatement(string table_name, ANCommon.Sql.ICondition criteria)
        {
            var select = new SelectStatement
                             {
                                 ConnectionStringName = _connectionStringName
                             };
            select.Column(new SqlLiteral("COUNT(*)"));
            select.From((SelectStatement)this.Tables[table_name].ExtendedProperties["SelectStatement"], table_name);

            if (criteria != null)
                select.Where(criteria);

            return select;
        }

        protected virtual SelectStatement GetSelectStatement(string table_name, ICondition criteria, IOrderByExpression order_by)
        {
            var select = new SelectStatement() { ConnectionStringName = _connectionStringName };
            var myTable = this.Tables[table_name];
            foreach (DataColumn col in myTable.Columns)
            {
                if (IgnoreMemoFields && col.ExtendedProperties["Memo"] != null)
                    continue;

                if (col.ExtendedProperties.ContainsKey("SelectExpression"))
                    select.Column((ISqlExpression)col.ExtendedProperties["SelectExpression"], col.Caption);
                else
                    select.Column(new SqlColumnName(col.ColumnName), col.ColumnName);
            }

            select.From((SelectStatement)myTable.ExtendedProperties["SelectStatement"], table_name);

            if (criteria != null)
                select.Where(criteria);

            if (order_by != null)
                select.OrderBy(order_by);

            return select;
        }

        public int GetCount()
        {
            return GetCount(this.Tables[0].TableName, null);
        }

        public int GetCount(string table_name)
        {
            return GetCount(table_name, null);
        }

        public int GetCount(ICondition criteria)
        {
            return GetCount(this.Tables[0].TableName, criteria);
        }

        public virtual int GetCount(string table_name, ICondition criteria)
        {
            int result = 0;
            var select = GetSelectCountStatement(table_name, criteria);
            Int32.TryParse(select.ExecuteScalar().ToString(), out result);
            return result;
        }


        protected virtual void FillRowFromReader(IDataReader reader, DataRow row)
        {
            for (int x = 0; x < row.ItemArray.Length; x++)
            {
                var fieldName = reader.GetName(x);
                try
                {
                    row[fieldName] = reader.IsDBNull(x) ? Convert.DBNull : reader.GetValue(x);

                }
                catch
                {
                    row[fieldName] = Convert.DBNull;
                }
            }
        }

        public void LoadData()
        {
            LoadData(this.Tables[0].TableName);
        }

        public virtual void LoadData(string table_name)
        {
            LoadData(table_name, null, null);
        }

        public void LoadData(ICondition criteria)
        {
            LoadData(this.Tables[0].TableName, criteria, null);
        }

        public void LoadData(string table_name, ICondition criteria)
        {
            LoadData(table_name, criteria, null);
        }

        public void LoadData(ICondition criteria, IOrderByExpression order_by)
        {
            LoadData(this.Tables[0].TableName, criteria, order_by);
        }

        public virtual void LoadData(string table_name, ICondition criteria, IOrderByExpression order_by)
        {
            LoadData(table_name, GetSelectStatement(table_name, criteria, order_by).ExecuteReader());
        }

        protected virtual void LoadData(string table_name, IDataReader reader)
        {
            DataTable myTable = this.Tables[table_name];
            myTable.Clear();
            while (reader.Read())
            {
                DataRow row = myTable.NewRow();
                FillRowFromReader(reader, row);

                myTable.Rows.Add(row);
            }
            reader.Close();
            myTable.AcceptChanges();
        }

        public virtual void LoadData(ICondition criteria, IOrderByExpression order_by, int start_record, int max_records)
        {
            LoadData(this.Tables[0].TableName, criteria, order_by, start_record, max_records);
        }

        public virtual void LoadData(string table_name, ICondition criteria, IOrderByExpression order_by, int start_record, int max_records)
        {
            var myTable = this.Tables[table_name];
            var select = GetSelectStatement(table_name, criteria, order_by);

            myTable.Clear();
            IDataReader reader = select.ExecuteReader(start_record);
            for (int i = 0; i < max_records; i++)
            {
                if (reader.Read())
                {
                    var row = myTable.NewRow();
                    FillRowFromReader(reader, row);
                    myTable.Rows.Add(row);
                }
                else
                    break;
            }
            reader.Close();
            myTable.AcceptChanges();
        }

        public void LoadDataById(object id)
        {
            LoadDataById(new object[] { id }, false);
        }

        public void LoadDataById(object id, bool with_children)
        {
            LoadDataById(new object[] { id }, with_children);
        }

        public void LoadDataChildrens(object id)
        {
            LoadDataChildrens(new object[] { id });
        }

        public void LoadDataChildrens(object[] ids)
        {
            if (this.Tables.Count == 1)
                AddDetailsDataTables();

            for (var x = 1; x < this.Tables.Count; x++)
            {
                var table = Tables[x];
                if (table.ExtendedProperties["IsChild"] == null || !(bool)table.ExtendedProperties["IsChild"])
                    continue;

                var relation = Relations[table.TableName];
                if (relation == null)
                    continue;

                var condition = Condition.IsEqual(relation.ChildColumns[0].ColumnName, new SqlParameter("p0", relation.ChildColumns[0].DataType, ids[0]));
                if (ids.Length > 1)
                {
                    var criteria = new Conditions(condition);
                    for (int c = 0; c < relation.ChildColumns.Length; c++)
                        criteria.And(Condition.IsEqual(relation.ChildColumns[c].ColumnName, new SqlParameter("p" + c.ToString(), relation.ChildColumns[c].DataType, ids[c])));
                    LoadData(table.TableName, criteria);
                }
                else
                    LoadData(table.TableName, condition);
            }
        }

        public virtual void LoadDataById(object[] ids, bool with_children)
        {
            for (int t = 1; t < Tables.Count; t++)
                Tables[t].Clear();
            Tables[0].Clear();
            var filter = new Conditions();
            var p = 0;
            foreach (DataColumn column in Tables[0].Columns)
            {
                if (column.ExtendedProperties.ContainsKey("PrimaryKey"))
                {
                    filter.And(Condition.IsEqual(column.ColumnName, new SqlParameter("p" + p.ToString(), column.DataType, ids[p])));
                    p++;
                }
            }
            LoadData(filter);

            if (!with_children)
                return;

            if (this.Tables.Count == 1)
                AddDetailsDataTables();

            for (var x = 1; x < this.Tables.Count; x++)
            {
                var table = Tables[x];
                var relation = Relations[table.TableName];
                if (relation != null)
                {
                    var condition = Condition.IsEqual(relation.ChildColumns[0].ColumnName, new SqlParameter("p0", relation.ChildColumns[0].DataType, ids[0]));
                    if (ids.Length > 1)
                    {
                        var criteria = new Conditions(condition);
                        for (int c = 0; c < relation.ChildColumns.Length; c++)
                            criteria.And(Condition.IsEqual(relation.ChildColumns[c].ColumnName, new SqlParameter("p" + c.ToString(), relation.ChildColumns[c].DataType, ids[c])));
                        LoadData(table.TableName, criteria);
                    }
                    else
                        LoadData(table.TableName, condition);
                }
            }
        }

        public virtual int Insert()
        {
            return Insert(this.Tables[0].TableName);
        }

        public virtual int Insert(string table_name)
        {
            return InsertRows(table_name);
        }

        protected int InsertRows(string table_name)
        {
            DataTable myTable = this.Tables[table_name];
            DataRowCollection rows = myTable.Rows;
            int rowsCount = rows.Count;
            int rowsAffected = 0;

            if (rowsCount <= 0)
                return rowsAffected;


            var cmd = new InsertStatement(myTable.ExtendedProperties["Storage"].ToString());
            cmd.ConnectionStringName = _connectionStringName;
            var prmlist = new List<SqlParameter>();
            var colsCount = myTable.Columns.Count;

            var identcol = String.Empty;
            for (var c = 0; c < colsCount; c++)
            {
                var col = myTable.Columns[c];
                var prm = new SqlParameter(col.ColumnName, col.DataType);
                prmlist.Add(prm);
                if (col.AutoIncrement)
                    identcol = col.ColumnName;
                else if (col.ExtendedProperties["NonStorable"] == null)
                    cmd.Column(col.ExtendedProperties["Storage"].ToString(), prm);
            }

            IEnumerator rowEnum = rows.GetEnumerator();
            while (rowEnum.MoveNext())
            {
                var row = (DataRow)rowEnum.Current;
                if (row.RowState != DataRowState.Added) continue;
                for (var c = 0; c < colsCount; c++)
                {
                    var col = myTable.Columns[c];
                    var prm = prmlist[c];
                    if (row[prm.Name] == null || row[prm.Name] == DBNull.Value && col.DefaultValue != null)
                        prm.Value = col.DefaultValue;
                    else
                        prm.Value = row[prm.Name];
                }

                if (identcol != String.Empty)
                {
                    object id;
                    cmd.Execute(out id);
                    row[identcol] = id;//Int32.Parse(id.ToString());
                }
                else
                    cmd.Execute();
                row.AcceptChanges();
                rowsAffected++;
            }
            return rowsAffected;
        }

        public virtual int Update()
        {
            return Update(this.Tables[0].TableName);
        }

        public virtual int Update(string table_name)
        {
            var records = 0;
            var dsSelf = (DataSet)this;
            var tableList = GetChangedTableList();

            if (tableList.Contains(Tables[0].TableName))
                records += UpdateRows(table_name);

            if (this.Tables.Count > 1)
            {
                for (var x = 1; x < Tables.Count; x++)
                {
                    if (tableList.Contains(Tables[x].TableName))
                        records += FullUpdate(Tables[x].TableName);
                }
            }

            return records;
        }

        protected int UpdateRows(string table_name)
        {
            DataTable myTable = this.Tables[table_name];
            DataRowCollection rows = myTable.Rows;
            int rowsCount = rows.Count;
            int rowsAffected = 0;

            if (rowsCount > 0)
            {
                var cmd = new UpdateStatement(myTable.ExtendedProperties["Storage"].ToString());
                cmd.ConnectionStringName = _connectionStringName;
                var prmlist = new List<SqlParameter>();
                int colsCount = myTable.Columns.Count;

                string identcol = String.Empty;
                for (int c = 0; c < colsCount; c++)
                {
                    DataColumn col = myTable.Columns[c];
                    SqlParameter prm = new SqlParameter(col.ColumnName, col.DataType);
                    prmlist.Add(prm);
                    if (col.ExtendedProperties["PrimaryKey"] != null)
                        cmd.Where(Condition.IsEqual(col.ExtendedProperties["Storage"].ToString(), prm));
                    else if (col.ExtendedProperties["NonStorable"] == null)
                        cmd.Column(col.ExtendedProperties["Storage"].ToString(), prm);
                }

                IEnumerator rowEnum = rows.GetEnumerator();
                while (rowEnum.MoveNext())
                {
                    DataRow row = (DataRow)rowEnum.Current;
                    if (row.RowState == DataRowState.Modified)
                    {
                        for (int c = 0; c < colsCount; c++)
                        {
                            SqlParameter prm = prmlist[c];
                            prm.Value = row[prm.Name];
                        }

                        cmd.Execute();
                        row.AcceptChanges();
                        rowsAffected++;
                    }
                }
            }
            return rowsAffected;
        }

        public virtual int Delete()
        {
            return DeleteRows(this.Tables[0].TableName);
        }

        public virtual int Delete(string table_name)
        {
            return DeleteRows(table_name);
        }

        protected int DeleteRows(string table_name)
        {
            DataTable myTable = this.Tables[table_name];
            DataRowCollection rows = myTable.Rows;
            int rowsCount = rows.Count;
            int rowsAffected = 0;

            if (rowsCount == 0)
                return 0;

            DeleteStatement cmd = new DeleteStatement(myTable.ExtendedProperties["Storage"].ToString());
            cmd.ConnectionStringName = _connectionStringName;
            List<SqlParameter> prmlist = new List<SqlParameter>();
            int colsCount = myTable.Columns.Count;
            int keys = 0;
            string identcol = String.Empty;
            for (int c = 0; c < colsCount; c++)
            {
                DataColumn col = myTable.Columns[c];

                if (col.ExtendedProperties["PrimaryKey"] != null)
                {
                    SqlParameter prm = new SqlParameter(col.ColumnName, col.DataType);
                    prmlist.Add(prm);
                    cmd.Where(Condition.IsEqual(col.ExtendedProperties["Storage"].ToString(), prm));
                    keys++;
                }
            }

            List<DataRow> toDelete = new List<DataRow>();
            foreach (DataRow row in myTable.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    for (int p = 0; p < keys; p++)
                    {
                        SqlParameter prm = prmlist[p];
                        prm.Value = row[prm.Name, DataRowVersion.Original];
                    }

                    cmd.Execute();
                    toDelete.Add(row);
                    rowsAffected++;
                }

            }
            toDelete.ForEach(delegate(DataRow row) { row.AcceptChanges(); });


            return rowsAffected;
        }

        public virtual int FullUpdate(string table_name)
        {
            int rowsAffected = 0;
            rowsAffected += DeleteRows(table_name);
            rowsAffected += InsertRows(table_name);
            rowsAffected += UpdateRows(table_name);
            return rowsAffected;
        }

        public string[] GetChangedTableList()
        {
            var tableList = new List<string>();
            for (var t = 0; t < this.Tables.Count; t++)
            {
                var table = Tables[t];
                var rowsCount = table.Rows.Count;
                if (rowsCount == 0) continue;

                for (var r = 0; r < rowsCount; r++)
                {
                    var row = table.Rows[r];
                    if ((row.RowState != DataRowState.Added && row.RowState != DataRowState.Deleted) &&
                        row.RowState != DataRowState.Modified) continue;

                    tableList.Add(table.TableName);
                    break;
                }

            }
            return tableList.ToArray();
        }

        public string WriteToDisk(string data_dir)
        {

            string path = "";
            DataTable mainTable = this.Tables[0];

            if (mainTable.Rows.Count > 0 && mainTable.Columns[0].ExtendedProperties.ContainsKey("PrimaryKey"))
            {
                if (data_dir.Substring(data_dir.Length - 1) != "\\")
                    data_dir += "\\";

                string targetDir = data_dir + mainTable.TableName;
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);
                path = targetDir + "\\" + mainTable.Rows[0][0].ToString() + ".xml";
                this.WriteXml(path);

            }
            return path;
        }

        #endregion

    }
}
