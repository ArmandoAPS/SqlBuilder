using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    [XmlRootAttribute("WhenExpression", IsNullable = false)]
    public class WhenExpression
    {
        private ISqlExpression _BooleanExpression;
        private ISqlExpression _ResultExpression;

        public WhenExpression()
        {
        }

        public WhenExpression(ISqlExpression boolean_expression, ISqlExpression result_expression)
        {
           _BooleanExpression = boolean_expression;
           _ResultExpression = result_expression;
        }

        [XmlElementAttribute]
        public ISqlExpression BooleanExpression
        {
            get { return _BooleanExpression; }
            set { _BooleanExpression = value; }
        }

        [XmlElementAttribute]
        public ISqlExpression ResultExpression
        {
            get { return _ResultExpression; }
            set { _ResultExpression = value; }
        }
    }
}
