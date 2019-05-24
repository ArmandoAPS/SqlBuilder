using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecms.Core.Domain.Model;
using System.ComponentModel.DataAnnotations;

namespace Ecms.Security.Domain.Model
{
    public class Role: Entity
    {
        #region Constants
        // entity constant
        public const string ROLE = "Role";

        // properties constant
        public const string CODE = "Code";
        public const string NAME = "Name";
        public const string MODULE_ID = "ModuleId";
        public const string ROLE_COMPONENTS = "RoleComponents";

        #endregion

        #region Properties
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get;  set; }

        [Required]
        public int ModuleId { get; protected set; }

        public Module Module { get; internal set; }

        private List<RoleComponent> _RoleComponents;      
        public IEnumerable<RoleComponent> RoleComponents
        {
            get 
            {
                if (_RoleComponents == null)
                    return null; 
                return _RoleComponents.AsReadOnly(); }
        }
        #endregion

        #region Constructors
        public Role()
        {
            Module = new Module();
        }

        public Role(int id) : base(id) {
            Module = new Module();
        }

        public Role(Module module)
        {
            this.ModuleId = module.Id;
            this.Module = module;
        }

        #endregion

        #region Methods

        public void AddComponent(Component component, string actions)
        {
            AddComponent(component.Id, actions);
        }

        public void AddComponent(int componentId, string actions)
        {
            RoleComponent _rmi = null;
            if (_RoleComponents == null)
                _RoleComponents = new List<RoleComponent>();
            else
                _rmi = _RoleComponents.FirstOrDefault(rmi => rmi.ComponentId == componentId);
            
            if (_rmi != null)
                _RoleComponents.Remove(_rmi);
            
            _RoleComponents.Add(new RoleComponent(this.Id, componentId, actions));

        }

        public override void AddValueObject(string property_name, Dictionary<string, object> property_values)
        {
            if (property_name == ROLE_COMPONENTS)
                AddComponent(property_values);
        }

        internal void AddComponent(Dictionary<string, object> property_values)
        {
            RoleComponent rc = new RoleComponent(property_values);
            if (_RoleComponents != null)
            {
                int compId = rc.ComponentId;
                var _rc = _RoleComponents.FirstOrDefault(x => x.ComponentId == compId);
                if (_rc == null)
                {
                    _RoleComponents.Add(rc);
                }
            }
            else
            {
                _RoleComponents = new List<RoleComponent>
                {
                    rc
                };
            }

        }

        public bool RemoveComponent(Component component)
        {
            return RemoveComponent(component.Id);
        }

        public bool RemoveComponent(int componentId)
        {
            var _rmi = _RoleComponents.FirstOrDefault(rmi => rmi.ComponentId == componentId);
            if (_rmi != null)
            {
                _RoleComponents.Remove(_rmi);
                return true;
            }

            return false;
        }


        #endregion

        #region Inner Classes

        public class RoleComponent : ValueObject
        {
            #region Constants
            public const string ROLE_COMPONENT = "RoleComponent";

            public const string ROLE_ID = "RoleId";
            public const string COMPONENT_ID = "ComponentId";
            public const string ACTIONS = "Actions";
            #endregion

            #region Properties
            [Required]
            public int RoleId { get; protected set; }

            [Required]
            public int ComponentId { get; protected set; }

            [Required]
            [StringLength(250)]
            public string Actions { get; internal set; }

            public Component Component { get; protected set; }

            #endregion

            #region Constructor
            public RoleComponent(int roleId, int componentId, string actions)
            {
                this.RoleId = roleId;
                this.ComponentId = componentId;
                this.Actions = actions;
            }

            public RoleComponent(int roleId, Component component, string actions)
            {
                this.RoleId = roleId;
                this.ComponentId = component.Id;
                this.Component = component;
                this.Actions = actions;
            }

            public RoleComponent(Dictionary<string, object> property_values)
            {
                Component = new Component();

                foreach (KeyValuePair<string, object> property_value in property_values)
                    this[property_value.Key] = property_value.Value;
            }
            #endregion

            #region Infrastructure
            protected override IEnumerable<object> GetAtomicValues()
            {
                yield return this.RoleId;
                yield return this.ComponentId;
                yield return this.Actions;
            }
            #endregion

        }

        #endregion
    }
}
