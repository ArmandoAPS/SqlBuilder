using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class DeleteStatement: NonQueryStatement
    {

        #region Constructor
        public DeleteStatement(string table_name)
        {
            TableName = table_name;
        }
        #endregion

        #region Properties

        public readonly string TableName;
        private ConditionClause _Where;
        

        public override void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            sql.Append("DELETE FROM ");
            sql.Append(Utils.FormatName(TableName, db_target));
            sql.Append(" ");

            if (_Where != null && _Where.Count > 0)
                _Where.GetSql(db_target,ref sql);
        }
        #endregion Properties

        #region Methods


        public DeleteStatement Where(ICondition condition)
        {
            if (_Where == null)
                _Where = new ConditionClause("WHERE");
            _Where.And(condition);
            return this;
        }

        #endregion
    }
}
