using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ecms.Core.Domain.Model
{
    public interface IIndexable
    {
        object this[string name] { get; set; }
        PropertyInfo GetProperty(string propertyName);
       
    }
}
