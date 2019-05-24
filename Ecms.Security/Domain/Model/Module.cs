using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecms.Core.Domain.Model;
using System.ComponentModel.DataAnnotations;

namespace Ecms.Security.Domain.Model
{
    public class Module : Entity
    {
        #region Constants
        public const string MODULE = "Module";

        public const string CODE = "Code";
        public const string NAME = "Name";
        public const string DESCRIPTION = "Description";
        public const string URL = "Url";
        public const string DISPLAY_ORDER = "DisplayOrder";
        public const string SOLUTION_ID = "SolutionId";

        #endregion

        #region Properties

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        [StringLength(100)]
        public string Url { get; set; }

        public int DisplayOrder { get; set; }

        public int SolutionId { get; set; }

        public Solution Solution { get; set; }

        #endregion

        #region Constructors

        public Module()
        {

        }

        public Module(int id)
        {
            this.Id = id;
        }

        #endregion

    }
}