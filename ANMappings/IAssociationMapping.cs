using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ANMappings
{
    public interface IAssociationMapping
    {
        string Name { get;  }
        IList<string> ThisKeys { get; }
        IList<string> OtherKeys { get;  }
        IList<string> OtherLabels { get; }
        IList<string> OtherColumns { get; }
        Type OtherMappingType { get;  } 
        bool IsNullable { get; }       
    }


	public interface IAssociationMapping<T, TElement>: IAssociationMapping
    {
        IAssociationMapping<T, TElement> ThisKey(string key_name);
        IAssociationMapping<T, TElement> OtherKey(string key_name);
        IAssociationMapping<T, TElement> OtherLabel(string label_name);
        IAssociationMapping<T, TElement> OtherColumn(string column_name);
        IAssociationMapping<T, TElement> OtherMapping(Type other_mapping_type);

        IAssociationMapping<T, TElement> ThisKey(Expression<Func<T, object>> key_expression);
        IAssociationMapping<T, TElement> OtherKey(Expression<Func<TElement, object>> key_expression);
        IAssociationMapping<T, TElement> OtherLabel(Expression<Func<TElement, object>> label_expression);
        IAssociationMapping<T, TElement> OtherColumn(Expression<Func<TElement, object>> column_expression);
    }
}
