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
    public abstract class Entity: EntityBase,  IEntity<int>
    {


        public int Id { get; set; }
    

        public Entity():base()
        {
            Id = 0;
        }

        public Entity(int id)
        {

           this.Id = id;
           this.IsRemoved = false;
        }
    }
}
