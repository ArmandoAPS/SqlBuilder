using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.DataSetAdapters;
using Ecms.Security.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecms.Security.Application.Services
{

    public class ComponentService: IComponentService
    {
        ComponentRepository _repository;
        public ComponentRepository GetRepository()
        {
            if (_repository == null)
                _repository = new ComponentRepository();
            return _repository;
        }

        public Component GetById(int id)
        {
            return GetRepository().GetById(id);
        }

        public void Save(Component component)
        {
            GetRepository().Save(component);
        }

        public void Delete(Component module)
        {
            GetRepository().Delete(module);
        }

        public void Remove(Component module)
        {
            GetRepository().Remove(module);
        }

        public ANCommon.DataAccess.IDataSetAdapter GetDataSetAdapter() { return new ComponentDataSetAdapter(); }

    }
}
