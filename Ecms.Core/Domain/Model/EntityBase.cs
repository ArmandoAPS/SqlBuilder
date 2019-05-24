using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ecms.Core.Domain.Model
{
    [Serializable]
    public abstract class EntityBase
    {
        public const string ID = "Id";
        public const string CREATED_TIME = "CreatedTime";
        public const string CREATED_BY = "CreatedBy";
        public const string CREATED_IP_ADDRESS = "CreatedIpAddress";
        public const string MODIFIED_TIME = "ModifiedTime";
        public const string MODIFIED_BY = "ModifiedBy";
        public const string MODIFIED_IP_ADDRESS = "ModifiedIpAddress";
        public const string LOG_ID = "LogId";
        public const string IS_ACTIVE = "IsActive";
        public const string IS_REMOVED = "IsRemoved";

      
        public DateTime? CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedIpAddress { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedIpAddress { get; set; }
        public Guid? LogId { get; set; }
        public bool IsActive { get; set; }
        public bool IsRemoved { get; set; }

        public EntityBase()
        {
            CreatedTime = DateTime.UtcNow;
            IsRemoved = false;
            IsActive = true;
        }

        public virtual void AddValueObject(string property_name, Dictionary<string, object> values)
        {
        }

        public PropertyInfo GetProperty(string name)
        {
            return GetType().GetProperty(name);

        }

        public virtual object this[string name] 
        {
            get
            {
                Type myType = GetType();
                
                if (name.IndexOf(".") > 0)
                {
                    string[] names = name.Split('.');
                    var parentProperty = myType.GetProperty(names[0]).GetValue(this, null);
                    if(parentProperty != null)
                        return parentProperty.GetType().GetProperty(names[1]).GetValue(parentProperty, null);
                    return null;
                }
                return myType.GetProperty(name).GetValue(this, null);
            }

            set
            {
                if (value == DBNull.Value)
                    value = null;

                Type myType = GetType();
                

                if (name.IndexOf(".") > 0)
                { 
                    string[] names = name.Split('.');
                    var parentProperty = myType.GetProperty(names[0]).GetValue(this, null);
                    if (parentProperty != null)
                    {
                        var prop = parentProperty.GetType().GetProperty(names[1]);
                        prop.SetValue(parentProperty, prop.PropertyType.IsEnum ? Enum.Parse(prop.PropertyType, value.ToString(), true) : value, null);
                    }
                }
                else
                {
                    var prop = myType.GetProperty(name);
                    myType.GetProperty(name).SetValue(this, prop.PropertyType.IsEnum ? Enum.Parse(prop.PropertyType, value.ToString(), true) : value, null);
                }
            }
        }
    }
}
