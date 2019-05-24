using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using ANCommon.DataAccess;
using ANCommon.Sql;
using ANMappings;
using ANSqlBuilder;
using Ecms.Core.Domain.Model;


namespace Ecms.Core.Infrastructure
{
    public class Repository<TEntity,TKey,TMappingInfo> : IRepository<TEntity>
        where TEntity : IEntity<TKey>, new()
        where TMappingInfo : Mapping<TEntity>, new()
    {
        private readonly IMapping _metadata;
        private string _connectionStringName = "IMSDB";
        private readonly string _tableName;
        private string _tableAlias;
        private string _entityName;
        private IEnumerable<IAssociationMapping> _associations;
        private IEnumerable<IAssociationMapping> _hasManyAssociations;

        public bool IgnoreMemoFields { get; set; }
        public bool IdentityInsert { get; set; }

        public Repository()
        {
            _metadata = MappingFactory.GetMapping<TMappingInfo>();
            _tableName = _metadata.TableName;
            _tableAlias = _metadata.EntityName;
            _entityName = _metadata.EntityName;
            _associations = _metadata.Associations.Where(x => x.GetType().Name == "MakeReferenceToMapping`2");
            _hasManyAssociations = _metadata.Associations.Where(x => x.GetType().Name == "HasManyMapping`2");
        }

        public Repository(string connection_string_name)
            : this()
        {
            _connectionStringName = connection_string_name;
        }

        public string GetConnectionStringName()
        {
            return _connectionStringName;
        }

        public TEntity GetById(TKey id)
        {
            ICondition criteria = IdentityInsert
                ? Conditions.IsEqual(Entity.ID, new SqlNumber(id.ToString()))
                : Conditions.IsEqual(Entity.ID, new SqlString(id.ToString()));
            return GetOne(criteria);
        }


        public virtual void Save(TEntity entity)
        {
            if (IdentityInsert)
            {
                if(entity.Id.ToString() == "0")
                    Insert(entity);
                else
                    Update(entity);
            }
            else
                Insert(entity);
        }

        public virtual void Insert(TEntity entity)
        {
            var insert = SqlUtils.CreateInsertStatement(_metadata,IdentityInsert);
            insert.ConnectionStringName = _connectionStringName;

            foreach (var item in _metadata.Columns)
            {
                if (IdentityInsert || !item.IsDbGenerated)
                    insert.SetParamValue(item.Name, entity[item.Name]);
            }

            if (!String.IsNullOrEmpty(insert.IdentityColumnName))
            {
                object id;
                insert.Execute(out id);
                entity[insert.IdentityColumnName] = Int32.Parse(id.ToString());
            }
            else
                insert.Execute();

            if (_hasManyAssociations.Count() == 0)
                return;

            foreach (IAssociationMapping hma in _hasManyAssociations)
            {
                var items = entity[hma.Name] as IEnumerable<IIndexable>;

                if (items == null || items.Count() == 0)
                    continue;

                var omapping = MappingFactory.GetMapping(hma.OtherMappingType);

                InsertStatement insert2 = SqlUtils.CreateInsertStatement(omapping);
                insert2.ConnectionStringName = insert.ConnectionStringName;
                int colCount = omapping.Columns.Count;

                foreach (TEntity item in items)
                {
                    if (!String.IsNullOrEmpty(insert.IdentityColumnName))
                        item[hma.OtherKeys[0]] = entity[insert.IdentityColumnName];

                    for (var x = 0; x < colCount; x++)
                    {
                        var propName = omapping.Columns[x].Name;

                        insert2.SetParamValue(propName, item[propName]);
                    }

                    if (!String.IsNullOrEmpty(insert2.IdentityColumnName))
                    {
                        object id;
                        insert2.Execute(out id);
                        item[insert2.IdentityColumnName] = Int32.Parse(id.ToString());
                    }
                    else
                        insert2.Execute();
                }
            }
        }

        public virtual void Update(TEntity entity)
        {
            ICondition criteria = IdentityInsert 
                ? Conditions.IsEqual(Entity.ID, new SqlNumber(entity.Id.ToString()))
                : Conditions.IsEqual(Entity.ID, new SqlString(entity.Id.ToString()));

            var current = GetOne(criteria);
            if (current == null)
                return;

            bool hasChanges = false;
            foreach (var item in _metadata.Columns)
            {
                if (current[item.Name] != entity[item.Name])
                {
                    hasChanges = true;
                    break;
                }
            }

            if (hasChanges)
            {
                var update = SqlUtils.CreateUpdateStatement(_metadata);
                update.ConnectionStringName = _connectionStringName;

                foreach (var item in _metadata.Columns)
                    update.SetParamValue(item.Name, entity[item.Name]);

                update.Execute();
            }

            if (_hasManyAssociations.Count() == 0)
                return;

            foreach (IAssociationMapping hma in _hasManyAssociations)
            {
                var items = entity[hma.Name] as IEnumerable<ValueObject>;

                if (items == null)
                    continue;

                var storedItems = current[hma.Name] as IEnumerable<ValueObject>;
               // if (storedItems == null)
               //     continue;


                var omapping = MappingFactory.GetMapping(hma.OtherMappingType);
                //var primKeys = omapping.Columns.Where(o => o.IsPrimaryKey || o.IsDbGenerated).ToList();

                DeleteStatement deleteStament = SqlUtils.CreateDeleteStatement(omapping, true);
                deleteStament.ConnectionStringName = _connectionStringName;
                InsertStatement insertStament = SqlUtils.CreateInsertStatement(omapping);
                insertStament.ConnectionStringName = _connectionStringName;

                
                if (storedItems != null)
                {
                    // Delete non existent
                    foreach (ValueObject storedItem in storedItems)
                    {
                        var itemFound = items.FirstOrDefault(ci => ci.Equals(storedItem));
                        if (itemFound == null)
                        {
                            foreach (ColumnMapping cm in omapping.Columns)
                                deleteStament.SetParamValue(cm.Name, storedItem[cm.Name]);

                            deleteStament.Execute();
                        }
                    }
                    
                    // Insert non existent
                    foreach (ValueObject item in items)
                    {
                        var itemFound = storedItems.FirstOrDefault(si => si.Equals(item));
                        if (itemFound == null)
                        {
                            foreach (ColumnMapping cm in omapping.Columns)
                                insertStament.SetParamValue(cm.Name, item[cm.Name]);

                            insertStament.Execute();
                        }
                    }
                }
                else
                {
                    foreach (ValueObject item in items)
                    {
                        foreach (ColumnMapping cm in omapping.Columns)
                            insertStament.SetParamValue(cm.Name, item[cm.Name]);

                        insertStament.Execute();
                    }
                }
                
            }

        }

        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
                return;

            Conditions filter = new Conditions();
            var items = _metadata.Columns;
            foreach (var item in items)
            {
                if (item.IsPrimaryKey)
                    filter.And(new Condition(item.ColumnName, Comparisons.Equals, new SqlString(entity[item.Name].ToString())));
            }
            if(filter.Count > 0)
                Delete(filter);
        }

