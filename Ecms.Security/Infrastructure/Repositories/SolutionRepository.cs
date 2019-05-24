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
    public class SolutionRepository: Repository<Solution, int, SolutionMapping>
    {
        public SolutionRepository()
            : base("Security")
        {
            
        }

        public SolutionRepository(string connection_string_name)
            : base(connection_string_name)
        {
        }

        public Dictionary<int, string> GetShortList()
        {
            var items = new Dictionary<int, string>();

            return GetAll(Condition.IsEqual(Solution.IS_REMOVED, SqlBoolean.FALSE), new OrderByExpression(Solution.NAME, SortType.Ascending))
                .Select(x => new { x.Id, x.Name })
                .ToDictionary(x => x.Id, x => x.Name);
        }
    }
}
