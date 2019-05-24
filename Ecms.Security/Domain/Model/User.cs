using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecms.Core.Domain.Model;
using Ecms.Security.Domain.Model;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Ecms.Security.Domain.Model
{
    public class User: Entity
    {
        #region Constants
        // entity constant
        public const string USER = "User";
        public const string ROLES = "Roles";

        // properties constants
        public const string USER_NAME = "UserName";
        public const string PASSWORD = "Password";
        public const string NAME = "Name";
        public const string EMAIL = "Email";
        public const string IS_LOCKED = "IsLocked";
        public const string LAST_ACTIVITY_DATE = "LastActivityDate";
        public const string PROPERTIES = "Properties";

        #endregion

        #region Properties
        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(100)]
        public string Password { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        public string Properties { get; set; }

        public DateTime? LastActivityDate { get; set; }
        public bool IsLocked { get; set; }

        private List<UserRole> _Roles;
        public IEnumerable<UserRole> Roles {
            get
            {
                return _Roles.AsReadOnly();
            }
        }

      
        #endregion

        #region Constructors
        public User()
        {
            _Roles = new List<UserRole>();
        }

        public User(int Id) : base(Id) { }
        #endregion

        #region Methods
        public static string EncryptPassword(string password)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();

            byte[] inputBytes = (new UnicodeEncoding()).GetBytes(password);
            byte[] hash = sha1.ComputeHash(inputBytes);

            return Convert.ToBase64String(hash);
        }
        #endregion

        #region Inner Classes

        public class UserRole : ValueObject
        {
            #region Constants
            public const string USER_ROLE = "UserRole";

            public const string USER_ID = "UserId";
            public const string ROLE_ID = "RoleId";

            #endregion

            #region Properties
            public int UserId { get; set; }
            public int RoleId { get; set; }

            public Role Role { get; private set; }
            #endregion

            #region Constructor

            public UserRole() { }

            public UserRole(int userId, int roleId)
            {
                this.UserId = userId;
                this.RoleId = roleId;
            }

            public UserRole(int userId, Role role)
            {
                this.UserId = userId;
                this.Role = role;
                this.RoleId = role.Id;
            }

            public UserRole(Dictionary<string, object> property_values)
            {
                Role = new Role();
                foreach (KeyValuePair<string, object> property_value in property_values)
                    this[property_value.Key] = property_value.Value;
            }
            #endregion

            #region Infrastructure
            protected override IEnumerable<object> GetAtomicValues()
            {
                yield return UserId;
                yield return RoleId;
            }
            #endregion

        }
        #endregion
    }
}
