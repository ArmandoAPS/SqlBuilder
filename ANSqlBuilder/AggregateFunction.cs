using System;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class AggregateFunction : ISqlExpression
    {
        protected AggregateFunctionType _aggregateFunctionType;
        protected ISqlExpression _expression = null;



        public AggregateFunction(AggregateFunctionType aggregate_function_type, ISqlExpression expression)
        {
            _aggregateFunctionType = aggregate_function_type;
            _expression = expression;
        }

        public AggregateFunction(AggregateFunctionType aggregate_function_type, string column_name)
        {
            _aggregateFunctionType = aggregate_function_type;
            _expression = new SqlColumnName(column_name);
        }

        public bool IsLiteral
        {
            get { return true; }
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {

            /*switch (_aggregateFunctionType)
            {
                case AggregateFunctionType.Sum:
                    sql.Append("SUM");
                    break;

                case AggregateFunctionType.Avg:
                    sql.Append("AVG");
                    break;

                case AggregateFunctionType.Max:
                    sql.Append("MAX");
                    break;

                case AggregateFunctionType.Min:
                    sql.Append("MIN");
                    break;

                case AggregateFunctionType.Count:
                    sql.Append("COUNT");
                    break;

            }*/

            sql.Append(_aggregateFunctionType.ToString());
            sql.Append("(");
            if (_expression != null)
            {
                if (!_expression.IsLiteral)
                    sql.Append("(");
                _expression.GetSql(db_target, ref sql);
                if (!_expression.IsLiteral)
                    sql.Append(")");
            }
            else
            {
                if(_aggregateFunctionType == AggregateFunctionType.Count)
                {
                    sql.Append("*");
                }
            }
            sql.Append(")");

        }
    }
}
