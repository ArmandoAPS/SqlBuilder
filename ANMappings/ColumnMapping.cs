using System;
using System.Reflection;

namespace ANMappings
{
    public class ColumnMapping: IColumnMapping, IPropertyMapping
    {
        public string Name { get; private set; }
        public string ColumnName { get; private set; }
        public object DefaultValue { get; private set; }
        public ColumnMappingDefaultFunctionValue DefaultFunctionValue { get; private set; }
        public int MaxLength { get; private set; }
        public byte Presicion { get; private set; }
        public byte Scale { get; private set; }
        public bool IsPrimaryKey { get; private set; }
        public bool IsLabel { get; private set; }
        public bool IsDbGenerated { get; private set; }
        public bool IsNullable { get; private set; }
        public bool IsVersion { get; private set; }
        public bool IsMemo { get; private set; }
        public bool IsUnicode { get; private set; }

        private bool _nextBool = true;

        public ColumnMapping(MemberInfo property)
        {
            Property = property;
            Name = Property.Name;
        }

        public ColumnMapping(MemberInfo property, string name)
        {
            Property = property;
            Name = name;
        }

        /// <summary>
        /// The property that is mapped to the database.
        /// </summary>
        public MemberInfo Property { get; private set; }

        public IColumnMapping Identity()
        {
            IsDbGenerated = _nextBool;
            IsPrimaryKey = IsDbGenerated;
            _nextBool = true;
            return this;
        }

        public IColumnMapping Named(string name)
        {
            ColumnName = name;
            return this;
        }

        public IColumnMapping PrimaryKey()
        {
            IsPrimaryKey = _nextBool;
            _nextBool = true;
            return this;
        }

        public IColumnMapping Label()
        {
            IsLabel = _nextBool;
            _nextBool = true;
            return this;
        }

        public IColumnMapping DbGenerated()
        {
            IsDbGenerated = _nextBool;
            _nextBool = true;
            return this;
        }

        public IColumnMapping Nullable()
        {
            IsNullable = _nextBool;
            _nextBool = true;
            return this;
        }

        public IColumnMapping Unicode()
        {
            IsUnicode = _nextBool;
            _nextBool = true;
            return this;
        }

        public IColumnMapping Nullable(object value)
        {
            IsNullable = _nextBool;
            DefaultValue = value;
            _nextBool = true;
            return this;
        }

        public IColumnMapping Default(object value)
        {
            DefaultValue = value;
            return this;
        }

        public IColumnMapping DefaultCurrentDateTime()
        {
            this.DefaultFunctionValue = (ColumnMappingDefaultFunctionValue)(() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return (IColumnMapping)this;
        }

        public IColumnMapping DefaultCurrentUtcDateTime()
        {
            this.DefaultFunctionValue = (ColumnMappingDefaultFunctionValue)(() => DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            return (IColumnMapping)this;
        }

        public IColumnMapping HasMaxLength(int max_length)
        {
            MaxLength = max_length;
            return this;
        }

        public IColumnMapping HasPrecision(byte precision, byte scale)
        {
            Presicion = precision;
            Scale = scale;
            return this;
        }

        public IColumnMapping Version()
        {
            IsVersion = _nextBool;
            _nextBool = true;
            return this;
        }

        public IColumnMapping Memo()
        {
            IsMemo = _nextBool;
            _nextBool = true;
            return this;
        }

        public IColumnMapping Not
        {
            get
            {
                _nextBool = !_nextBool;
                return this;
            }
        }
    }
}
