using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecms.Core.Domain.Model;
using System.ComponentModel.DataAnnotations;

namespace Ecms.Security.Domain.Model
{
    public class Menu: Entity
    {
        #region Constants
        public const string MENU = "Menu";

        public const string NAME = "Name";
        public const string DESCRIPTION = "Description";
        public const string URL = "Url";
        public const string DISPLAY_ORDER = "DisplayOrder";
        public const string MODULE_ID = "ModuleId";
   

        #endregion

        #region Properties
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        [StringLength(100)]
        public string Url { get; set; }

        public int DisplayOrder { get; set; }

        public int ModuleId { get; protected set; }


        #endregion

        #region Constructors
        public Menu()
        {
        }

        public Menu(int id)
        {
            this.Id = id;
        }

        public Menu(Module module)
        {
            this.ModuleId = module.Id;
        }
        #endregion

    }
}
