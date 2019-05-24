using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ANCommon.Sql;

namespace ANCommon.DataAccess
{
    public interface IRepository<TEntity>
    {
        void Save(TEntity entity);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
         void Delete(ICondition filter);
        void Remove(TEntity entity);
        void Remove(ICondition filter);

        TEntity GetOne(ICondition filter);
        TEntity GetOne(ICondition filter, IOrderByExpression order_by);

        IEnumerable<TEntity> GetAll(ICondition filter);
        IEnumerable<TEntity> GetAll(ICondition filter, IOrderByExpression order_by);
        IEnumerable<TEntity> GetAll(ICondition filter, IOrderByExpression order_by, int start_record, int max_records);

        int GetCount();
        int GetCount(ICondition filter);

        //TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter);
        //IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter);
        int GetCount(Expression<Func<TEntity, bool>> filter);
    }
}
