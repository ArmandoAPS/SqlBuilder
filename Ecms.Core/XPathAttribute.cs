using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecms.Core
{
    public class XPathAttribute : Attribute
    {
        public XPathAttribute(string PathExpression)
        {
            this.PathExpression = PathExpression;
        }

        public XPathAttribute(String PathExpression, string NotMatchValue)
        {
            this.PathExpression = PathExpression;
            this.NotMatchValue = NotMatchValue;
        }

        public string PathExpression { get; private set; }
        public string NotMatchValue { get; private set; }
    }

}
