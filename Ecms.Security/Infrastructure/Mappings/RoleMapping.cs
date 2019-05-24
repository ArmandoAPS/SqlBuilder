using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecms.Security.Domain.Model;
using ANMappings;

namespace Ecms.Security.Infrastructure.Mappings
{
    public class RoleMapping: Mapping<Role>
    {
        public const string TABLE_NAME = "sec_role";
        public const string COLUMN_ID = "role_id";
        public const string COLUMN_NAME = "role_name";
        public const string COLUMN_CODE = "role_code";
        public const string COLUMN_MODULE_ID = "role_module_id";
        public const string COLUMN_CREATED_TIME = "role_created_time";
        public const string COLUMN_CREATED_BY = "role_created_by";
        public const string COLUMN_CREATED_IP_ADDRESS = "role_created_ip_address";
        public const string COLUMN_MODIFIED_TIME = "role_modified_time";
        public const string COLUMN_MODIFIED_BY = "role_modified_by";
        public const string COLUMN_MODIFIED_IP_ADDRESS = "role_modified_ip_address";
        public const string COLUMN_LOG_ID = "role_log_id";
        public const string COLUMN_IS_ACTIVE = "role_is_active";
        public const string COLUMN_IS_REMOVED = "role_is_removed";

        public RoleMapping()
        {
            Named(TABLE_NAME);
            Identity(x => x.Id).Named(COLUMN_ID);
            Map(x => x.Name).Named(COLUMN_NAME).HasMaxLength(50);
            Map(x => x.Code).Named(COLUMN_CODE).Nullable().HasMaxLength(50);
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

            MakeReferenceTo(x => x.Module)
                .ThisKey(x => x.ModuleId)
                .OtherKey(o => o.Id)
                .OtherLabel(o => o.Name)
                .OtherMapping(typeof(ModuleMapping));


            HasMany(x => x.RoleComponents)
                .ThisKey(x => x.Id)
                .OtherKey(o => o.RoleId)
                .OtherMapping(typeof(RoleComponentMapping));
        }
    }

    public class RoleComponentMapping : Mapping<Role.RoleComponent>
    {
        public const string TABLE_NAME = "rolecomponents";
        public const string COLUMN_ROLE_ID = "rc_role_id";
        public const string COLUMN_COMPONENT_ID = "rc_component_id";
        public const string COLUMN_ACTIONS = "rc_actions";

        public RoleComponentMapping()
        {
            Named(TABLE_NAME);
            Map(x => x.RoleId).Named(COLUMN_ROLE_ID).PrimaryKey();
            Map(x => x.ComponentId).Named(COLUMN_COMPONENT_ID).PrimaryKey();
            Map(x => x.Actions).Named(COLUMN_ACTIONS).HasMaxLength(512);

            MakeReferenceTo(x => x.Component)
                .ThisKey(x => x.ComponentId)
                .OtherKey(o => o.Id)
                .OtherMapping(typeof(ComponentMapping));
            
        }
    }

}