        public virtual void Delete(ICondition filter)
        {
            if (filter == null)
                return;

            var delete = new DeleteStatement(_tableName)
            {
                ConnectionStringName = _connectionStringName
            };
            
            delete.Where(filter);
            delete.Execute();
        }

        public virtual void Remove(TEntity entity)
        {
            if (entity == null)
                return;

            Conditions filter = new Conditions();
            var items = _metadata.Columns;
            foreach (var item in items)
            {
                if (item.IsPrimaryKey)
                    filter.And(new Condition(item.ColumnName, Comparisons.Equals, new SqlString(entity[item.Name].ToString())));
            }
            if (filter.Count > 0)
                Remove(filter);
        }

        public virtual void Remove(ICondition filter)
        {
            if (filter == null)
                return;

            var update = new UpdateStatement(_tableName)
            {
                ConnectionStringName = _connectionStringName
            };
            var item = _metadata.Columns.First(x => x.Name == Entity.IS_REMOVED);
            if (item != null)
            {
                update.Column(item.ColumnName, SqlBoolean.TRUE);
                update.Where(filter);
                update.Execute();
            }
        }

        protected virtual TEntity ReadEntity(IDataReader reader)
        {
            var entity = new TEntity();

            var items = _metadata.Columns;
            foreach (var item in items)
            {
                if (IgnoreMemoFields && item.IsMemo)
                    continue;

                var fieldIndex = reader.GetOrdinal(item.Name);
                if (reader.IsDBNull(fieldIndex)) continue;
                entity[item.Name] = reader.GetValue(fieldIndex);
            }

            if (_associations == null)
                return entity;

            foreach (var belong in _associations)
            {
                var fieldIndex = reader.GetOrdinal(belong.ThisKeys[0]);

                if (reader.IsDBNull(fieldIndex)) continue;

                var propEntity = entity[belong.Name] as Entity;
                if (propEntity == null)
                    continue;

                var omapping = MappingFactory.GetMapping(belong.OtherMappingType);
                
                for (var x = 0; x < omapping.Columns.Count; x++)
                {
                    var item = omapping.Columns[x];
                    var itemName = belong.Name + "." + item.Name;
                    fieldIndex = reader.GetOrdinal(itemName);
                    if (!reader.IsDBNull(fieldIndex))
                        propEntity[item.Name] = reader.GetValue(fieldIndex);
                }
            }


            return entity;
        }

