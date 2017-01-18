using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Alma.Infra;
using Alma.Infra.Dados;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Proxy;
using NHibernate.Tool.hbm2ddl;

namespace Alma.Dados.OrmNHibernate
{
    static class Repositorio
    {
        internal static void SetConnectionStringResolver(Func<string, string> connectionResolver)
        {
            ConnectionProvider.connectionResolver = connectionResolver;
        }

        public static ISessionFactory GetSessionFactory(string connectionKey, params Assembly[] assemblies)
        {
            var cfg = new NHibernate.Cfg.Configuration();
            switch (Alma.Infra.Config.DeterminarDBMS(connectionKey))
            {
                case DBMS.MsSql:

                    cfg.DataBaseIntegration(db =>
                    {
                        db.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                        db.Dialect<NHibernate.Dialect.MsSql2008Dialect>();
                        db.Driver<NHibernate.Driver.SqlClientDriver>();
                        db.ConnectionStringName = connectionKey;
                        db.ConnectionProvider<ConnectionProvider>();
                        db.LogFormattedSql = true;
                        db.LogSqlInConsole = true;
                        //UseReflectionOptimizer
                    });

                    break;
                case DBMS.SqLite:
                    cfg.DataBaseIntegration(db =>
                    {
                        db.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                        db.Dialect<NHibernate.Dialect.SQLiteDialect>();
                        db.Driver<NHibernate.Driver.SQLite20Driver>();
                        db.ConnectionStringName = connectionKey;
                        db.ConnectionProvider<ConnectionProvider>();
                        db.LogFormattedSql = true;
                        db.LogSqlInConsole = true;
                        //UseReflectionOptimizer
                    });
                    break;
                case DBMS.Oracle:
                    if (Alma.Infra.Config.IsManagedOracle(connectionKey))
                    {

                        cfg.DataBaseIntegration(db =>
                        {
                            db.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                            db.Dialect<NHibernate.Dialect.Oracle10gDialect>();
                            db.Driver<NHibernate.Driver.OracleManagedDataClientDriver>();
                            db.ConnectionStringName = connectionKey;
                            db.ConnectionProvider<ConnectionProvider>();
                            db.LogFormattedSql = true;
                            db.LogSqlInConsole = true;
                            //UseReflectionOptimizer
                        });
                    }
                    else
                    {
                        cfg.DataBaseIntegration(db =>
                        {
                            db.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                            db.Dialect<NHibernate.Dialect.Oracle10gDialect>();
                            db.Driver<NHibernate.Driver.OracleDataClientDriver>();
                            db.ConnectionStringName = connectionKey;
                            db.ConnectionProvider<ConnectionProvider>();
                            db.LogFormattedSql = true;
                            db.LogSqlInConsole = true;
                            //UseReflectionOptimizer
                        });
                    }
                    break;

                case DBMS.MySql:

                    cfg.DataBaseIntegration(db =>
                    {
                        db.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                        db.Dialect<NHibernate.Dialect.MySQL55Dialect>();
                        db.Driver<NHibernate.Driver.MySqlDataDriver>();
                        db.ConnectionStringName = connectionKey;
                        db.ConnectionProvider<ConnectionProvider>();
                        db.LogFormattedSql = true;
                        db.LogSqlInConsole = true;
                        //UseReflectionOptimizer
                    });

                    break;
                default:
                    throw new NotImplementedException("Not implemented provider: " + Alma.Infra.Config.DeterminarDBMS(connectionKey));
            }

            var mapper = new ModelMapper();
            mapper.AddConventions();
            foreach (var a in assemblies)
                mapper.AddMappings(a.GetExportedTypes());

            var hbm = mapper.CompileMappingForAllExplicitlyAddedEntities();
            hbm.autoimport = true;

            cfg.AddMapping(hbm);
            if (Alma.Infra.Config.ExecutarMigracoes)
                cfg.AddSchemaValidationAndMigration();

            try
            {

                var fact = cfg.BuildSessionFactory();
                return fact;
            }
            catch (HibernateException ex)
            {
                if (ex.Message.Contains("NHibernate.Driver") && (Alma.Infra.Config.DeterminarDBMS(connectionKey) == DBMS.Oracle))
                {
                    throw new System.Configuration.ConfigurationErrorsException(
@"Não foi possível localizar o binário no gac do Oracle.DataAccess ou Oracle.ManagedDataAccess. 
Você deve instalar uma versão do oracle client de 64 e/ou 32 bits, ou configurar o ManagedDataAccess adequadamente. 

Versão mínima do Unmanaged: Oracle Client 11.2 com ODP.NET. 
Versões anteriores a 11.2 não possuem o Oracle.DataAccess para o framework 4.0/4.5.
Caso não tenha essa versão, remova todas as versões do client instaladas e instale a
mínima ou superior.

Acrescente a seguinte configuração na Web.Config do Service, caso possua mais de um client:

  <runtime>
    <assemblyBinding xmlns=""urn:schemas-microsoft-com:asm.v1"">
      <qualifyAssembly 
        partialName=""Oracle.DataAccess""
        fullName=""Oracle.DataAccess, Version=4.112.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342, processorArchitecture=AMD64"" />
      <dependentAssembly>
        <assemblyIdentity name=""Oracle.DataAccess"" publicKeyToken=""89b483f429c47342"" culture=""neutral"" />
        <bindingRedirect oldVersion=""0.0.0.0-4.112.1.0"" newVersion=""4.112.2.0"" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>

Confira a versão e a arquitetura com o assembly instalado no GAC pelo Oracle Universal installer.
O GAC do framework 4.0/4.5 fica em C:\Windows\Microsoft.NET\assembly
", ex);
                }
                else
                {
                    throw;
                }
            }

            #region Castle ActiveRecord - Disabled

            //    Configure.ActiveRecord
            //        .ForWeb()
            //        .MakeLazyByDefault()
            //        .VerifyModels()
            //        .RegisterSearch();

            //    DatabaseType db = 0;
            //    if (Alma.Core.Config.ConnectionString.ProviderName.Contains("SqlClient"))
            //        db = DatabaseType.MsSqlServer2008;
            //    if (Alma.Core.Config.ConnectionString.ProviderName.Contains("Oracle"))
            //        db = DatabaseType.Oracle10g;


            //    if (db == 0)
            //        throw new NotImplementedException("Not implemented provider: " + Alma.Core.Config.ConnectionString.ProviderName);

            //    Configure.Storage
            //        .ConnectionStringName(Alma.Core.Config.ConnectionString.Name)
            //        .ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>()
            //        .ShowSql();

            //    if (db == DatabaseType.MsSqlServer2008)
            //    {
            //        Configure.Storage
            //            .Driver<NHibernate.Driver.SqlClientDriver>()
            //            .Dialect<NHibernate.Dialect.MsSql2008Dialect>();
            //    }
            //    else if (db == DatabaseType.Oracle10g)
            //    {
            //        Configure.Storage
            //            .Driver<NHibernate.Driver.OracleDataClientDriver>()
            //            .Dialect<NHibernate.Dialect.Oracle10gDialect>();
            //    }

            //    ActiveRecordStarter.RegisterAssemblies(assemblies);

            //    var sessionFac = ActiveRecordMediator.GetSessionFactoryHolder();
            //    var configs = sessionFac.GetAllConfigurations();

            //    foreach (var config in configs)
            //    {
            //        //config.EventListeners.PostUpdateEventListeners =
            //        //    new IPostUpdateEventListener[] { AuditEventListener.Current };
            //        //config.EventListeners.PostDeleteEventListeners =
            //        //    new IPostDeleteEventListener[] { AuditEventListener.Current };
            //        //config.EventListeners.PostInsertEventListeners =
            //        //    new IPostInsertEventListener[] { AuditEventListener.Current };
            //    }

            //    var sessionsFacs = sessionFac.GetSessionFactories();
            //    return sessionsFacs.SingleOrDefault();

            #endregion
        }

        public static ISessionFactory GetSessionFactory(string connectionKey, params Type[] type)
        {
            return Repositorio.GetSessionFactory(connectionKey, type.Select(t => t.Assembly).ToArray());
        }

        private class ConnectionProvider : NHibernate.Connection.ConnectionProvider
        {
            public static Func<string, string> connectionResolver;

            private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ConnectionProvider));
            public override IDbConnection GetConnection()
            {
                log.Debug("Obtaining IDbConnection from Driver");
                IDbConnection conn = Driver.CreateConnection();
                try
                {
                    conn.ConnectionString = ConnectionString;
                    if (connectionResolver != null)
                        conn.ConnectionString = connectionResolver(conn.ConnectionString);
                    conn.Open();
                }
                catch (Exception)
                {
                    conn.Dispose();
                    throw;
                }

                return conn;
            }
        }


