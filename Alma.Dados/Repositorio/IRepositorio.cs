using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Alma.Dados
{

    /// <summary>
    /// A interface <see cref="IRepositorio{TEntidade}"/> define um contrato
    /// padrão que todo repositório deve implementar.
    /// </summary>
    /// <typeparam name="TEntidade">Tipo de entidade que o respositório encapsula.</typeparam>
    public interface IRepositorio<TEntity> : IQueryable<TEntity> where TEntity : class
    {
        TEntity Get(object primaryKey);
        TEntity GetSessionless(object primaryKey);
        IList<T> ExecuteNamedQuery<T>(string queryName, IDictionary<string, object> parameters = null);
        T ExecuteNamedQueryScalar<T>(string queryName, IDictionary<string, object> parameters = null);
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
        void ExecuteProcedure(string procName, IDictionary<string, object> parameters = null);
        IDbTransaction GetTransaction();

        IList<TEntity> UnProxy(IList<TEntity> list);
        TEntity UnProxy(TEntity instance);

        void EnableFilter(string name, object value);
        void DisableFilter(string name);

    }

}
