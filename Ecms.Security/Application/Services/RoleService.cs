using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Repositories;
using Ecms.Security.Infrastructure.DataSetAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANCommon.Sql;

namespace Ecms.Security.Application.Services
{
    public interface IRoleService
    {
        Role GetById(int id);
        void Save(Role group);
        void Delete(Role group);
        void Remove(Role group);
        ANCommon.DataAccess.IDataSetAdapter GetDataSetAdapter();
        RoleRepository GetRepository();
        Dictionary<int, string> GetShortList();
        string GetPermissionsAsXml(int roleId);
    }

    public class RoleService: IRoleService
    {
        RoleRepository _repository;
        public RoleRepository GetRepository()
        {
            if (_repository == null)
                _repository = new RoleRepository();
            return _repository;
        }

        public Role GetById(int id)
        {
            return GetRepository().GetById(id);
        }

        public void Save(Role role)
        {
            GetRepository().Save(role);
        }

        public void Delete(Role role)
        {
            GetRepository().Delete(role);
        }

        public void Remove(Role role)
        {
            GetRepository().Remove(role);
        }

        public Dictionary<int, string> GetShortList() { return GetRepository().GetShortList(); }

        public string GetPermissionsAsXml(int roleId) { return GetRepository().GetPermissionsAsXml(roleId); }

        public ANCommon.DataAccess.IDataSetAdapter GetDataSetAdapter() { return new RoleDataSetAdapter();  }

    }
}
