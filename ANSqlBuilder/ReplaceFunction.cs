using System;
using System.Text;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public class ReplaceFunction:ISqlExpression
    {
        public ISqlExpression Text { get; set; }
        public ISqlExpression OldText { get; set; }
        public ISqlExpression NewText { get; set; }

        public bool IsLiteral
        {
            get { return true; }
        }

        public ReplaceFunction(string text, string old_text, string new_text)
        {
            Text = new SqlLiteral(text);
            OldText = new SqlLiteral(old_text);
            NewText = new SqlLiteral(new_text);
        }

        public ReplaceFunction(ISqlExpression text, string old_text, string new_text)
        {
            Text = text;
            OldText = new SqlLiteral(old_text);
            NewText = new SqlLiteral(new_text);
        }

        public ReplaceFunction(ISqlExpression text, ISqlExpression old_text, string new_text)
        {
            Text = text;
            OldText = old_text;
            NewText = new SqlLiteral(new_text);
        }

        public ReplaceFunction(ISqlExpression text, string old_text, ISqlExpression new_text)
        {
            Text = text;
            OldText = new SqlLiteral(old_text);
            NewText = new_text;
        }

        public ReplaceFunction(ISqlExpression text, ISqlExpression old_text, ISqlExpression new_text)
        {
            Text = text;
            OldText = old_text;
            NewText = new_text;
        }



        public ReplaceFunction(string text, string old_text, ISqlExpression new_text)
        {
            Text = new SqlLiteral(text);
            OldText = new SqlLiteral(old_text);
            NewText = new_text;
        }

        public void GetSql(DbTarget db_target, ref StringBuilder sql)
        {
            sql.Append("REPLACE(");
            if (!Text.IsLiteral)
                sql.Append("(");
            Text.GetSql(db_target, ref sql);
            if (!Text.IsLiteral)
                sql.Append(")");
            sql.Append(",");
            if (!OldText.IsLiteral)
                sql.Append("(");
            OldText.GetSql(db_target, ref sql);
            if (!OldText.IsLiteral)
                sql.Append(")");
            sql.Append(",");
            if (!NewText.IsLiteral)
                sql.Append("(");
            NewText.GetSql(db_target, ref sql);
            if (!NewText.IsLiteral)
                sql.Append(")");
            sql.Append(")");
           
        }
    }
}