        private static ModelMapper AddConventions(this ModelMapper mapper)
        {
            //TODO: Fazer isto automático.
            mapper.BeforeMapProperty += Alma.Dados.OrmNHibernate.Conventions.TimeSpanConvention.BeforeMapProperty;


            return mapper;
        }

        private static Configuration AddSchemaValidationAndMigration(this Configuration config)
        {
            var logger = LoggerProvider.LoggerFor(typeof(Repositorio));
            System.Action<string> updateExport = x =>
            {
                if (logger != null)
                    logger.Info(x);
            };

            var update = new SchemaUpdate(config);
            try
            {
                update.Execute(updateExport, true);
                if (update.Exceptions != null && update.Exceptions.Count > 0)
                    throw new AggregateException("Ocorreram um ou mais erros ao executar a migração da base de dados. Verifique a lista de exceptions.", update.Exceptions.ToArray());
            }
            catch (Exception)
            {

                throw;
            }

            return config;
        }
    }


    sealed class Repositorio<TEntity> : IRepositorio<TEntity> where TEntity : class
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
            return Session.Load<TEntity>(primaryKey);
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
        public void MergeUpdate(TEntity instance)
        {
            this.Session.Merge(instance);
            Session.Flush();
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
                var dmbs = Alma.Infra.Config.DeterminarDBMS(Config.ResolveConnectionName(typeof(TEntity)));

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
    }
}
