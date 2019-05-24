using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecms.Security.Domain.Model;
using ANMappings;

namespace Ecms.Security.Infrastructure.Mappings
{
    public class LogMapping: Mapping<Log>
    {
        public const string TABLE_NAME = "logs";
        public const string COLUMN_ID = "log_id";
        public const string COLUMN_DESCRIPTION = "log_description";
        public const string COLUMN_CREATED_TIME = "log_created_time";
        public const string COLUMN_CREATED_BY = "log_created_by";
        public const string COLUMN_IP_ADDRESS = "log_ip_address";

        public LogMapping()
        {
            Named(TABLE_NAME);
            Map(x => x.Id).Named(COLUMN_ID).PrimaryKey();
            Map(x => x.Description).Named(COLUMN_DESCRIPTION).HasMaxLength(100);
            Map(x => x.CreatedTime).Named(COLUMN_CREATED_TIME);
            Map(x => x.CreatedBy).Named(COLUMN_CREATED_BY).Nullable();
            Map(x => x.IpAddress).Named(COLUMN_IP_ADDRESS);
        }
    }


}
