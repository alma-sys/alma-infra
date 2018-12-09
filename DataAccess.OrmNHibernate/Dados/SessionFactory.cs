using Alma.DataAccess.OrmNHibernate.Events;
using NHibernate;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using StackExchange.Profiling;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Alma.DataAccess.OrmNHibernate
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
                        //db.Driver<DriverSqlClient>();
                        db.Driver<NHibernate.Driver.SqlClientDriver>();
                        db.ConnectionStringName = connectionKey;
                        db.ConnectionProvider<ConnectionProvider>();
                        db.PrepareCommands = Config.PrepareCommands;

                        db.LogFormattedSql = true;
                        db.LogSqlInConsole = false;
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
                        //db.Driver<DriverSQLite20>();
                        db.Driver<NHibernate.Driver.SQLite20Driver>();
                        db.ConnectionStringName = connectionKey;
                        db.ConnectionProvider<ConnectionProvider>();
                        db.PrepareCommands = Config.PrepareCommands;
                        db.LogFormattedSql = true;
                        db.LogSqlInConsole = false;
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
                            //db.Driver<DriverOracleManaged>();
                            db.Driver<NHibernate.Driver.OracleManagedDataClientDriver>();
                            db.ConnectionStringName = connectionKey;
                            db.ConnectionProvider<ConnectionProvider>();
                            db.PrepareCommands = Config.PrepareCommands;
                            db.LogFormattedSql = true;
                            db.LogSqlInConsole = false;
                            if (Config.IsolationLevel != null)
                                db.IsolationLevel = Config.IsolationLevel.Value;
                            db.Batcher<OracleLoggingBatchingBatcherFactory>();
                            //UseReflectionOptimizer
                        });
                    }
                    else
                    {
                        cfg.DataBaseIntegration(db =>
                        {
                            db.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                            db.Dialect<NHibernate.Dialect.Oracle10gDialect>();
                            //db.Driver<DriverOracle>();
                            db.Driver<NHibernate.Driver.OracleDataClientDriver>();
                            db.ConnectionStringName = connectionKey;
                            db.ConnectionProvider<ConnectionProvider>();
                            db.PrepareCommands = Config.PrepareCommands;
                            db.LogFormattedSql = true;
                            db.LogSqlInConsole = false;
                            if (Config.IsolationLevel != null)
                                db.IsolationLevel = Config.IsolationLevel.Value;
                            //UseReflectionOptimizer
                            db.Batcher<OracleLoggingBatchingBatcherFactory>();
                        });
                    }
                    break;

                case DBMS.MySql:

                    cfg.DataBaseIntegration(db =>
                    {
                        db.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                        db.Dialect<NHibernate.Dialect.MySQL55Dialect>();
                        //db.Driver<DriverMySql>();
                        db.Driver<NHibernate.Driver.MySqlDataDriver>();
                        db.ConnectionStringName = connectionKey;
                        db.ConnectionProvider<ConnectionProvider>();
                        db.PrepareCommands = Config.PrepareCommands;
                        db.LogFormattedSql = true;
                        db.LogSqlInConsole = false;
                        if (Config.IsolationLevel != null)
                            db.IsolationLevel = Config.IsolationLevel.Value;
                        //UseReflectionOptimizer
                    });

                    break;
                default:
                    throw new NotImplementedException("Not implemented provider: " + Config.DeterminarDBMS(connectionKey));
            }

            if (Config.EnableLog)
            {
                cfg.SessionFactory()
                    .GenerateStatistics();
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

            private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(ConnectionProvider));
            public override DbConnection GetConnection()
            {
                log?.Debug("Obtaining IDbConnection from Driver");

                var conn = Driver.CreateConnection();
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
            var logger = NHibernateLogger.For(typeof(SessionFactory));
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
            var logger = Config.EnableLog ? (Action<string>)((string text) => Trace.WriteLine(text, nameof(AddFilters))) : null;

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
            var logger = Config.EnableLog ? (Action<string>)((string text) => Trace.WriteLine(text, nameof(AddEvents))) : null;

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
            hbm.defaultlazy = Config.EnableLazyLoad;

            cfg.AddMapping(hbm);
            if (Config.ExecuteMigrations)
                cfg.AddSchemaValidationAndMigration();
        }

        #region Drivers
        /*
        private static DbCommand CheckProfiled(DbCommand command)
        {
            if (Config.AtivarMiniProfiler && MiniProfiler.Current != null)
                return new ProfiledDbCommand(command, command.Connection, MiniProfiler.Current);
            else
                return command;
        }



        private class DriverOracleManaged : NHibernate.Driver.OracleManagedDataClientDriver
        {
            public override DbCommand CreateCommand()
            {
                return CheckProfiled(base.CreateCommand());
            }
        }
        private class DriverSqlClient : NHibernate.Driver.SqlClientDriver
        {
            public override DbCommand CreateCommand()
            {
                return CheckProfiled(base.CreateCommand());
            }
        }
        private class DriverSQLite20 : NHibernate.Driver.SQLite20Driver
        {
            public override DbCommand CreateCommand()
            {
                return CheckProfiled(base.CreateCommand());
            }
        }
        private class DriverOracle : NHibernate.Driver.OracleDataClientDriver
        {
            public override DbCommand CreateCommand()
            {
                return CheckProfiled(base.CreateCommand());
            }
        }

        private class DriverMySql : NHibernate.Driver.MySqlDataDriver
        {
            public override DbCommand CreateCommand()
            {
                return CheckProfiled(base.CreateCommand());
            }
        }
        */
        #endregion

        #region Oracle Profiler 
        class OracleLoggingBatchingBatcherFactory : NHibernate.AdoNet.OracleDataClientBatchingBatcherFactory
        {
            public override IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
            {
                return new OracleLoggingBatchingBatcher(connectionManager, interceptor);
            }
        }

        class OracleLoggingBatchingBatcher : OracleDataClientBatchingBatcher, IBatcher
        {
            // here override ExecuteNonQuery, DoExecuteBatch and ExecuteReader. 
            //You can do all kind of intercepting, logging or measuring here
            //If they are not overrideable just implement them and use "new" keyword if necessary
            //since we inherit IBatcher explicitly it will work polymorphically.
            //Make sure you call base implementation too or re-implement the method from scratch
            public OracleLoggingBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor) :
                base(connectionManager, interceptor)
            {
            }

            public override DbDataReader ExecuteReader(DbCommand cmd)
            {
                if (MiniProfiler.Current != null)
                    using (cmd.GetTiming("Reader", MiniProfiler.Current))
                    {
                        return base.ExecuteReader(cmd);
                    }
                else
                    return base.ExecuteReader(cmd);
            }

            public override async Task<DbDataReader> ExecuteReaderAsync(DbCommand cmd, CancellationToken cancellationToken)
            {
                if (MiniProfiler.Current != null)
                    using (cmd.GetTiming("Reader", MiniProfiler.Current))
                    {
                        return await base.ExecuteReaderAsync(cmd, cancellationToken);
                    }
                else
                    return await base.ExecuteReaderAsync(cmd, cancellationToken);
            }

            protected override void DoExecuteBatch(DbCommand ps)
            {
                if (MiniProfiler.Current != null)
                    using (ps.GetTiming("NonQuery", MiniProfiler.Current))
                    {
                        base.DoExecuteBatch(ps);
                    }
                else
                    base.DoExecuteBatch(ps);
            }

            protected override async Task DoExecuteBatchAsync(DbCommand ps, CancellationToken cancellationToken)
            {
                if (MiniProfiler.Current != null)
                    using (ps.GetTiming("NonQuery", MiniProfiler.Current))
                    {
                        await base.DoExecuteBatchAsync(ps, cancellationToken);
                    }
                else
                    await base.DoExecuteBatchAsync(ps, cancellationToken);
            }
        }
        #endregion


    }
}
