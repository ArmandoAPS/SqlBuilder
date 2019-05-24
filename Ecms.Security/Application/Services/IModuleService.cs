using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Repositories;
using System.Collections.Generic;

namespace Ecms.Security.Application.Services
{
    public interface IModuleService
    {
        Module GetById(int id);
        void Save(Module module);
        void Delete(Module module);
        void Remove(Module module);
        ANCommon.DataAccess.IDataSetAdapter GetDataSetAdapter();
        ModuleRepository GetRepository();
        Dictionary<int, string> GetShortList();
        string GetPermissionsAsXml(int moduleId);
    }
}