        protected SelectStatement GetBaseSelectStatement()
        {
            var colNames = new List<string>();

            var select = new SelectStatement
            {
                ConnectionStringName = _connectionStringName
            };
            select.From(_tableName, _tableAlias);

            for (var x = 0; x < _metadata.Columns.Count; x++)
            {
                var col = _metadata.Columns[x];
                if (col.Name.IndexOf(".") < 0)
                    select.Column(new SqlColumnName(_tableAlias, col.ColumnName), col.Name);
            }

            //if (_associations == null)
            //    _associations = _metadata.Associations.Where(x => x.GetType().Name == "BelongsToMapping`2" || x.GetType().Name == "HasOneMapping`2");

            if (_associations.Count() == 0)
                return select;

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

                    select.Column(new SqlColumnName(item.Name, col.ColumnName), item.Name + "." + col.Name);
                }

                var keyName = item.ThisKeys[0];
                var otherKeyName = item.OtherKeys[0];
                var tcol = _metadata.Columns.Where(x => x.Name == keyName).Single();
                var ocol = omapping.Columns.Where(x => x.Name == otherKeyName).Single();
                if (tcol.IsNullable)
                    select.LeftJoin(omapping.TableName, item.Name, Condition.IsEqual(new SqlColumnName(_entityName, tcol.ColumnName), new SqlColumnName(item.Name, ocol.ColumnName)));
                else
                    select.InnerJoin(omapping.TableName, item.Name, Condition.IsEqual(new SqlColumnName(_entityName, tcol.ColumnName), new SqlColumnName(item.Name, ocol.ColumnName)));


            }
            return select;
        }

        protected virtual SelectStatement GetSelectStatement(ICondition criteria, IOrderByExpression order_by_expression)
        {
            var colNames = new List<string>();
            var select = new SelectStatement
            {
                ConnectionStringName = _connectionStringName
            };

            select.From(GetBaseSelectStatement(), _entityName);

            for (var x = 0; x < _metadata.Columns.Count; x++)
            {
                string colName = _metadata.Columns[x].Name;
                colNames.Add(colName);
                select.Column(new SqlColumnName(_metadata.Columns[x].Name), colName);
            }


            if (_associations != null)
            {
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
            }

            if (criteria != null)
                select.Where(criteria);

            if (order_by_expression != null)
                select.OrderBy(order_by_expression);

            return select;
        }

        protected virtual SelectStatement GetSelectCountStatement(ICondition criteria)
        {
            var select = new SelectStatement
            {
                ConnectionStringName = _connectionStringName
            };
            select.Column(new SqlLiteral("COUNT(*)"));
            select.From(GetBaseSelectStatement(), "Data");


            if (criteria != null)
                select.Where(criteria);

            return select;
        }

        public virtual TEntity GetOne(ICondition filter)
        {
            return GetOne(filter, null);
        }

        public virtual TEntity GetOne(ICondition filter, IOrderByExpression order_by)
        {
            var select = GetSelectStatement(filter, order_by);

            var reader = select.ExecuteReader();
            TEntity entity = default(TEntity);

            if (reader.Read())
                entity = ReadEntity(reader);

            reader.Close();

            if (entity == null)
                return default(TEntity);

            if (_associations.Count() > 0)
            {
                foreach (IAssociationMapping _association in _associations)
                {
                    var amapping = MappingFactory.GetMapping(_association.OtherMappingType);

                    object val = entity[_association.ThisKeys[0]];
                    if (val == null)
                        continue;

                    int colCount = amapping.Columns.Count;
                    var select2 = SqlUtils.CreateSelectStatement(amapping);
                    select2.ConnectionStringName = select.ConnectionStringName;

                    select2.Where(Condition.IsEqual(_association.OtherKeys[0], new SqlNumber((int)val)));
                    var values = new Dictionary<string, object>();
                    var reader2 = select2.ExecuteReader();
                    if (reader2.Read())
                    {
                        if(entity[_association.Name] == null)
                            entity[_association.Name] = Activator.CreateInstance(entity.GetProperty(_association.Name).PropertyType, (int)val);

                        Entity _prop = entity[_association.Name] as Entity;
                        
                        for (int f = 0; f < reader2.FieldCount; f++)
                            _prop[reader2.GetName(f)] = reader2.GetValue(f);
                    }
                    reader2.Close();
                }
            }

            if (_hasManyAssociations.Count() == 0)
                return entity;

            foreach (IAssociationMapping hma in _hasManyAssociations)
            {
                var omapping = MappingFactory.GetMapping(hma.OtherMappingType);
                int colCount = omapping.Columns.Count;
                var select2 = SqlUtils.CreateSelectStatement(omapping);
                select2.ConnectionStringName = select.ConnectionStringName;

                ICondition idCriteria = IdentityInsert
                    ? Conditions.IsEqual(hma.OtherKeys[0], new SqlNumber(entity.Id.ToString()))
                    : Conditions.IsEqual(hma.OtherKeys[0], new SqlString(entity.Id.ToString()));

                select2.Where(idCriteria);

                var reader2 = select2.ExecuteReader();
                while (reader2.Read())
                {
                    var values = new Dictionary<string, object>();
                    for (int f = 0; f < reader2.FieldCount; f++)
                        values.Add(reader2.GetName(f), reader2.GetValue(f));

                    entity.AddValueObject(hma.Name, values);
                }
                reader2.Close();
            }

            return entity;
        }

        /*public TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter)
        {
            return GetFirstOrDefault(ConvertToConditions(filter));
        }*/

        public IEnumerable<TEntity> GetAll(ICondition filter)
        {
            return GetAll(filter, null);
        }

        public IEnumerable<TEntity> GetAll(ICondition filter, IOrderByExpression order_by_expression)
        {
            var select = GetSelectStatement(filter, order_by_expression);

            var reader = select.ExecuteReader();
            var list = new List<TEntity>();
            while (reader.Read())
                list.Add(ReadEntity(reader));
            reader.Close();
            return list;
        }

        public IEnumerable<TEntity> GetAll(ICondition criteria, IOrderByExpression order_by_expression, int start_record, int max_records)
        {
            var select = GetSelectStatement(criteria, order_by_expression);

            var reader = select.ExecuteReader(start_record);
            var list = new List<TEntity>();
            for (var i = 0; i < max_records; i++)
            {
                if (reader.Read())
                    list.Add(ReadEntity(reader));
                else
                    break;
            }
            reader.Close();
            return list;
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter)
        {
            return GetAll(ConvertToConditions(filter), null);
        }

        public int GetCount()
        {
            int result = 0;
            var select = GetSelectCountStatement(null);
            Int32.TryParse(select.ExecuteScalar().ToString(), out result);
            return result;

        }

        public int GetCount(Expression<Func<TEntity, bool>> filter)
        {
            return GetCount(ConvertToConditions(filter));
        }

        public int GetCount(ICondition criteria)
        {
            int result = 0;
            var select = GetSelectCountStatement(criteria);
            Int32.TryParse(select.ExecuteScalar().ToString(), out result);
            return result;
        }

        protected ICondition ConvertToConditions(Expression expression)
        {
            var _expression = expression as BinaryExpression;

            if (expression.NodeType == ExpressionType.AndAlso)
                return new Conditions(ConvertToConditions(_expression.Left))
                    .And(ConvertToConditions(((BinaryExpression)expression).Right));

            if (expression.NodeType == ExpressionType.OrElse)
                return new Conditions(ConvertToConditions(_expression.Left))
                    .Or(ConvertToConditions(((BinaryExpression)expression).Right));

            ISqlExpression left = null;
            ISqlExpression right = null;

            #region Left Evaluation
            if (_expression.Left.NodeType == ExpressionType.MemberAccess)
            {
                var leftExp = _expression.Left as MemberExpression;
                if (leftExp.Expression != null)
                {
                    var column = _metadata.Columns.FirstOrDefault(x => x.Name == leftExp.Member.Name);
                    var colName = _metadata.TableAlias + "." + column.ColumnName;
                    left = new SqlColumnName(colName, column.ColumnName);
                }
                else if (leftExp.Member.DeclaringType == typeof(DateTime))
                    left = new SqlDateTime(Convert.ToDateTime(ReflectionUtils.GetMemberValue(leftExp.Member, null)));
                else if (leftExp.Member.DeclaringType == typeof(bool))
                    left = new SqlNumber(Convert.ToBoolean(ReflectionUtils.GetMemberValue(leftExp.Member, null)) ? 1 : 0);
                else
                    left = new SqlString(ReflectionUtils.GetMemberValue(leftExp.Member, null).ToString());
            }
            else if (_expression.Left.NodeType == ExpressionType.Constant)
            {
                var constant = _expression.Left as ConstantExpression;

                if (constant.Type == typeof(bool))
                    left = new SqlBoolean((bool)constant.Value);
                else
                    left = new SqlString(constant.Value.ToString());
            }
            #endregion

            #region Right Evaluation
            if (_expression.Right.NodeType == ExpressionType.MemberAccess)
            {
                var rightExp = _expression.Right as MemberExpression;
                if (rightExp.Expression != null)
                {
                    var column = _metadata.Columns.FirstOrDefault(x => x.Name == rightExp.Member.Name);
                    var colName = _metadata.TableAlias + "." + column.ColumnName;
                    right = new SqlColumnName(colName, column.ColumnName);
                }
                else if (rightExp.Member.DeclaringType == typeof(DateTime))
                    right = new SqlDateTime(Convert.ToDateTime(ReflectionUtils.GetMemberValue(rightExp.Member, null)));
                else if (rightExp.Member.DeclaringType == typeof(bool))
                    right = new SqlNumber(Convert.ToBoolean(ReflectionUtils.GetMemberValue(rightExp.Member, null)) ? 1 : 0);
                else
                    right = new SqlString(ReflectionUtils.GetMemberValue(rightExp.Member, null).ToString());
            }
            else if (_expression.Right.NodeType == ExpressionType.Constant)
            {
                var constant = _expression.Right as ConstantExpression;

                if (constant.Type == typeof(bool))
                    right = new SqlBoolean((bool)constant.Value);
                else
                    right = new SqlString(constant.Value.ToString());
            }
            #endregion

            return new Condition(left, GetComparisonOperator(_expression), right);

        }


        protected Comparisons GetComparisonOperator(BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                    return Comparisons.Equals;

                case ExpressionType.NotEqual:
                    return Comparisons.NotEquals;

                case ExpressionType.LessThan:
                    return Comparisons.LessThan;

                case ExpressionType.LessThanOrEqual:
                    return Comparisons.LessOrEquals;

                case ExpressionType.GreaterThan:
                    return Comparisons.GreaterThan;

                case ExpressionType.GreaterThanOrEqual:
                    return Comparisons.GreaterOrEquals;

                default:
                    throw new ArgumentException();
            }
        }



    }
}
