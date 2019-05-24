namespace ANMappings
{
    public delegate string ColumnMappingDefaultFunctionValue();

    public interface IColumnMapping: IPropertyMapping
    {
        string Name { get; }
        string ColumnName { get; }
        object DefaultValue { get;  }
        ColumnMappingDefaultFunctionValue DefaultFunctionValue { get; }
        int MaxLength { get; }
        byte Presicion { get; }
        byte Scale { get; }
        bool IsPrimaryKey { get;  }
        bool IsLabel { get; }
        bool IsDbGenerated { get; }
        bool IsNullable { get;  }
        bool IsVersion { get; }
        bool IsMemo { get; }
        bool IsUnicode { get; }

        IColumnMapping Named(string name);
        IColumnMapping Identity();
        IColumnMapping PrimaryKey();
        IColumnMapping Label();
        IColumnMapping DbGenerated();
        IColumnMapping Nullable();
        IColumnMapping Nullable(object value);
        IColumnMapping Default(object value);
        IColumnMapping DefaultCurrentDateTime();
        IColumnMapping DefaultCurrentUtcDateTime();
        IColumnMapping HasMaxLength(int max_length);
        IColumnMapping HasPrecision(byte presicion, byte scale);
        IColumnMapping Unicode();
        IColumnMapping Version();
        IColumnMapping Memo();
        IColumnMapping Not { get; }

    }
}
