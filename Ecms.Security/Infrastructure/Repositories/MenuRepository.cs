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

namespace Ecms.Security.Infrastructure.Repositories
{
    public class MenuRepository: Repository<Menu, int, MenuMapping>
    {
        public MenuRepository()
            : base("Security")
        {
            
        }

        public MenuRepository(string connection_string_name)
            : base(connection_string_name)
        {
        }

        public Dictionary<int, string> GetShortList(int moduleId)
        {
            var items = new Dictionary<int, string>();

            return GetAll(Conditions.IsEqual(Menu.IS_REMOVED, SqlBoolean.FALSE).AndIsEqual(Menu.MODULE_ID, new SqlNumber(moduleId)), new OrderByExpression(Module.NAME, SortType.Ascending))
                .Select(x => new { x.Id, x.Name })
                .ToDictionary(x => x.Id, x => x.Name);
        }
    }
}
