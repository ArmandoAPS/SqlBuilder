using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Repositories;
using System.Collections.Generic;

namespace Ecms.Security.Application.Services
{
    public interface ISolutionService
    {
        Solution GetById(int id);
        void Save(Solution module);
        void Delete(Solution module);
        void Remove(Solution module);
        ANCommon.DataAccess.IDataSetAdapter GetDataSetAdapter();
        SolutionRepository GetRepository();
        Dictionary<int, string> GetShortList();
    }
}
