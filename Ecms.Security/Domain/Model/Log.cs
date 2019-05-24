using Ecms.Core.Domain.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;

namespace Ecms.Security.Domain.Model
{
    public class Log : IEntity<Guid>
    {
        #region Constants
        // entity constant
        public const string LOG = "Log";

        // properties constant

        public const string DESCRIPTION = "Description";
        public const string IP_ADDRESS = "IpAddress";
        public const string CREATED_TIME = "CreatedTime";
        public const string CREATED_BY = "CreatedBy";


        #endregion

        #region Properties
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedBy { get; set; }

              #endregion

        #region Constructors
        public Log() {
            Id = Guid.NewGuid();
            CreatedTime = DateTime.UtcNow;
        }


        public Log(string description, string userName, string ipAddress, DateTime date)
        {
            Id = Guid.NewGuid();
            Description = description; 
            CreatedTime = date;
            CreatedBy = userName;
            IpAddress = ipAddress;
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
                    if (parentProperty != null)
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
        #endregion
    }
}
