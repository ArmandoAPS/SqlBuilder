using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.DataSetAdapters;
using Ecms.Security.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANCommon.Sql;

namespace Ecms.Security.Application.Services
{
    public class SolutionService: ISolutionService
    {
        SolutionRepository _repository;
        public SolutionRepository GetRepository()
        {
            if (_repository == null)
                _repository = new SolutionRepository();
            return _repository;
        }

        public Solution GetById(int id)
        {
            return GetRepository().GetById(id);
        }

        public void Save(Solution solution)
        {
            GetRepository().Save(solution);
        }

        public void Delete(Solution solution)
        {
            GetRepository().Delete(solution);
        }

        public void Remove(Solution solution)
        {
            GetRepository().Remove(solution);
        }


        public Dictionary<int, string> GetShortList() { return GetRepository().GetShortList(); }

        public ANCommon.DataAccess.IDataSetAdapter GetDataSetAdapter() { return new SolutionDataSetAdapter(); }
    }
}
