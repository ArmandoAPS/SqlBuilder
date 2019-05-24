using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecms.Core.Infrastructure;
using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Mappings;

namespace Ecms.Security.Infrastructure.DataSetAdapters
{
    public class LogDataSetAdapter: DataSetAdapter<Log, Guid, LogMapping>
    {
        public LogDataSetAdapter()
            : base("Security")
        {
            
        }

        public LogDataSetAdapter(string connection_string_name)
            : base(connection_string_name)
        {
        }
    }
}
