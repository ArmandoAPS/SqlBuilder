using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Ecms.Core.Domain.Model
{
    /// <summary>
    /// Base class for Value Objects.
    /// </summary>
    public abstract class ValueObject : IIndexable
    {
        public ValueObject()
        {
        }

        protected ValueObject(Dictionary<string, object> property_values)
        {
            InitializeProperties(property_values);
        }

        protected void InitializeProperties(Dictionary<string, object> property_values)
        {
            foreach (KeyValuePair<string, object> property_value in property_values)
            {
                this[property_value.Key] = property_value.Value;
            }
        }

        public PropertyInfo GetProperty(string name)
        {
            return GetType().GetProperty(name);

        }

        public virtual object this[string name]
        {
            get
            {
                Type myType = GetType();

                if (name.IndexOf(".") > 0)
                {
                    string[] names = name.Split('.');
                    var parentProperty = myType.GetProperty(names[0]).GetValue(this, null);
                    if (parentProperty != null)
                        return parentProperty.GetType().GetProperty(names[1]).GetValue(parentProperty, null);
                    return null;
                }
                return myType.GetProperty(name).GetValue(this, null);
            }

            set
            {
                Type myType = GetType();

                if (name.IndexOf(".") > 0)
                {

                    string[] names = name.Split('.');
                    var parentProperty = myType.GetProperty(names[0]).GetValue(this, null);
                    if (parentProperty != null)
                    {
                        var prop = parentProperty.GetType().GetProperty(names[1]);
                        if (prop != null)
                        {
                            object val = value == DBNull.Value ? String.Empty : value;
                            prop.SetValue(parentProperty, prop.PropertyType.IsEnum ? Enum.Parse(prop.PropertyType, val.ToString(), true) : val, null);
                        }
                    }
                }
                else
                {
                    var prop = myType.GetProperty(name);
                    myType.GetProperty(name).SetValue(this, prop.PropertyType.IsEnum ? Enum.Parse(prop.PropertyType, value.ToString(), true) : value, null);
                }
            }
        }

        /// <summary>
        /// Helper function for implementing overloaded equality operator.
        /// </summary>
        /// <param name="left">Left-hand side object.</param>
        /// <param name="right">Right-hand side object.</param>
        /// <returns></returns>
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        /// <summary>
        /// Helper function for implementing overloaded inequality operator.
        /// </summary>
        /// <param name="left">Left-hand side object.</param>
        /// <param name="right">Right-hand side object.</param>
        /// <returns></returns>
        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !(EqualOperator(left, right));
        }

        /// <summary>
        /// To be overridden in inheriting clesses for providing a collection of atomic values of
        /// this Value Object.
        /// </summary>
        /// <returns>Collection of atomic values.</returns>
        protected abstract IEnumerable<object> GetAtomicValues();

        /// <summary>
        /// Compares two Value Objects according to atomic values returned by <see cref="GetAtomicValues"/>.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>True if objects are considered equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }
            ValueObject other = (ValueObject)obj;
            IEnumerator<object> thisValues = GetAtomicValues().GetEnumerator();
            IEnumerator<object> otherValues = other.GetAtomicValues().GetEnumerator();
            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (ReferenceEquals(thisValues.Current, null) ^ ReferenceEquals(otherValues.Current, null))
                {
                    return false;
                }
                if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current))
                {
                    return false;
                }
            }
            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        /// <summary>
        /// Returns hashcode value calculated according to a collection of atomic values
        /// returned by <see cref="GetAtomicValues"/>.
        /// </summary>
        /// <returns>Hashcode value.</returns>
        public override int GetHashCode()
        {
            return GetAtomicValues()
               .Select(x => x != null ? x.GetHashCode() : 0)
               .Aggregate((x, y) => x ^ y);
        }
    }
}
