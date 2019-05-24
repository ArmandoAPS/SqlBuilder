using Ecms.Core.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecms.Security.Domain.Model
{
    public class Solution : Entity
    {
        public const string SOLUTION = "Solution";

        public const string NAME = "Name";
        public const string CODE = "Code";
        public const string DESCRIPTION = "Description";
        public const string URL = "Url";
        public const string PROPERTIES = "Properties";

        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Properties { get; set; }
    }
}
