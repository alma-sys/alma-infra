using NHibernate;
using NHibernate.Proxy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Alma.Dados.OrmNHibernate
{
    sealed class Repositorio<TEntity> : IQueryable<TEntity>, IRepositorio<TEntity> where TEntity : class
    {


        #region Reflection Helper
        private static bool IsNHibernateProxy(Type entityType)
        {
            return entityType.Name.IndexOf("proxy", StringComparison.InvariantCultureIgnoreCase) != -1;
        }

        private static Type GetRealType(Type entityType)
        {
            Type originalType = entityType;

            if (IsNHibernateProxy(entityType))
            {
                originalType = entityType.BaseType;
            }

            return originalType;
        }

        private string GetIdName(Type type)
        {

            Type originalType = GetRealType(type);

            string idName = Session.SessionFactory.GetClassMetadata(originalType.FullName).IdentifierPropertyName;

            return (idName);
        }

        private bool IsPropertyMapped(Type type, string property)
        {

            Type originalType = GetRealType(type);

            int index = Session.SessionFactory.GetClassMetadata(originalType.FullName).PropertyNames.ToList().IndexOf(property);

            return index != -1;
        }


        private bool IsPropertyAssociation(Type type, string property)
        {
            Type originalType = GetRealType(type);

            int index = Session.SessionFactory.GetClassMetadata(originalType.FullName).PropertyNames.ToList().IndexOf(property);

            return Session.SessionFactory.GetClassMetadata(originalType.FullName).PropertyTypes[index].IsAssociationType;
        }
        #endregion



        private ISession Session { get; set; }
        public Repositorio(ISession session)
        {
            Session = session;
            var c = session.SessionFactory.GetClassMetadata(typeof(TEntity));
            if (c == null)
                throw new InvalidOperationException("Cannot create repository of unmapped class: " + typeof(TEntity).FullName);
        }

        public ISession GetSession()
        {
            return Session;
        }

        public void Evict(TEntity instance)
        {
            Session.Evict(instance);
        }

        public void ClearCache()
        {
            Session.Evict(typeof(TEntity));
            Session.Clear();
        }

        public TEntity Get(object primaryKey)
        {
            return Session.Get<TEntity>(primaryKey);
        }

        public TEntity GetSessionless(object primaryKey)
        {
            using (IStatelessSession statelessSesssion = Session.SessionFactory.OpenStatelessSession())
            {
                Type type = typeof(TEntity);

                // Identificando a propriedade id
                string idName = GetIdName(type);

                var criteria = statelessSesssion.CreateCriteria<TEntity>();

                string[] propertiesArray = type.GetProperties().Select(pi => pi.Name).ToArray();

                bool needsDistinct = false;
                foreach (string property in propertiesArray)
                {
                    if (IsPropertyMapped(type, property) && IsPropertyAssociation(type, property))
                    {
                        criteria = criteria.SetFetchMode(property, FetchMode.Join);
                        needsDistinct = true;
                    }
                }
                criteria = criteria.Add(NHibernate.Criterion.Restrictions.Eq(idName, primaryKey));
                if (needsDistinct)
                {
                    criteria = criteria.SetResultTransformer(NHibernate.Transform.Transformers.DistinctRootEntity); //porque isso nao funciona?
                }

                var list = criteria.List<TEntity>();

                return list.Distinct().SingleOrDefault();
            }
        }

        public IList<T> ExecuteNamedQuery<T>(string queryName, IDictionary<string, object> parameters = null)
        {
            var query = Session.GetNamedQuery(queryName);

            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    query.SetParameter(item.Key, item.Value);
                }
            }

            return query.List<T>();
        }

        public T ExecuteNamedQueryScalar<T>(string queryName, IDictionary<string, object> parameters = null)
        {
            var query = Session.GetNamedQuery(queryName);

            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    query.SetParameter(item.Key, item.Value);
                }
            }

            return (T)System.Convert.ChangeType(query.UniqueResult(), typeof(T));
        }

        public IList<TEntity> List<TProperty>(Expression<Func<TEntity, TProperty>> property, bool orderAsc = true)
        {
            if (orderAsc)
            {
                return this.OrderBy(property).ToList();
            }
            else
            {
                return this.OrderByDescending(property).ToList();
            }
        }



        #region Implementation of IEnumerable

        public IEnumerator<TEntity> GetEnumerator()
        {
            return Session.Query<TEntity>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IQueryable

        public Expression Expression
        {
            get { return Session.Query<TEntity>().Expression; }
        }

        public Type ElementType
        {
            get { return Session.Query<TEntity>().ElementType; }
        }

        public IQueryProvider Provider
        {
            get { return Session.Query<TEntity>().Provider; }
        }

        #endregion

        #region Implementation of Transactions


        //[Transacao]
        public void Create(TEntity instance)
        {
            Session.Save(instance);
            Session.Flush();
        }

        //[Transacao]
        public void Create(IList<TEntity> instances)
        {
            foreach (var instance in instances)
            {
                Session.Save(instance);
            }
            Session.Flush();
        }


        //[Transacao]
        public async Task CreateAsync(TEntity instance)
        {
            await Session.SaveAsync(instance);
            await Session.FlushAsync();
        }

        //[Transacao]
        public async Task CreateAsync(IList<TEntity> instances)
        {
            var tasks = instances.Select(x => Session.SaveAsync(x)).ToList();
            await Task.WhenAll(tasks);
            await Session.FlushAsync();
        }


        //[Transacao]
        public void Save(TEntity instance)
        {
            Session.Update(instance);
            Session.Flush();
        }

        //[Transacao]
        public void Save(IList<TEntity> instances)
        {
            foreach (var instance in instances)
            {
                Session.Update(instance);
            }
            Session.Flush();
        }

        public async Task SaveAsync(TEntity instance)
        {
            await Session.UpdateAsync(instance);
            await Session.FlushAsync();
        }

        //[Transacao]
        public async Task SaveAsync(IList<TEntity> instances)
        {
            var tasks = instances.Select(x => Session.UpdateAsync(x)).ToList();
            await Task.WhenAll(tasks);
            await Session.FlushAsync();
        }

        //[Transacao]
        public void Delete(TEntity instance)
        {
            Session.Delete(instance);
            Session.Flush();
        }

        //[Transacao]
        public void Delete(IList<TEntity> instances)
        {
            foreach (var instance in instances)
            {
                Session.Delete(instance);
            }
            Session.Flush();
        }


        //[Transacao]
        public async Task DeleteAsync(TEntity instance)
        {
            await Session.DeleteAsync(instance);
            await Session.FlushAsync();
        }

        //[Transacao]
        public async Task DeleteAsync(IList<TEntity> instances)
        {
            var tasks = instances.Select(x => Session.DeleteAsync(x)).ToList();
            await Task.WhenAll(tasks);
            await Session.FlushAsync();
        }

        //[Transacao]
        public void MergeUpdate(TEntity instance)
        {
            this.Session.Merge(instance);
            Session.Flush();
        }

        //[Transacao]
        public async Task MergeUpdateAsync(TEntity instance)
        {
            await this.Session.MergeAsync(instance);
            await Session.FlushAsync();
        }


        #endregion

        public IDbTransaction GetTransaction()
        {
            return null;// GetSession().Transaction
        }


        public IList<TEntity> UnProxy(IList<TEntity> list)
        {
            if (list == null)
                return null;

            if (NHibernateProxyHelper.IsProxy(list))
            {
                var session = GetSession().GetSessionImplementation();
                list = (IList<TEntity>)session.PersistenceContext.Unproxy(list);

            }
            for (var i = 0; i < list.Count; i++)
            {
                list[i] = UnProxy(list[i]);
            }
            return list;
        }

        public TEntity UnProxy(TEntity instance)
        {
            if (instance != null && NHibernateProxyHelper.IsProxy(instance))
            {
                var session = GetSession().GetSessionImplementation();
                instance = (TEntity)session.PersistenceContext.Unproxy(instance);
            }
            if (instance != null)
                UnProxyTree(instance);
            return instance;
        }


        /// <summary>
        /// Force initialzation of a possibly proxied object tree up to the maxDepth.
        /// Once the maxDepth is reached, entity properties will be replaced with
        /// placeholder objects having only the identifier property populated.
        /// </summary>
        private void UnProxyTree(TEntity instance)
        {
            var persistentType = instance.GetType();
            // Determine persistent type of the object
            var session = GetSession().GetSessionImplementation();
            var sessionFactory = GetSession().SessionFactory;

            var classMetadata = sessionFactory.GetClassMetadata(persistentType);

            // Iterate through each property and unproxy entity types
            for (int i = 0; i < classMetadata.PropertyTypes.Length; i++)
            {
                var nhType = classMetadata.PropertyTypes[i];
                var propertyName = classMetadata.PropertyNames[i];
                var propertyInfo = persistentType.GetProperty(propertyName);
                var propertyValue = propertyInfo.GetValue(instance, null);

                if (!NHibernateUtil.IsInitialized(propertyValue))
                {
                    propertyInfo.SetValue(instance, null, null);
                }
                else if (NHibernateProxyHelper.IsProxy(propertyValue))
                {
                    propertyInfo.SetValue(instance, session.PersistenceContext.Unproxy(propertyValue), null);
                }
            }
        }

        public void ExecuteProcedure(string procName, IDictionary<string, object> parameters = null)
        {
            var session = GetSession();
            if (session.Connection != null && session.Connection.State == ConnectionState.Open)
            {
                var dmbs = Config.DeterminarDBMS(Config.ResolveConnectionName(typeof(TEntity)));

                var pmark = ":";
                var queryFormat = "";
                switch (dmbs)
                {
                    case DBMS.MsSql:
                        queryFormat = "EXEC {0} {1}";
                        break;
                    case DBMS.Oracle:
                    case DBMS.MySql:
                    case DBMS.PostgreSql:
                    case DBMS.SqLite:
                        queryFormat = "CALL {0}({1})";
                        break;
                    default:
                        throw new NotImplementedException();
                }

                var param = string.Empty;
                if (parameters != null && parameters.Count > 0)
                {
                    var keys = parameters.Keys;
                    param = string.Join(", ", keys.Select(k => pmark + k).ToArray());
                }
                var query = session.CreateSQLQuery(string.Format(queryFormat, procName, param));
                if (parameters != null && parameters.Count > 0)
                    foreach (var key in parameters.Keys)
                        query.SetParameter(key, parameters[key]);

                var result = query.ExecuteUpdate();
            }
            else
                throw new InvalidOperationException("Connection is invalid or closed.");

        }

        public void EnableFilter(string name, object value)
        {
            Session.EnableFilter(name)
                .SetParameter(name, value);
        }

        public void DisableFilter(string name)
        {
            Session.DisableFilter(name);
        }
    }
}
