using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using ANMappings;
using ANSqlBuilder;
using Ecms.Core.Infrastructure;
using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Mappings;
using ANCommon.Sql;

namespace Ecms.Security.Infrastructure.Repositories
{
    public class UserRepository: Repository<User, int, UserMapping>
    {
        public UserRepository()
            : base("Security")
        {
            
        }

        public UserRepository(string connection_string_name)
            : base(connection_string_name)
        {
        }

        public User GetUserById(int id)
        {
            return GetOne
                (
                    Conditions.IsEqual(User.ID, new SqlNumber(id))
                    .AndIsEqual(User.IS_REMOVED, SqlBoolean.FALSE)
                );
        }

        public User GetUserByUserName(string userName)
        {
            return GetOne
                (
                    Conditions.IsEqual(User.USER_NAME, new SqlString(userName))
                    .AndIsEqual(User.IS_REMOVED, SqlBoolean.FALSE)
                );
        }

        public User GetUserByEmail(string email)
        {
            return GetOne
                (
                    Conditions.IsEqual(User.EMAIL, new SqlString(email))
                    .AndIsEqual(User.IS_REMOVED, SqlBoolean.FALSE)
                );
        }

        public Dictionary<int, string> GetShortList()
        {
            var items = new Dictionary<int, string>();

            return GetAll(Condition.IsEqual(User.IS_REMOVED, SqlBoolean.FALSE), new OrderByExpression(User.NAME, SortType.Ascending))
                .Select(x => new { x.Id, x.Name })
                .ToDictionary(x => x.Id, x => x.Name);
        }

        public Dictionary<string, string> GetShortList2()
        {
            var items = new Dictionary<string, string>();

            return GetAll(Condition.IsEqual(User.IS_REMOVED, SqlBoolean.FALSE), new OrderByExpression(User.NAME, SortType.Ascending))
                .Select(x => new { x.UserName, x.Name })
                .ToDictionary(x => x.UserName, x => x.Name);
        }

        public bool SetPassword(string userName, string oldPassword, string newPassword)
        {
            var user = GetUserByUserName(userName);
            if (user == null)
                return false;

            if(user.Password == oldPassword)
            {
                user.Password = newPassword;
                Save(user);
                return true;
            }
            return false;
        }

