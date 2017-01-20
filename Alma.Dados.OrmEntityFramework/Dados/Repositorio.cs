using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Alma.Dados.OrmEntityFramework
{
    sealed class Repositorio<TEntity> : IRepositorio<TEntity> where TEntity : class
    {
        private IContexto contexto;
        public Repositorio(IContexto contexto)
        {
            this.contexto = contexto;
        }


        #region Implementation of IEnumerable

        public IEnumerator<TEntity> GetEnumerator()
        {
            return contexto.Query<TEntity>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IQueryable

        public Expression Expression
        {
            get { return contexto.Query<TEntity>().Expression; }
        }

        public Type ElementType
        {
            get { return contexto.Query<TEntity>().ElementType; }
        }

        public IQueryProvider Provider
        {
            get { return contexto.Query<TEntity>().Provider; }
        }

        #endregion

        #region Implementation of Transactions


        //[Transacao]
        public void Create(TEntity instance)
        {
            contexto.Save(instance);
            contexto.Flush();
        }

        //[Transacao]
        public void Create(IList<TEntity> instances)
        {
            foreach (var instance in instances)
            {
                contexto.Save(instance);
            }
            contexto.Flush();
        }

        //[Transacao]
        public void Save(TEntity instance)
        {
            contexto.Update(instance);
            contexto.Flush();
        }

        //[Transacao]
        public void Save(IList<TEntity> instances)
        {
            foreach (var instance in instances)
            {
                contexto.Update(instance);
            }
            contexto.Flush();
        }

        //[Transacao]
        public void Delete(TEntity instance)
        {
            contexto.Delete(instance);
            contexto.Flush();
        }

        //[Transacao]
        public void Delete(IList<TEntity> instances)
        {
            foreach (var instance in instances)
            {
                contexto.Delete(instance);
            }
            contexto.Flush();
        }

        //[Transacao]
        public void MergeUpdate(TEntity instance)
        {
            contexto.Merge(instance);
            contexto.Flush();
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


    }
}