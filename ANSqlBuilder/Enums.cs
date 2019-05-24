using System;
using System.Collections.Generic;
using System.Text;

namespace ANSqlBuilder
{

    [Serializable]
    public enum DbTarget
    {
        SqlServer,
        MySql,        
        SqlLite
    }

    [Serializable]
    public enum DbColumnType
    {
        String,
        Number,
        Date,
        Boolean,
        Binary
    }
    /// <summary>
    /// Enum for General SQL Functions
    /// </summary>
    [Serializable]
    public enum AggregateFunctionType
    {
        Count,
        Sum,
        Avg,
        Min,
        Max
    }

    /// <summary>
    /// SQL Comparison Operators
    /// </summary>
    [Serializable]
    public enum Comparisons
    {
        Equals,
        NotEquals,
        Like,
        GreaterThan,
        GreaterOrEquals,
        LessThan,
        LessOrEquals,
        IsNull,
        Between,
        In
    }

   /// <summary>
    /// SQL Condition Types
    /// </summary>
    [Serializable]
    public enum ConditionType
    {
        None,
        And,
        Or
    }

    /// <summary>
    /// SQL SortType Keywords
    /// </summary>
    [Serializable]
    public enum SortType
    {
         Ascending,
        Descending
    }

    /// <summary>
    /// SQL JoinType Keywords
    /// </summary>
    [Serializable]
    public enum JoinType
    {
        Inner,
        Left,
        Right
    }

    /// <summary>
    /// SQL CombineType Keywords
    /// </summary>
    [Serializable]
    public enum CombineType
    {
        Union,
        UnionAll,
        Intersect,
        Except,
        Minus
    }


    [Serializable]
    public enum DatePart
    {
        Day,
        DayOfWeek,
        DayOfYear,
        Hour,
        Minute,       
        Month,
        Second,
        Week,
        Year,
        Date,
        Time
    }

}
