using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ANMappings.Internal;

namespace ANMappings
{
    public class Mapping<T> : IMapping
    {
        protected string _EntityName;
        protected string _TableName;
        protected string _TableAlias;

        public string EntityName
        {
            get { return _EntityName; }

        }
        public string TableName
        {
            get { return _TableName; }

        }

        public string TableAlias
        {
            get { return _TableAlias ?? _TableName; }
        }


        public IList<IColumnMapping> Columns { get; private set; }
        public IList<IAssociationMapping> Associations { get; private set; }

        public Mapping()
        {
            _EntityName = typeof (T).Name;
            Columns = new List<IColumnMapping>();
            Associations = new List<IAssociationMapping>();
        }

        public void Named(string table_name)
        {
            table_name.Guard("A table name must be specified when calling 'Named'");
            _TableName = table_name;
        }

        public void Aliased(string table_alias)
        {
            table_alias.Guard("A table name must be specified when calling 'Aliased'");
            _TableAlias = table_alias;
        }

        public IColumnMapping Map<TProperty>(Expression<Func<T, TProperty>> property_selector)
        {
            property_selector.Guard("An expression must be specified when calling Map");

            var property = property_selector.GetMember();

            if (property == null)
            {
                throw new ArgumentException("You can only pass MemberExpressions to Map.");
            }

            var name = property_selector.GetPropertyName();
            var columnMapping = new ColumnMapping(property, name);
            Columns.Add(columnMapping);
            return columnMapping;
        }

        public IColumnMapping Identity<TProperty>(Expression<Func<T, TProperty>> property_selector)
        {
            return Map(property_selector).PrimaryKey().DbGenerated();
        }

        public IAssociationMapping<T, TReference> HasOne<TReference>(Expression<Func<T, TReference>> property_selector)
        {
            property_selector.Guard("An expression must be specified when calling HasOne");

            var property = property_selector.GetMember();

            if (property == null)
            {
                throw new ArgumentException("You can ony pass a MemberExpression to HasOne");
            }

            var hasOne = new HasOneMapping<T, TReference>(property);
            Associations.Add(hasOne);
            return hasOne;
        }

        public IAssociationMapping<T, TElement> HasMany<TElement>(Expression<Func<T, IEnumerable<TElement>>> property_selector)
        {
            property_selector.Guard("An expression must be specified when calling HasMany");

            var property = property_selector.GetMember();

            if (property == null)
            {
                throw new ArgumentException("You can only pass a MemberExpression to HasMany");
            }

            var hasMany = new HasManyMapping<T, TElement>(property);
            Associations.Add(hasMany);
            return hasMany;
        }

        public IAssociationMapping<T, TReference> BelongsTo<TReference>(Expression<Func<T, TReference>> property_selector)
        {
            property_selector.Guard("An expression myst be specified when calling BelongsTo");

            var property = property_selector.GetMember();

            if (property == null)
            {
                throw new ArgumentException("You can only pass a MemberExpression to BelongsTo");
            }

            var belongsTo = new BelongsToMapping<T, TReference>(property);
            Associations.Add(belongsTo);
            return belongsTo;
        }


        public IAssociationMapping<T, TReference> MakeReferenceTo<TReference>(Expression<Func<T, TReference>> property_selector)
        {
            property_selector.Guard("An expression myst be specified when calling MakeReferenceTo");

            var property = property_selector.GetMember();

            if (property == null)
            {
                throw new ArgumentException("You can only pass a MemberExpression to MakeReferenceTo");
            }

            var makeRefTo = new MakeReferenceToMapping<T, TReference>(property);
            Associations.Add(makeRefTo);
            return makeRefTo;
        }

        public IColumnMapping IdentityColumnMapping => Columns.FirstOrDefault(x => x.IsDbGenerated);

    }
}
