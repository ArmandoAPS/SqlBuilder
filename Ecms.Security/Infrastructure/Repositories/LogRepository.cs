using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANMappings;
using Ecms.Core.Infrastructure;
using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Mappings;

namespace Ecms.Security.Infrastructure.Repositories
{
    public class LogRepository: Repository<Log, Guid,  LogMapping>
    {
        public LogRepository()
            : base("Security")
        {
            
        }

        public LogRepository(string connection_string_name)
            : base(connection_string_name)
        {
        }
    }
}
