using Ecms.Core.Domain.Model;
using System.ComponentModel.DataAnnotations;

namespace Ecms.Security.Domain.Model
{
    public class Component: Entity
    {
        #region Constants
        public const string COMPONENT = "Component";

        public const string NAME = "Name";
        public const string DESCRIPTION = "Description";
        public const string URL = "Url";
        public const string SHOW_IN_SECTION = "ShowInSection";
        public const string SECTION_ID = "SectionId";
        public const string DISPLAY_ORDER = "DisplayOrder";
        public const string MODULE_ID = "ModuleId";
        #endregion

        #region Properties
        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        [StringLength(100)]
        public string Url { get; set; }

        public bool ShowInMenu { get; set; }

        public int? MenuId { get; protected set; }

        public int DisplayOrder { get; set; }

        public int ModuleId { get; protected set; }

        private Menu _Menu;
        public Menu Menu
        {
            get { return _Menu; }
            set
            {
                _Menu = value;
                if (value != null)
                {
                    MenuId = value.Id;
                }

            }
        }
        #endregion

        #region Constructors

        public Component()
        {
            
        }

        public Component(int id): base(id)
        {

        }

        public Component(Module module)
        {
            this.ModuleId = module.Id;
        }

        public Component(Module module, Menu menu)
        {
            this.ModuleId = module.Id;
            this.MenuId = menu.Id;
        }

        public Component(int moduleId, int menuId)
        {
            this.ModuleId = moduleId;
            this.MenuId = menuId;
        }
        #endregion
    }
}
