using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecms.Core.Infrastructure;
using Ecms.Core.Domain.Model;
using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Repositories;
using Ecms.Security.Infrastructure.DataSetAdapters;
using ANSqlBuilder;
using ANCommon.Sql;

namespace Ecms.Security.Application.Services
{
    public interface IUserService
    {
        ANCommon.DataAccess.IDataSetAdapter GetDataSetAdapter();
        UserRepository GetRepository();
        bool SetPassword(string userName, string oldPassword, string newPassword);
        User GetUserById(int id);
        User GetUserByUserName(string userName);
        User GetUserByEmail(string email);
        string GetNavigationAsXml(string[] solutionCode, string userName);
        void Save(User user);
        void Delete(User user);
        void Remove(User user);
        Dictionary<int, string> GetShortList();
        Dictionary<string, string> GetShortList2();

    }

    public class UserService: IUserService
    {
        UserRepository _repository;
        public UserRepository GetRepository()
        {
            if (_repository == null)
                _repository = new UserRepository();
            return _repository;
        }

        public ANCommon.DataAccess.IDataSetAdapter GetDataSetAdapter()
        {
            return new UserDataSetAdapter();
        }

        public bool SetPassword(string userName, string oldPassword, string newPassword) { return GetRepository().SetPassword(userName, oldPassword, newPassword); }

        public User GetUserById(int id) { return GetRepository().GetUserById(id); }

        public User GetUserByUserName(string userName) { return GetRepository().GetUserByUserName(userName); }

        public User GetUserByEmail(string email) { return GetRepository().GetUserByEmail(email); }

        public string GetNavigationAsXml(string[] solutionCode, string userName)
        {
            return GetRepository().GetNavigationAsXml(solutionCode, userName);
        }

        public void Save(User user) { GetRepository().Save(user); }
        public void Delete(User user) { GetRepository().Delete(user); }
        public void Remove(User user) { GetRepository().Remove(user); }

        public Dictionary<int, string> GetShortList() { return GetRepository().GetShortList();  }
        public Dictionary<string, string> GetShortList2() { return GetRepository().GetShortList2(); }

    }

    

}
