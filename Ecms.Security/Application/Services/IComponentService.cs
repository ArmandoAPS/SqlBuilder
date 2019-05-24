using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Repositories;

namespace Ecms.Security.Application.Services
{
    public interface IComponentService
    {
        Component GetById(int id);
        void Save(Component module);
        void Delete(Component module);
        void Remove(Component module);
        ANCommon.DataAccess.IDataSetAdapter GetDataSetAdapter();
        ComponentRepository GetRepository();
    }
}
