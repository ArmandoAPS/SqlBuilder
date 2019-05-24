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
    public class MenuService: IMenuService
    {
        MenuRepository _repository;
        public MenuRepository GetRepository()
        {
            if (_repository == null)
                _repository = new MenuRepository();
            return _repository;
        }

        public Menu GetById(int id)
        {
            return GetRepository().GetById(id);
        }

        public void Save(Menu menu)
        {
            GetRepository().Save(menu);
        }

        public void Delete(Menu menu)
        {
            GetRepository().Delete(menu);
        }

        public void Remove(Menu menu)
        {
            GetRepository().Remove(menu);
        }

        public Dictionary<int, string> GetShortList(int moduleId) { return GetRepository().GetShortList(moduleId); }

        public ANCommon.DataAccess.IDataSetAdapter GetDataSetAdapter() { return new MenuDataSetAdapter();  }

    }
}
