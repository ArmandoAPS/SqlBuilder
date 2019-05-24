using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Mappings;
using ANMappings;

namespace Ecms.Security.Infrastructure.Mappings
{
    public class UserMapping : Mapping<User>
    {
        public const string TABLE_NAME = "users";
        public const string COLUMN_ID = "user_id";
        public const string COLUMN_NAME = "user_name";
        public const string COLUMN_USER_NAME = "user_user_name";
        public const string COLUMN_PASSWORD = "user_password";
        public const string COLUMN_EMAIL = "user_email";
        public const string COLUMN_LAST_ACTIVITY_DATE = "user_last_activity_date";
        public const string COLUMN_IS_LOCKED = "user_is_locked";
        public const string COLUMN_GROUP_ID = "user_group_id";
        public const string COLUMN_PROPERTIES = "user_properties";
        public const string COLUMN_CREATED_TIME = "user_created_time";
        public const string COLUMN_CREATED_BY = "user_created_by";
        public const string COLUMN_CREATED_IP_ADDRESS = "user_created_ip_address";
        public const string COLUMN_MODIFIED_TIME = "user_modified_time";
        public const string COLUMN_MODIFIED_BY = "user_modified_by";
        public const string COLUMN_MODIFIED_IP_ADDRESS = "user_modified_ip_address";

        public const string COLUMN_LOG_ID = "user_log_id";
        public const string COLUMN_IS_ACTIVE = "user_is_active";
        public const string COLUMN_IS_REMOVED = "user_is_removed";



        public UserMapping()
        {
            Named(TABLE_NAME);
            Identity(x => x.Id).Named(COLUMN_ID);
            Map(x => x.Name).Named(COLUMN_NAME).HasMaxLength(50);
            Map(x => x.UserName).Named(COLUMN_USER_NAME).HasMaxLength(50);
            Map(x => x.Password).Named(COLUMN_PASSWORD).HasMaxLength(100);
            Map(x => x.Email).Named(COLUMN_EMAIL).HasMaxLength(50);
            Map(x => x.LastActivityDate).Named(COLUMN_LAST_ACTIVITY_DATE).Nullable();
            Map(x => x.IsLocked).Named(COLUMN_IS_LOCKED);
            Map(x => x.Properties).Named(COLUMN_PROPERTIES).Nullable();
            Map(x => x.CreatedTime).Named(COLUMN_CREATED_TIME).Nullable().DefaultCurrentUtcDateTime();
            Map(x => x.CreatedBy).Named(COLUMN_CREATED_BY).HasMaxLength(50).Nullable();
            Map(x => x.CreatedIpAddress).Named(COLUMN_CREATED_IP_ADDRESS).HasMaxLength(40).Nullable();
            Map(x => x.ModifiedTime).Named(COLUMN_MODIFIED_TIME).Nullable().DefaultCurrentUtcDateTime();
            Map(x => x.ModifiedBy).Named(COLUMN_MODIFIED_BY).HasMaxLength(50).Nullable();
            Map(x => x.ModifiedIpAddress).Named(COLUMN_MODIFIED_IP_ADDRESS).HasMaxLength(40).Nullable();
            Map(x => x.LogId).Named(COLUMN_LOG_ID).Nullable();
            Map(x => x.IsActive).Named(COLUMN_IS_ACTIVE).Default(true);
            Map(x => x.IsRemoved).Named(COLUMN_IS_REMOVED).Default(false);

            HasMany(x => x.Roles)
                .ThisKey(x => x.Id)
                .OtherKey(o => o.UserId)
                .OtherMapping(typeof(UserRoleMapping));
        }
    }

    public class UserRoleMapping : Mapping<User.UserRole>
    {
        public const string TABLE_NAME = "sec_user_role";
        public const string COLUMN_USER_ID = "ur_user_id";
        public const string COLUMN_ROLE_ID = "ur_role_id";

        public UserRoleMapping()
        {
            Named(TABLE_NAME);
            Map(x => x.UserId).Named(COLUMN_USER_ID).PrimaryKey();
            Map(x => x.RoleId).Named(COLUMN_ROLE_ID).PrimaryKey();

            MakeReferenceTo(x => x.Role)
                .ThisKey(x => x.RoleId)
                .OtherKey(o => o.Id)
                .OtherColumn(o => o.Name)
                .OtherMapping(typeof(RoleMapping));
        }
    }

}
