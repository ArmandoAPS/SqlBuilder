using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecms.Security.Domain.Model;
using ANMappings;

namespace Ecms.Security.Infrastructure.Mappings
{
    public class ComponentMapping : Mapping<Component>
    {
        public const string TABLE_NAME = "components";
        public const string COLUMN_ID = "component_id";
        public const string COLUMN_NAME = "component_name";
        public const string COLUMN_DESCRIPTION = "component_description";
        public const string COLUMN_URL = "component_url";
        public const string COLUMN_DISPLAY_ORDER = "component_display_order";
        public const string COLUMN_CODE = "component_code";
        public const string COLUMN_SHOW_IN_MENU = "component_show_in_menu";
        public const string COLUMN_MENU_ID = "component_menu_id";
        public const string COLUMN_MODULE_ID = "component_module_id";
        public const string COLUMN_CREATED_TIME = "component_created_time";
        public const string COLUMN_CREATED_BY = "component_created_by";
        public const string COLUMN_CREATED_IP_ADDRESS = "component_created_ip_address";
        public const string COLUMN_MODIFIED_TIME = "component_modified_time";
        public const string COLUMN_MODIFIED_BY = "component_modified_by";
        public const string COLUMN_MODIFIED_IP_ADDRESS = "component_modified_ip_address";
        public const string COLUMN_LOG_ID = "component_log_id";
        public const string COLUMN_IS_ACTIVE = "component_is_active";
        public const string COLUMN_IS_REMOVED = "component_is_removed";

        public ComponentMapping()
        {
            Named(TABLE_NAME);
            Identity(x => x.Id).Named(COLUMN_ID);
            Map(x => x.Code).Named(COLUMN_CODE).HasMaxLength(50);
            Map(x => x.Name).Named(COLUMN_NAME).HasMaxLength(50);
            Map(x => x.Description).Named(COLUMN_DESCRIPTION).HasMaxLength(512).Nullable();
            Map(x => x.Url).Named(COLUMN_URL).HasMaxLength(100).Nullable();
            Map(x => x.DisplayOrder).Named(COLUMN_DISPLAY_ORDER);
            Map(x => x.ShowInMenu).Named(COLUMN_SHOW_IN_MENU).Nullable();
            Map(x => x.MenuId).Named(COLUMN_MENU_ID).Nullable();
            Map(x => x.ModuleId).Named(COLUMN_MODULE_ID);
            Map(x => x.CreatedTime).Named(COLUMN_CREATED_TIME).Nullable().DefaultCurrentUtcDateTime();
            Map(x => x.CreatedBy).Named(COLUMN_CREATED_BY).HasMaxLength(50).Nullable();
            Map(x => x.CreatedIpAddress).Named(COLUMN_CREATED_IP_ADDRESS).HasMaxLength(40).Nullable();
            Map(x => x.ModifiedTime).Named(COLUMN_MODIFIED_TIME).Nullable().DefaultCurrentUtcDateTime();
            Map(x => x.ModifiedBy).Named(COLUMN_MODIFIED_BY).HasMaxLength(50).Nullable();
            Map(x => x.ModifiedIpAddress).Named(COLUMN_MODIFIED_IP_ADDRESS).HasMaxLength(40).Nullable();
            Map(x => x.LogId).Named(COLUMN_LOG_ID).Nullable();
            Map(x => x.IsActive).Named(COLUMN_IS_ACTIVE).Default(true);
            Map(x => x.IsRemoved).Named(COLUMN_IS_REMOVED).Default(false);

            MakeReferenceTo(x => x.Menu)
                .ThisKey(x => x.MenuId)
                .OtherKey(o => o.Id)
                .OtherLabel(o => o.Name)
                .OtherMapping(typeof(MenuMapping));
        }

    }
}
