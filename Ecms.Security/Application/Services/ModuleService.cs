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
    public class ModuleService: IModuleService
    {
        ModuleRepository _repository;
        public ModuleRepository GetRepository()
        {
            if (_repository == null)
                _repository = new ModuleRepository();
            return _repository;
        }

        public Module GetById(int id)
        {
            return GetRepository().GetById(id);
        }

        public void Save(Module module)
        {
            GetRepository().Save(module);
        }

        public void Delete(Module module)
        {
            GetRepository().Delete(module);
        }

        public void Remove(Module module)
        {
            GetRepository().Remove(module);
        }

        public string GetPermissionsAsXml(int moduleId) { return GetRepository().GetPermissionsAsXml(moduleId); }

        public Dictionary<int, string> GetShortList() { return GetRepository().GetShortList(); }

        public ANCommon.DataAccess.IDataSetAdapter GetDataSetAdapter() { return new ModuleDataSetAdapter(); }
    }
}
