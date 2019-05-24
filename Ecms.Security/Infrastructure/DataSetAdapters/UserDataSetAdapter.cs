using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecms.Core.Infrastructure;
using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Mappings;

namespace Ecms.Security.Infrastructure.DataSetAdapters
{
    public class UserDataSetAdapter: DataSetAdapter<User, int,  UserMapping>
    {
        public UserDataSetAdapter()
            : base("Security")
        {
            
        }

        public UserDataSetAdapter(string connection_string_name)
            : base(connection_string_name)
        {
        }
    }
}
