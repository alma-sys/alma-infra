﻿using Alma.Dados.OrmNHibernate.Events;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Alma.Dados.OrmNHibernate
{
    static class SessionFactory
    {
        internal static void SetConnectionStringResolver(Func<string, string> connectionResolver)
        {
            ConnectionProvider.connectionResolver = connectionResolver;
        }

        public static ISessionFactory GetSessionFactory(string connectionKey, params Assembly[] assemblies)
        {
            var cfg = new NHibernate.Cfg.Configuration();
            switch (Config.DeterminarDBMS(connectionKey))
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
                        if (Config.IsolationLevel != null)
                            db.IsolationLevel = Config.IsolationLevel.Value;
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
                        if (Config.IsolationLevel != null)
                            db.IsolationLevel = Config.IsolationLevel.Value;
                        //UseReflectionOptimizer
                    });
                    break;
                case DBMS.Oracle:
                    if (Config.IsManagedOracle(connectionKey))
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
                            if (Config.IsolationLevel != null)
                                db.IsolationLevel = Config.IsolationLevel.Value;
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
                            if (Config.IsolationLevel != null)
                                db.IsolationLevel = Config.IsolationLevel.Value;
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
                        if (Config.IsolationLevel != null)
                            db.IsolationLevel = Config.IsolationLevel.Value;
                        //UseReflectionOptimizer
                    });

                    break;
                default:
                    throw new NotImplementedException("Not implemented provider: " + Config.DeterminarDBMS(connectionKey));
            }

            var types =
                assemblies.SelectMany(a => a.GetTypes());

            AddFilters(cfg, types);

            AddEvents(cfg, types);

            AddMappings(cfg, types);

            try
            {

                var fact = cfg.BuildSessionFactory();
                return fact;
            }
            catch (HibernateException ex)
            {
                if (ex.Message.Contains("NHibernate.Driver") && (Config.DeterminarDBMS(connectionKey) == DBMS.Oracle))
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
            //    if (Core.Config.ConnectionString.ProviderName.Contains("SqlClient"))
            //        db = DatabaseType.MsSqlServer2008;
            //    if (Core.Config.ConnectionString.ProviderName.Contains("Oracle"))
            //        db = DatabaseType.Oracle10g;


            //    if (db == 0)
            //        throw new NotImplementedException("Not implemented provider: " + Core.Config.ConnectionString.ProviderName);

            //    Configure.Storage
            //        .ConnectionStringName(Core.Config.ConnectionString.Name)
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
            return SessionFactory.GetSessionFactory(connectionKey, type.Select(t => t.Assembly).ToArray());
        }

        private class ConnectionProvider : NHibernate.Connection.ConnectionProvider
        {
            public static Func<string, string> connectionResolver;

            private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ConnectionProvider));
            public override DbConnection GetConnection()
            {
                log?.Debug("Obtaining IDbConnection from Driver");
                DbConnection conn = Driver.CreateConnection();
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

            public override async Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken)
            {
                return await Task.Run(() =>
                {
                    return GetConnection();
                }, cancellationToken);
            }
        }


        private static ModelMapper AddConventions(this ModelMapper mapper)
        {
            //TODO: Fazer isto automático.
            mapper.BeforeMapProperty += Conventions.TimeSpanConvention.BeforeMapProperty;


            return mapper;
        }

        private static Configuration AddSchemaValidationAndMigration(this Configuration config)
        {
            var logger = LoggerProvider.LoggerFor(typeof(SessionFactory));
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

        private static void AddFilters(Configuration cfg, IEnumerable<Type> types)
        {
            var logger = Config.AtivarLog ? (Action<string>)((string text) => Trace.WriteLine(text, nameof(AddFilters))) : null;

            var filters = types.Where(x => typeof(Mapper.GlobalFilterMapping).IsAssignableFrom(x)).ToArray();
            logger?.Invoke($"Registrando {filters.Length} filtros globais de repositório.");
            foreach (var fmapType in filters)
            {
                var map = (Mapper.GlobalFilterMapping)Activator.CreateInstance(fmapType);
                foreach (var fd in map.filters)
                    cfg.FilterDefinitions.Add(fd);
            }
        }
        private static void AddEvents(Configuration cfg, IEnumerable<Type> types)
        {
            var logger = Config.AtivarLog ? (Action<string>)((string text) => Trace.WriteLine(text, nameof(AddEvents))) : null;

            logger?.Invoke($"Registrando SavedDataEventHandler para listeners.");

            cfg.SetListeners(ListenerType.PostCommitInsert, new[] { typeof(SavedDataEventHandler).AssemblyQualifiedName });
            cfg.SetListeners(ListenerType.PostCommitUpdate, new[] { typeof(SavedDataEventHandler).AssemblyQualifiedName });
            cfg.SetListeners(ListenerType.PostCommitDelete, new[] { typeof(SavedDataEventHandler).AssemblyQualifiedName });
            cfg.SetListeners(ListenerType.PostLoad, new[] { typeof(SavedDataEventHandler).AssemblyQualifiedName });
        }

        private static void AddMappings(Configuration cfg, IEnumerable<Type> types)
        {
            var mapper = new ModelMapper();
            mapper.AddConventions();
            mapper.AddMappings(types);

            var hbm = mapper.CompileMappingForAllExplicitlyAddedEntities();
            hbm.autoimport = true;
            hbm.defaultlazy = Config.AtivarLazy;

            cfg.AddMapping(hbm);
            if (Config.ExecutarMigracoes)
                cfg.AddSchemaValidationAndMigration();
        }
    }
}
