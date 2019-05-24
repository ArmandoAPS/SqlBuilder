using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecms.Core.Domain.Model;
using Ecms.Security.Domain.Model;
using ANMappings;

namespace Ecms.Security.Infrastructure.Mappings
{
    public class ModuleMapping: Mapping<Module>
    {
        public const string TABLE_NAME = "modules";
        public const string COLUMN_ID = "module_id";
        public const string COLUMN_CODE = "module_code";
        public const string COLUMN_NAME = "module_name";
        public const string COLUMN_DESCRIPTION = "module_description";
        public const string COLUMN_URL = "module_url";
        public const string COLUMN_DISPLAY_ORDER = "module_display_order";
        public const string COLUMN_SOLUTION_ID = "module_solution_id";
        public const string COLUMN_CREATED_TIME = "module_created_time";
        public const string COLUMN_CREATED_BY = "module_created_by";
        public const string COLUMN_CREATED_IP_ADDRESS = "module_created_ip_address";
        public const string COLUMN_MODIFIED_TIME = "module_modified_time";
        public const string COLUMN_MODIFIED_BY = "module_modified_by";
        public const string COLUMN_MODIFIED_IP_ADDRESS = "module_modified_ip_address";
        public const string COLUMN_LOG_ID = "module_log_id";
        public const string COLUMN_IS_ACTIVE = "module_is_active";
        public const string COLUMN_IS_REMOVED = "module_is_removed";


        public ModuleMapping()
        {
            Named(TABLE_NAME);
            Identity(x => x.Id).Named(COLUMN_ID);
            Map(x => x.Code).Named(COLUMN_CODE).HasMaxLength(50);
            Map(x => x.Name).Named(COLUMN_NAME).HasMaxLength(50);
            Map(x => x.Description).Named(COLUMN_DESCRIPTION).HasMaxLength(512).Nullable();
            Map(x => x.Url).Named(COLUMN_URL).HasMaxLength(100).Nullable();
            Map(x => x.DisplayOrder).Named(COLUMN_DISPLAY_ORDER);
            Map(x => x.SolutionId).Named(COLUMN_SOLUTION_ID);
            Map(x => x.CreatedTime).Named(COLUMN_CREATED_TIME).Nullable().DefaultCurrentUtcDateTime();
            Map(x => x.CreatedBy).Named(COLUMN_CREATED_BY).HasMaxLength(50).Nullable();
            Map(x => x.CreatedIpAddress).Named(COLUMN_CREATED_IP_ADDRESS).HasMaxLength(40).Nullable();
            Map(x => x.ModifiedTime).Named(COLUMN_MODIFIED_TIME).Nullable().DefaultCurrentUtcDateTime();
            Map(x => x.ModifiedBy).Named(COLUMN_MODIFIED_BY).HasMaxLength(50).Nullable();
            Map(x => x.ModifiedIpAddress).Named(COLUMN_MODIFIED_IP_ADDRESS).HasMaxLength(40).Nullable();
            Map(x => x.LogId).Named(COLUMN_LOG_ID).Nullable();
            Map(x => x.IsActive).Named(COLUMN_IS_ACTIVE).Default(true);
            Map(x => x.IsRemoved).Named(COLUMN_IS_REMOVED).Default(false);
        }
    }
}
