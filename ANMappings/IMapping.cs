using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANMappings
{
    public interface IMapping
    {
        string EntityName { get; }
        string TableName { get; }
        string TableAlias { get;  }
        IColumnMapping IdentityColumnMapping { get; }
        IList<IColumnMapping> Columns { get; }
        IList<IAssociationMapping> Associations { get; }
    }
}
