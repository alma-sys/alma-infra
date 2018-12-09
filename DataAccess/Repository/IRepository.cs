using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Alma.DataAccess
{

    /// <summary>
    /// The interface <see cref="IRepository{TEntity}"/> defines a standard contract for all types of repositories.
    /// </summary>
    /// <typeparam name="TEntity">The entity type that this repository will work for.</typeparam>
    public interface IRepository<TEntity> : IQueryable<TEntity> where TEntity : class
    {
        TEntity Get(object primaryKey);
        TEntity GetSessionless(object primaryKey);
        IList<TEntity> ExecuteNamedQuery(string queryName, IDictionary<string, object> parameters = null);
        Task<IList<TEntity>> ExecuteNamedQueryAsync(string queryName, IDictionary<string, object> parameters = null);
        T ExecuteNamedQueryScalar<T>(string queryName, IDictionary<string, object> parameters = null);
        IList<TEntity> ExecuteCustomSql(string sql, IDictionary<string, object> parameters = null);
        T ExecuteCustomSqlScalar<T>(string sql, IDictionary<string, object> parameters = null);
        Task<IList<TEntity>> ExecuteCustomSqlAsync(string sql, IDictionary<string, object> parameters = null);
        Task<T> ExecuteCustomSqlScalarAsync<T>(string sql, IDictionary<string, object> parameters = null);
        IList<TEntity> List<TProperty>(Expression<Func<TEntity, TProperty>> orderProperty, bool orderAsc = true);
        //ISession GetSession();
        void Evict(TEntity entidade);
        void ClearCache();
        //void Validate(TEntidade entity);
        //T Mesclar<T>(T entidade, string configuracao);

        void Create(TEntity entidade);
        void Create(IList<TEntity> entidades);
        void Save(TEntity entidade);
        void Save(IList<TEntity> entidade);
        void Delete(TEntity entidade);
        void Delete(IList<TEntity> entidades);
        void MergeUpdate(TEntity entidade);




        Task CreateAsync(TEntity entidade);
        Task CreateAsync(IList<TEntity> entidades);
        Task SaveAsync(TEntity entidade);
        Task SaveAsync(IList<TEntity> entidade);
        Task DeleteAsync(TEntity entidade);
        Task DeleteAsync(IList<TEntity> entidades);
        Task MergeUpdateAsync(TEntity entidade);



        void ExecuteProcedure(string procName, IDictionary<string, object> parameters = null);
        Task ExecuteProcedureAsync(string procName, IDictionary<string, object> parameters = null);
        IDbTransaction GetTransaction();

        IList<TEntity> UnProxy(IList<TEntity> list);
        TEntity UnProxy(TEntity instance);

        void EnableFilter(string name, object value);
        void DisableFilter(string name);

    }

}
