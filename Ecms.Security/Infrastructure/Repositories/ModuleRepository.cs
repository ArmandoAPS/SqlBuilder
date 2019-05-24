using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANMappings;
using Ecms.Core.Infrastructure;
using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Mappings;
using ANSqlBuilder;
using ANCommon.Sql;
using System.Xml;
using System.IO;

namespace Ecms.Security.Infrastructure.Repositories
{
    public class ModuleRepository: Repository<Module, int, ModuleMapping>
    {
        public ModuleRepository()
            : base("Security")
        {
            
        }

        public ModuleRepository(string connection_string_name)
            : base(connection_string_name)
        {
        }

        public Dictionary<int, string> GetShortList()
        {
            var items = new Dictionary<int, string>();

            return GetAll(Condition.IsEqual(Module.IS_REMOVED, SqlBoolean.FALSE), new OrderByExpression(Module.NAME, SortType.Ascending))
                .Select(x => new { x.Id, x.Name })
                .ToDictionary(x => x.Id, x => x.Name);
        }

        public string GetPermissionsAsXml(int moduleId)
        {
            var valTrue = new SqlBoolean(true);

            var cndModuleJoinSection = Condition.IsEqual(ModuleMapping.COLUMN_ID, MenuMapping.COLUMN_MODULE_ID);
            var cndSectionJoinComponent = Condition.IsEqual(MenuMapping.COLUMN_ID, ComponentMapping.COLUMN_MENU_ID);

            var cndModuleIsActive = Condition.IsEqual(ModuleMapping.COLUMN_IS_ACTIVE, valTrue);
            var cndSectionIsActive = Condition.IsEqual(MenuMapping.COLUMN_IS_ACTIVE, valTrue);
            var cndComponentIsActive = Condition.IsEqual(ComponentMapping.COLUMN_IS_ACTIVE, valTrue);
            var cndModuleFilter = Condition.IsEqual(ModuleMapping.COLUMN_ID, new SqlNumber(moduleId));


            var queryModules = (new SelectStatement(true))
                .Column("1", "Tag")
                .Column(SqlLiteral.Null, "Parent")
                .Column(ModuleMapping.COLUMN_ID, "Module!1!Id")
                .Column(ModuleMapping.COLUMN_CODE, "Module!1!Code")
                .Column(ModuleMapping.COLUMN_NAME, "Module!1!Name")
                .Column(ModuleMapping.COLUMN_URL, "Module!1!Url")
                .Column(ModuleMapping.COLUMN_DISPLAY_ORDER, "Module!1!DisplayOrder")
                .Column(SqlLiteral.Null, "Section!2!Id")
                .Column(SqlLiteral.Null, "Section!2!Name")
                .Column(SqlLiteral.Null, "Section!2!DisplayOrder")
                .Column(SqlLiteral.Null, "Section!2!Url")
                .Column(SqlLiteral.Null, "Component!3!Id")
                .Column(SqlLiteral.Null, "Component!3!Name")
                .Column(SqlLiteral.Null, "Component!3!Code")
                .Column(SqlLiteral.Null, "Component!3!Url")
                .Column(SqlLiteral.Null, "Component!3!DisplayOrder")
                .Column(SqlLiteral.Null, "Component!3!Actions")
                .From(ModuleMapping.TABLE_NAME)
                .InnerJoin(MenuMapping.TABLE_NAME, cndModuleJoinSection)
                .InnerJoin(ComponentMapping.TABLE_NAME, cndSectionJoinComponent)
                .Where(cndModuleIsActive)
                .Where(cndSectionIsActive)
                .Where(cndComponentIsActive)
                .Where(cndModuleFilter);

            var querySection = (new SelectStatement(true))
                .Column("2", "Tag")
                .Column("1", "Parent")
                .Column(MenuMapping.COLUMN_MODULE_ID, "Module!1!Id")
                .Column(SqlLiteral.Null, "Module!1!Code")
                .Column(SqlLiteral.Null, "Module!1!Name")
                .Column(SqlLiteral.Null, "Module!1!Url")
                .Column(SqlLiteral.Null, "Module!1!DisplayOrder")
                .Column(MenuMapping.COLUMN_ID, "Section!2!Id")
                .Column(MenuMapping.COLUMN_NAME, "Section!2!Name")
                .Column(MenuMapping.COLUMN_DISPLAY_ORDER, "Section!2!DisplayOrder")
                .Column(MenuMapping.COLUMN_URL, "Section!2!Url")
                .Column(SqlLiteral.Null, "Component!3!Id")
                .Column(SqlLiteral.Null, "Component!3!Name")
                .Column(SqlLiteral.Null, "Component!3!Code")
                .Column(SqlLiteral.Null, "Component!3!Url")
                .Column(SqlLiteral.Null, "Component!3!DisplayOrder")
                .Column(SqlLiteral.Null, "Component!3!Actions")
                .From(ModuleMapping.TABLE_NAME)
                .InnerJoin(MenuMapping.TABLE_NAME, cndModuleJoinSection)
                .InnerJoin(ComponentMapping.TABLE_NAME, cndSectionJoinComponent)
                .Where(cndModuleIsActive)
                .Where(cndSectionIsActive)
                .Where(cndComponentIsActive)
                .Where(cndModuleFilter);

            var queryComponents = (new SelectStatement(true))
                .Column("3", "Tag")
                .Column("2", "Parent")
                .Column(ComponentMapping.COLUMN_MODULE_ID, "Module!1!Id")
                .Column(SqlLiteral.Null, "Module!1!Code")
                .Column(SqlLiteral.Null, "Module!1!Name")
                .Column(SqlLiteral.Null, "Module!1!Url")
                .Column(SqlLiteral.Null, "Module!1!DisplayOrder")
                .Column(ComponentMapping.COLUMN_MENU_ID, "Section!2!Id")
                .Column(SqlLiteral.Null, "Section!2!Name")
                .Column(SqlLiteral.Null, "Section!2!DisplayOrder")
                .Column(SqlLiteral.Null, "Section!2!Url")
                .Column(ComponentMapping.COLUMN_ID, "Component!3!Id")
                .Column(ComponentMapping.COLUMN_NAME, "Component!3!Name")
                .Column(ComponentMapping.COLUMN_CODE, "Component!3!Code")
                .Column(ComponentMapping.COLUMN_URL, "Component!3!Url")
                .Column(ComponentMapping.COLUMN_DISPLAY_ORDER, "Component!3!DisplayOrder")
                .Column(new SqlString("NNNN"), "Component!3!Actions")
                .From(ModuleMapping.TABLE_NAME)
                .InnerJoin(MenuMapping.TABLE_NAME, cndModuleJoinSection)
                .InnerJoin(ComponentMapping.TABLE_NAME, cndSectionJoinComponent)
                .Where(cndModuleIsActive)
                .Where(cndSectionIsActive)
                .Where(cndComponentIsActive)
                .Where(cndModuleFilter)
                .OrderBy(new SqlName("Module!1!Id"), SortType.Ascending)
                .OrderBy(new SqlName("Section!2!Id"), SortType.Ascending)
                .OrderBy(new SqlName("Component!3!Id"), SortType.Ascending)
                .OrderBy(new SqlName("Component!3!DisplayOrder"), SortType.Ascending)
                ;

            var query = (new SelectStatementCombine(CombineType.UnionAll));
            query.ConnectionStringName = GetConnectionStringName();
            query.Add(queryModules);
            query.Add(querySection);
            query.Add(queryComponents);

            var ms = new MemoryStream();
            query.WriteXmlExplicit(ms);
            ms.Position = 0;

            var xmlstring = new System.Text.StringBuilder();
            xmlstring.Append("<?xml version='1.0' encoding='UTF-8'?>");
            xmlstring.Append("<navigation>");
            xmlstring.Append((new StreamReader(ms)).ReadToEnd());
            xmlstring.Append("</navigation>");

            //query.GetSql(DbTarget.MySql, ref xmlstring);

            return xmlstring.ToString();
        }
    }
}
