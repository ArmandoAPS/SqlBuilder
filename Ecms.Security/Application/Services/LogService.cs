using Ecms.Security.Domain.Model;
using Ecms.Security.Infrastructure.Repositories;
using System;
using System.Collections.Generic;


namespace Ecms.Security.Application.Services
{
    public interface ILogService
    {
        Log GetById(Guid id);

        Guid CreateLog(string description, string userName, string ipAddress);
    }

    public class LogService: ILogService
    {
        LogRepository _repository;
        public LogRepository GetRepository()
        {
            if (_repository == null)
                _repository = new LogRepository();
            return _repository;
        }

        public Log GetById(Guid id)
        {
            return GetRepository().GetById(id);
        }

        public Guid CreateLog(string description, string userName, string ipAddress)
        {
            var log = new Log(description, userName, ipAddress, DateTime.UtcNow);
            GetRepository().Insert(log);
            return log.Id;
        }

    }
}