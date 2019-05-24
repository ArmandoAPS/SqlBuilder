using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecms.Core.Infrastructure;
using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Mappings;

namespace Ecms.Security.Infrastructure.DataSetAdapters
{
    public class RoleDataSetAdapter: DataSetAdapter<Role, int,  RoleMapping>
    {
        public RoleDataSetAdapter()
            : base("Security")
        {
            
        }

        public RoleDataSetAdapter(string connection_string_name)
            : base(connection_string_name)
        {
        }
    }
}