        public string GetNavigationAsXml(string[] solutionCode, string userName)
        {
            var valTrue = new SqlBoolean(true);
           
            var cndUserJoinUserRole = Condition.IsEqual(UserMapping.COLUMN_ID, UserRoleMapping.COLUMN_USER_ID);
            var cndUserRoleJoinRole = Condition.IsEqual(UserRoleMapping.COLUMN_ROLE_ID, RoleMapping.COLUMN_ID);
            var cndRoleJoinRoleComponent = Condition.IsEqual(RoleMapping.COLUMN_ID, RoleComponentMapping.COLUMN_ROLE_ID);
            var cndRoleComponentJoinComponent = Condition.IsEqual(RoleComponentMapping.COLUMN_COMPONENT_ID, ComponentMapping.COLUMN_ID);

            var cndComponentJoinSection = Condition.IsEqual(MenuMapping.COLUMN_ID, ComponentMapping.COLUMN_MENU_ID);
            var cndSectionJoinModule = Condition.IsEqual(MenuMapping.COLUMN_MODULE_ID, ModuleMapping.COLUMN_ID);
            var cndModuleJoinSolution = Condition.IsEqual(ModuleMapping.COLUMN_SOLUTION_ID, SolutionMapping.COLUMN_ID);

            var cndModuleIsActive = Condition.IsEqual(ModuleMapping.COLUMN_IS_ACTIVE, valTrue);
            var cndSectionIsActive = Condition.IsEqual(MenuMapping.COLUMN_IS_ACTIVE, valTrue);
            var cndComponentIsActive = Conditions.IsEqual(ComponentMapping.COLUMN_IS_ACTIVE, valTrue);
            var cndRoleIsActive = Condition.IsEqual(RoleMapping.COLUMN_IS_ACTIVE, valTrue);
            var cndUserIsActive = Condition.IsEqual(UserMapping.COLUMN_IS_ACTIVE, valTrue);
            var cndSolution = Condition.IsIn(SolutionMapping.COLUMN_CODE, solutionCode);

           
            var cndUserFilter = Condition.IsEqual(UserMapping.COLUMN_USER_NAME, new SqlString(userName));
            var cndActionFilter = Condition.IsNotEqual(RoleComponentMapping.COLUMN_ACTIONS,new SqlString("NNNN"));
            
            var queryModules = (new SelectStatement(true))
                .Column("1", "Tag")
                .Column(SqlLiteral.Null, "Parent")
                .Column(ModuleMapping.COLUMN_ID, "Module!1!Id")
                .Column(ModuleMapping.COLUMN_CODE, "Module!1!Code")
                .Column(ModuleMapping.COLUMN_NAME, "Module!1!Name")
                .Column(ModuleMapping.COLUMN_URL, "Module!1!Url")
                .Column(ModuleMapping.COLUMN_DISPLAY_ORDER, "Module!1!DisplayOrder")
                .Column(SqlLiteral.Null, "Menu!2!Id")
                .Column(SqlLiteral.Null, "Menu!2!Name")
                .Column(SqlLiteral.Null, "Menu!2!DisplayOrder")
                .Column(SqlLiteral.Null, "Menu!2!Url")
                .Column(SqlLiteral.Null, "Component!3!Id")
                .Column(SqlLiteral.Null, "Component!3!Name")
                .Column(SqlLiteral.Null, "Component!3!Code")
                .Column(SqlLiteral.Null, "Component!3!Url")
                .Column(SqlLiteral.Null, "Component!3!DisplayOrder")
                .Column(SqlLiteral.Null, "Component!3!ShowInMenu")
                .Column(SqlLiteral.Null, "Component!3!Actions")
                .From(UserMapping.TABLE_NAME)
                .InnerJoin(UserRoleMapping.TABLE_NAME, cndUserJoinUserRole)
                .InnerJoin(RoleMapping.TABLE_NAME, cndUserRoleJoinRole)
                .InnerJoin(RoleComponentMapping.TABLE_NAME, cndRoleJoinRoleComponent)
                .InnerJoin(ComponentMapping.TABLE_NAME, cndRoleComponentJoinComponent)
                .InnerJoin(MenuMapping.TABLE_NAME, cndComponentJoinSection)
                .InnerJoin(ModuleMapping.TABLE_NAME, cndSectionJoinModule)
                .InnerJoin(SolutionMapping.TABLE_NAME, cndModuleJoinSolution)
                .Where(cndSolution)
                .Where(cndModuleIsActive)
                .Where(cndSectionIsActive)
                .Where(cndComponentIsActive)
                .Where(cndRoleIsActive)
                .Where(cndUserIsActive)
                .Where(cndUserFilter)
                .Where(cndActionFilter);

            var querySection = (new SelectStatement(true))
                .Column("2", "Tag")
                .Column("1", "Parent")
                .Column(MenuMapping.COLUMN_MODULE_ID, "Module!1!Id")
                .Column(SqlLiteral.Null, "Module!1!Code")
                .Column(SqlLiteral.Null, "Module!1!Name")
                .Column(SqlLiteral.Null, "Module!1!Url")
                .Column(ModuleMapping.COLUMN_DISPLAY_ORDER, "Module!1!DisplayOrder")
                .Column(MenuMapping.COLUMN_ID, "Menu!2!Id")
                .Column(MenuMapping.COLUMN_NAME, "Menu!2!Name")
                .Column(MenuMapping.COLUMN_DISPLAY_ORDER, "Menu!2!DisplayOrder")
                .Column(MenuMapping.COLUMN_URL, "Menu!2!Url")
                .Column(SqlLiteral.Null, "Component!3!Id")
                .Column(SqlLiteral.Null, "Component!3!Name")
                .Column(SqlLiteral.Null, "Component!3!Code")
                .Column(SqlLiteral.Null, "Component!3!Url")
                .Column(SqlLiteral.Null, "Component!3!DisplayOrder")
                .Column(SqlLiteral.Null, "Component!3!ShowInMenu")
                .Column(SqlLiteral.Null, "Component!3!Actions")
                .From(UserMapping.TABLE_NAME)
                .InnerJoin(UserRoleMapping.TABLE_NAME, cndUserJoinUserRole)
                .InnerJoin(RoleMapping.TABLE_NAME, cndUserRoleJoinRole)
                .InnerJoin(RoleComponentMapping.TABLE_NAME, cndRoleJoinRoleComponent)
                .InnerJoin(ComponentMapping.TABLE_NAME, cndRoleComponentJoinComponent)
                .InnerJoin(MenuMapping.TABLE_NAME, cndComponentJoinSection)
                .InnerJoin(ModuleMapping.TABLE_NAME, cndSectionJoinModule)
                .InnerJoin(SolutionMapping.TABLE_NAME, cndModuleJoinSolution)
                .Where(cndSolution)
                .Where(cndModuleIsActive)
                .Where(cndSectionIsActive)
                .Where(cndComponentIsActive)
                .Where(cndRoleIsActive)
                .Where(cndUserIsActive)
                .Where(cndUserFilter)
                .Where(cndActionFilter);

            var queryComponents = (new SelectStatement(true))
                .Column("3", "Tag")
                .Column("2", "Parent")
                .Column(ComponentMapping.COLUMN_MODULE_ID, "Module!1!Id")
                .Column(SqlLiteral.Null, "Module!1!Code")
                .Column(SqlLiteral.Null, "Module!1!Name")
                .Column(SqlLiteral.Null, "Module!1!Url")
                .Column(ModuleMapping.COLUMN_DISPLAY_ORDER, "Module!1!DisplayOrder")
                .Column(ComponentMapping.COLUMN_MENU_ID, "Menu!2!Id")
                .Column(SqlLiteral.Null, "Menu!2!Name")
                .Column(MenuMapping.COLUMN_DISPLAY_ORDER, "Menu!2!DisplayOrder")
                .Column(SqlLiteral.Null, "Menu!2!Url")
                .Column(ComponentMapping.COLUMN_ID, "Component!3!Id")
                .Column(ComponentMapping.COLUMN_NAME, "Component!3!Name")
                .Column(ComponentMapping.COLUMN_CODE, "Component!3!Code")
                .Column(ComponentMapping.COLUMN_URL, "Component!3!Url")
                .Column(ComponentMapping.COLUMN_DISPLAY_ORDER, "Component!3!DisplayOrder")
                .Column(ComponentMapping.COLUMN_SHOW_IN_MENU, "Component!3!ShowInMenu")
                .Column(new IsNullFunction(RoleComponentMapping.COLUMN_ACTIONS, new SqlString("NNNN")), "Component!3!Actions")
                .From(UserMapping.TABLE_NAME)
                .InnerJoin(UserRoleMapping.TABLE_NAME, cndUserJoinUserRole)
                .InnerJoin(RoleMapping.TABLE_NAME, cndUserRoleJoinRole)
                .InnerJoin(RoleComponentMapping.TABLE_NAME, cndRoleJoinRoleComponent)
                .InnerJoin(ComponentMapping.TABLE_NAME, cndRoleComponentJoinComponent)
                .InnerJoin(MenuMapping.TABLE_NAME, cndComponentJoinSection)
                .InnerJoin(ModuleMapping.TABLE_NAME, cndSectionJoinModule)
                .InnerJoin(SolutionMapping.TABLE_NAME, cndModuleJoinSolution)
                .Where(cndSolution)
                .Where(cndModuleIsActive)
                .Where(cndSectionIsActive)
                .Where(cndComponentIsActive)
                .Where(cndRoleIsActive)
                .Where(cndUserIsActive)
                .Where(cndUserFilter)
                .Where(cndActionFilter)
                .OrderBy(new SqlName("Module!1!DisplayOrder"), SortType.Ascending)
                .OrderBy(new SqlName("Module!1!Id"), SortType.Ascending)
                .OrderBy(new SqlName("Menu!2!DisplayOrder"), SortType.Ascending)
                .OrderBy(new SqlName("Menu!2!Id"), SortType.Ascending)
                .OrderBy(new SqlName("Component!3!DisplayOrder"), SortType.Ascending)
                .OrderBy(new SqlName("Component!3!Id"), SortType.Ascending);


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


 /*       public static string GetPermissionsAsXml(string userName, bool full)
        {
            var strActivo = new SqlString("1");
            var strNoPermissions = new SqlString("NNNN");
            ICondition condModuleActive = Condition.IsEqual(ModuleMapping.COLUMN_IS_ACTIVE, strActivo);
            ICondition condSectionActive = Condition.IsEqual(SectionMapping.COLUMN_IS_ACTIVE, strActivo);
            ICondition condFormActive = Condition.IsEqual(FormMapping.COLUMN_IS_ACTIVE, strActivo);
            ICondition condModuleSection = Condition.IsEqual(ModuleMapping.COLUMN_ID, SectionMapping.COLUMN_MODULE_ID);
            ICondition condSectionForm = Condition.IsEqual(SectionMapping.COLUMN_ID, FormMapping.COLUMN_MENU_ID);
            ICondition condFormPermission = Condition.IsEqual(FormMapping.COLUMN_ID, PermissionMapping.COLUMN_FORM_ID);
            ICondition condPermissionRoleId = Condition.IsEqual(PermissionMapping.COLUMN_ROLE_ID, new SqlNumber(role_id));
            ISqlExpression IsNullPermissionMatrix = new IsNullFunction(PermissionMapping.COLUMN_MATRIX, strNoPermissions);

            var query = (new SelectStatementCombine(CombineType.UnionAll));

            var queryModules = (new SelectStatement(true))
                .Column("1", "Tag")
                .Column(SqlLiteral.Null, "Parent")
                .Column(ModuleMapping.COLUMN_ID, "module!1!id")
                .Column(ModuleMapping.COLUMN_NAME, "module!1!name")
                .Column(ModuleMapping., "module!1!key")
                .Column(ModuleMapping.COLUMN_URL, "module!1!url")
                .Column(ModuleMapping.COLUMN_ORDER, "module!1!order")
                .Column(SqlLiteral.Null, "section!2!id")
                .Column(SqlLiteral.Null, "section!2!name")
                .Column(SqlLiteral.Null, "section!2!key")
                .Column(SqlLiteral.Null, "section!2!order")
                .Column(SqlLiteral.Null, "section!2!url")
                .Column(SqlLiteral.Null, "section!2!status")
                .Column(SqlLiteral.Null, "form!3!id")
                .Column(SqlLiteral.Null, "form!3!name")
                .Column(SqlLiteral.Null, "form!3!key")
                .Column(SqlLiteral.Null, "form!3!url")
                .Column(SqlLiteral.Null, "form!3!type")
                .Column(SqlLiteral.Null, "form!3!order")
                .Column(SqlLiteral.Null, "form!3!status")
                .Column(SqlLiteral.Null, "form!3!permissions")
                .From(ModuleMapping.TABLE_NAME)
                .InnerJoin(SectionMapping.TABLE_NAME, SectionMapping.TABLE_NAME, condModuleSection)
                .InnerJoin(FormMapping.TABLE_NAME, FormMapping.TABLE_NAME, condSectionForm)
                .LeftJoin(PermissionMapping.TABLE_NAME, PermissionMapping.TABLE_NAME, condFormPermission)
                .Where(condModuleActive)
                .Where(condSectionActive)
                .Where(condFormActive);

            if (!full)
            {
                queryModules.Where(condPermissionRoleId)
                    .WhereIsNotEqual(IsNullPermissionMatrix, strNoPermissions);

            }

            query.Add(queryModules);


            var querySections = (new SelectStatement(true))
                .Column("2", "Tag")
                .Column("1", "Parent")
                .Column(ModuleMapping.COLUMN_ID, "module!1!id")
                .Column(SqlLiteral.Null, "module!1!name")
                .Column(SqlLiteral.Null, "module!1!key")
                .Column(SqlLiteral.Null, "module!1!url")
                .Column(ModuleMapping.COLUMN_ORDER, "module!1!order")
                .Column(SectionMapping.COLUMN_ID, "section!2!id")
                .Column(SectionMapping.COLUMN_NAME, "section!2!name")
                .Column(SectionMapping.COLUMN_KEY, "section!2!key")
                .Column(SectionMapping.COLUMN_ORDER, "section!2!order")
                .Column(SectionMapping.COLUMN_URL, "section!2!url")
                .Column(SectionMapping.COLUMN_STATUS, "section!2!status")
                .Column(SqlLiteral.Null, "form!3!id")
                .Column(SqlLiteral.Null, "form!3!name")
                .Column(SqlLiteral.Null, "form!3!key")
                .Column(SqlLiteral.Null, "form!3!url")
                .Column(SqlLiteral.Null, "form!3!type")
                .Column(SqlLiteral.Null, "form!3!order")
                .Column(SqlLiteral.Null, "form!3!status")
                .Column(SqlLiteral.Null, "form!3!permissions")
                .From(ModuleMapping.TABLE_NAME)
                .InnerJoin(SectionMapping.TABLE_NAME, SectionMapping.TABLE_NAME, condModuleSection)
                .InnerJoin(FormMapping.TABLE_NAME, FormMapping.TABLE_NAME, condSectionForm)
                .LeftJoin(PermissionMapping.TABLE_NAME, PermissionMapping.TABLE_NAME, condFormPermission)
                .Where(condModuleActive)
                .Where(condSectionActive)
                .Where(condFormActive);

            if (!full)
            {
                querySections.Where(condPermissionRoleId)
                    .WhereIsNotEqual(IsNullPermissionMatrix, strNoPermissions);

            }

            query.Add(querySections);

            var queryForms = (new SelectStatement(true))
                .Column("3", "Tag")
                .Column("2", "Parent")
                .Column(ModuleMapping.COLUMN_ID, "module!1!id")
                .Column(SqlLiteral.Null, "module!1!name")
                .Column(SqlLiteral.Null, "module!1!key")
                .Column(SqlLiteral.Null, "module!1!url")
                .Column(ModuleMapping.COLUMN_ORDER, "module!1!order")
                .Column(SectionMapping.COLUMN_ID, "section!2!id")
                .Column(SqlLiteral.Null, "section!2!name")
                .Column(SqlLiteral.Null, "section!2!key")
                .Column(SectionMapping.COLUMN_ORDER, "section!2!order")
                .Column(SqlLiteral.Null, "section!2!url")
                .Column(SqlLiteral.Null, "section!2!status")
                .Column(FormMapping.COLUMN_ID, "form!3!id")
                .Column(FormMapping.COLUMN_NAME, "form!3!name")
                .Column(FormMapping.COLUMN_KEY, "form!3!key")
                .Column(FormMapping.COLUMN_URL, "form!3!url")
                .Column(FormMapping.COLUMN_IS_SUBFORM, "form!3!type")
                .Column(FormMapping.COLUMN_ORDER, "form!3!order")
                .Column(FormMapping.COLUMN_STATUS, "form!3!status")
                .Column(IsNullPermissionMatrix, "form!3!permissions")
                .From(ModuleMapping.TABLE_NAME)
                .InnerJoin(SectionMapping.TABLE_NAME, SectionMapping.TABLE_NAME, condModuleSection)
                .InnerJoin(FormMapping.TABLE_NAME, FormMapping.TABLE_NAME, condSectionForm)
                .LeftJoin(PermissionMapping.TABLE_NAME, PermissionMapping.TABLE_NAME, condFormPermission)
                .Where(condModuleActive)
                .Where(condSectionActive)
                .Where(condFormActive)
                .OrderBy(new SqlName("module!1!order"), SortType.Ascending)
                .OrderBy(new SqlName("module!1!id"), SortType.Ascending)
                .OrderBy(new SqlName("section!2!order"), SortType.Ascending)
                .OrderBy(new SqlName("section!2!id"), SortType.Ascending)
                .OrderBy(new SqlName("form!3!order"), SortType.Ascending)
                .OrderBy(new SqlName("form!3!id"), SortType.Ascending);

            if (!full)
            {
                queryForms.Where(condPermissionRoleId)
                    .WhereIsNotEqual(IsNullPermissionMatrix, strNoPermissions);

            }

            query.Add(queryForms);

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
        */
    }
}
