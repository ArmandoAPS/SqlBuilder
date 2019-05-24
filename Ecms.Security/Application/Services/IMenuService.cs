using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Repositories;
using System.Collections.Generic;

namespace Ecms.Security.Application.Services
{
    public interface IMenuService
    {
        Menu GetById(int id);
        void Save(Menu group);
        void Delete(Menu group);
        void Remove(Menu group);
        ANCommon.DataAccess.IDataSetAdapter GetDataSetAdapter();
        MenuRepository GetRepository();
        Dictionary<int, string> GetShortList(int moduleId);
    }
}
