using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ANMappings.Internal;

namespace ANMappings
{
    public class AssociationMapping<T, TElement> : IAssociationMapping<T, TElement>, IPropertyMapping
    {
        public string Name { get; private set; }
        public IList<string> ThisKeys { get; private set; }
        public IList<string> OtherKeys { get; private set; }
        public IList<string> OtherLabels { get; private set; }
        public IList<string> OtherColumns { get; private set; }
        public Type OtherMappingType { get; private set; }
        public bool IsNullable { get; private set; }
        public MemberInfo Property { get; private set; }

        private bool _nextBool = true;

        public AssociationMapping(MemberInfo property)
        {
            Property = property;
            Name = Property.Name;
            ThisKeys = new List<string>();
            OtherKeys = new List<string>();
            OtherLabels = new List<string>();
            OtherColumns = new List<string>();
        }

        public IAssociationMapping<T, TElement> ThisKey(string key_name)
        {
            if (string.IsNullOrEmpty(key_name))
                return this;

            if (!ThisKeys.Contains(key_name))
                ThisKeys.Add(key_name);

            return this;
        }

        public IAssociationMapping<T, TElement> OtherKey(string key_name)
        {
            if (string.IsNullOrEmpty(key_name))
                return this;

            if (!OtherKeys.Contains(key_name))
                OtherKeys.Add(key_name);

            return this;
        }

        public IAssociationMapping<T, TElement> OtherLabel(string label_name)
        {
            if (string.IsNullOrEmpty(label_name))
                return this;

            if (!OtherLabels.Contains(label_name))
                OtherLabels.Add(label_name);

            return this;
        }

        public IAssociationMapping<T, TElement> OtherColumn(string column_name)
        {
            if (string.IsNullOrEmpty(column_name))
                return this;

            if (!OtherColumns.Contains(column_name))
                OtherColumns.Add(column_name);

            return this;
        }

        public IAssociationMapping<T, TElement> OtherMapping(Type other_mapping_type)
        {
            OtherMappingType = other_mapping_type;
            return this;
        }

        public IAssociationMapping<T, TElement> ThisKey(Expression<Func<T, object>> key_expression)
        {
            return ThisKey(key_expression.GetPropertyName());
        }

        public IAssociationMapping<T, TElement> OtherKey(Expression<Func<TElement, object>> key_expression)
        {
            return OtherKey(key_expression.GetPropertyName());
        }

        public IAssociationMapping<T, TElement> OtherLabel(Expression<Func<TElement, object>> label_expression)
        {
            return OtherLabel(label_expression.GetPropertyName());
        }

        public IAssociationMapping<T, TElement> OtherColumn(Expression<Func<TElement, object>> column_expression)
        {
            return OtherColumn(column_expression.GetPropertyName());
        }

        public IAssociationMapping<T, TElement> Nullable()
        {
            IsNullable = _nextBool;
            _nextBool = true;
            return this;
        }
        public IAssociationMapping<T, TElement> Not
        {
            get
            {
                _nextBool = !_nextBool;
                return this;
            }
        }
    }
}
