using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Alma.DataAccess.OrmEntityFramework
{
    sealed class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private IContext context;
        public Repository(IContext context)
        {
            this.context = context;
        }


        #region Implementation of IEnumerable

        public IEnumerator<TEntity> GetEnumerator()
        {
            return context.Query<TEntity>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IQueryable

        public Expression Expression
        {
            get { return context.Query<TEntity>().Expression; }
        }

        public Type ElementType
        {
            get { return context.Query<TEntity>().ElementType; }
        }

        public IQueryProvider Provider
        {
            get { return context.Query<TEntity>().Provider; }
        }

        #endregion

        #region Implementation of Transactions


        //[Transacao]
        public void Create(TEntity instance)
        {
            context.Save(instance);
            context.Flush();
        }

        //[Transacao]
        public void Create(IList<TEntity> instances)
        {
            foreach (var instance in instances)
            {
                context.Save(instance);
            }
            context.Flush();
        }

        //[Transacao]
        public void Save(TEntity instance)
        {
            context.Update(instance);
            context.Flush();
        }

        //[Transacao]
        public void Save(IList<TEntity> instances)
        {
            foreach (var instance in instances)
            {
                context.Update(instance);
            }
            context.Flush();
        }

        //[Transacao]
        public void Delete(TEntity instance)
        {
            context.Delete(instance);
            context.Flush();
        }

        //[Transacao]
        public void Delete(IList<TEntity> instances)
        {
            foreach (var instance in instances)
            {
                context.Delete(instance);
            }
            context.Flush();
        }

        //[Transacao]
        public void MergeUpdate(TEntity instance)
        {
            context.Merge(instance);
            context.Flush();
        }
        #endregion

        public TEntity Get(object primaryKey)
        {
            throw new NotImplementedException();
        }

        public TEntity GetSessionless(object primaryKey)
        {
            throw new NotImplementedException();
        }

        public IList<T> ExecuteNamedQuery<T>(string queryName, IDictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }

        public T ExecuteNamedQueryScalar<T>(string queryName, IDictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> List<TProperty>(Expression<Func<TEntity, TProperty>> orderProperty, bool orderAsc = true)
        {
            throw new NotImplementedException();
        }

        public void Evict(TEntity entidade)
        {
            throw new NotImplementedException();
        }

        public void ClearCache()
        {
            throw new NotImplementedException();
        }

        public void ExecuteProcedure(string procName, IDictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }

        public IDbTransaction GetTransaction()
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> UnProxy(IList<TEntity> list)
        {
            throw new NotImplementedException();
        }

        public TEntity UnProxy(TEntity instance)
        {
            throw new NotImplementedException();
        }

        public void EnableFilter(string name, object value)
        {
            throw new NotImplementedException();
        }

        public void DisableFilter(string name)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(TEntity entidade)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(IList<TEntity> entidades)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(TEntity entidade)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(IList<TEntity> entidade)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TEntity entidade)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IList<TEntity> entidades)
        {
            throw new NotImplementedException();
        }

        public Task MergeUpdateAsync(TEntity entidade)
        {
            throw new NotImplementedException();
        }

        public IList<T> ExecuteCustomSql<T>(string sql, IDictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }

        public T ExecuteCustomSqlScalar<T>(string sql, IDictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }

        public Task<IList<T>> ExecuteCustomSqlAsync<T>(string sql, IDictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> ExecuteCustomSqlScalarAsync<T>(string sql, IDictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }

        public Task ExecuteProcedureAsync(string procName, IDictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> ExecuteNamedQuery(string queryName, IDictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TEntity>> ExecuteNamedQueryAsync(string queryName, IDictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> ExecuteCustomSql(string sql, IDictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TEntity>> ExecuteCustomSqlAsync(string sql, IDictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }
    }
}