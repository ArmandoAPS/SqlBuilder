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
    public class MenuMapping: Mapping<Menu>
    {
        public const string TABLE_NAME = "menus";
        public const string COLUMN_ID = "menu_id";
        public const string COLUMN_NAME = "menu_name";
        public const string COLUMN_DESCRIPTION = "menu_description";
        public const string COLUMN_URL = "menu_url";
        public const string COLUMN_DISPLAY_ORDER = "menu_display_order";
        public const string COLUMN_MODULE_ID = "menu_module_id";
        public const string COLUMN_CREATED_TIME = "menu_created_time";
        public const string COLUMN_CREATED_BY = "menu_created_by";
        public const string COLUMN_CREATED_IP_ADDRESS = "menu_created_ip_address";
        public const string COLUMN_MODIFIED_TIME = "menu_modified_time";
        public const string COLUMN_MODIFIED_BY = "menu_modified_by";
        public const string COLUMN_MODIFIED_IP_ADDRESS = "menu_modified_ip_address";
        public const string COLUMN_LOG_ID = "menu_log_id";
        public const string COLUMN_IS_ACTIVE = "menu_is_active";
        public const string COLUMN_IS_REMOVED = "menu_is_removed";

        public MenuMapping()
        {
            Named(TABLE_NAME);
            Identity(x => x.Id).Named(COLUMN_ID);
            Map(x => x.Name).Named(COLUMN_NAME).HasMaxLength(50);
            Map(x => x.Description).Named(COLUMN_DESCRIPTION).HasMaxLength(512).Nullable();
            Map(x => x.Url).Named(COLUMN_URL).HasMaxLength(100).Nullable();
            Map(x => x.DisplayOrder).Named(COLUMN_DISPLAY_ORDER);
            Map(x => x.ModuleId).Named(COLUMN_MODULE_ID);
            Map(x => x.CreatedTime).Named(COLUMN_CREATED_TIME).Nullable().DefaultCurrentUtcDateTime();
            Map(x => x.CreatedBy).Named(COLUMN_CREATED_BY).HasMaxLength(50).Nullable();
            Map(x => x.CreatedIpAddress).Named(COLUMN_CREATED_IP_ADDRESS).HasMaxLength(40).Nullable();
            Map(x => x.ModifiedTime).Named(COLUMN_CREATED_TIME).Nullable().DefaultCurrentUtcDateTime();
            Map(x => x.ModifiedBy).Named(COLUMN_CREATED_BY).HasMaxLength(50).Nullable();
            Map(x => x.ModifiedIpAddress).Named(COLUMN_CREATED_IP_ADDRESS).HasMaxLength(40).Nullable();
            Map(x => x.LogId).Named(COLUMN_LOG_ID).Nullable();
            Map(x => x.IsActive).Named(COLUMN_IS_ACTIVE).Default(true);
            Map(x => x.IsRemoved).Named(COLUMN_IS_REMOVED).Default(false);
        }
    }
}
