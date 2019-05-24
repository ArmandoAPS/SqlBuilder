using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ecms.Core.Domain.Model
{
    public interface IEntity<T>: IIndexable
    {
        T Id { get; set; }
        void AddValueObject(string property_name, Dictionary<string, object> values);
    }
}
