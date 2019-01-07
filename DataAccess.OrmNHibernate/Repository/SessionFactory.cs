using Alma.Common;
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
            var sett = Alma.Common.Config.Settings;
            var conn = sett.GetConnectionString(connectionKey);
            switch (conn.Provider)
            {
                case DBMS.MsSql:

                    cfg.DataBaseIntegration(db =>
                    {
                        db.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                        db.Dialect<NHibernate.Dialect.MsSql2008Dialect>();
                        //db.Driver<DriverSqlClient>();
                        db.Driver<NHibernate.Driver.SqlClientDriver>();
                        db.ConnectionString = conn.ConnectionString;
                        db.ConnectionProvider<ConnectionProvider>();
                        db.PrepareCommands = sett.PrepareCommands;

                        db.LogFormattedSql = true;
                        db.LogSqlInConsole = false;
                        if (sett.IsolationLevel != null)
                            db.IsolationLevel = sett.IsolationLevel.Value;
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
                        db.ConnectionString = conn.ConnectionString;
                        db.ConnectionProvider<ConnectionProvider>();
                        db.PrepareCommands = sett.PrepareCommands;
                        db.LogFormattedSql = true;
                        db.LogSqlInConsole = false;
                        if (sett.IsolationLevel != null)
                            db.IsolationLevel = sett.IsolationLevel.Value;
                        //UseReflectionOptimizer
                    });
                    break;
                case DBMS.Oracle:
                    cfg.DataBaseIntegration(db =>
                    {
                        db.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                        db.Dialect<NHibernate.Dialect.Oracle10gDialect>();
                        if (true /* isManagedOracle */)
                            db.Driver<NHibernate.Driver.OracleManagedDataClientDriver>();
                        else
                            db.Driver<NHibernate.Driver.OracleDataClientDriver>();
                        db.ConnectionString = conn.ConnectionString;
                        db.ConnectionProvider<ConnectionProvider>();
                        db.PrepareCommands = sett.PrepareCommands;
                        db.LogFormattedSql = true;
                        db.LogSqlInConsole = false;
                        if (sett.IsolationLevel != null)
                            db.IsolationLevel = sett.IsolationLevel.Value;
                        db.Batcher<OracleLoggingBatchingBatcherFactory>();
                        //UseReflectionOptimizer
                    });
                    break;
                case DBMS.MySql:

                    cfg.DataBaseIntegration(db =>
                    {
                        db.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                        db.Dialect<NHibernate.Dialect.MySQL55Dialect>();
                        //db.Driver<DriverMySql>();
                        db.Driver<NHibernate.Driver.MySqlDataDriver>();
                        db.ConnectionString = conn.ConnectionString;
                        db.ConnectionProvider<ConnectionProvider>();
                        db.PrepareCommands = sett.PrepareCommands;
                        db.LogFormattedSql = true;
                        db.LogSqlInConsole = false;
                        if (sett.IsolationLevel != null)
                            db.IsolationLevel = sett.IsolationLevel.Value;
                        //UseReflectionOptimizer
                    });

                    break;
                default:
                    throw new NotImplementedException("Not implemented provider: " + conn.Provider);
            }

            if (sett.Logging.Enable)
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
                if (ex.Message.Contains("NHibernate.Driver") && (conn.Provider == DBMS.Oracle))
                {
                    throw new System.Configuration.ConfigurationErrorsException(
@"Cannot find Oracle.DataAcces or Oracle.ManagedDataAccess binaries in GAC or working directory. 

Minimum Unmanaged Version: Oracle Client 11.2 with ODP.NET if using Full Framework 4.0/4.5. 
Remove all installed versions and install the required version and try again.", ex);
                }
                else
                {
                    throw;
                }
            }

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
                    throw new AggregateException("Multiple exceptions occurred while executing database migrations. Check the inner list of exceptions.", update.Exceptions.ToArray());
            }
            catch (Exception)
            {

                throw;
            }

            return config;
        }

        private static void AddFilters(Configuration cfg, IEnumerable<Type> types)
        {
            var logger = Alma.Common.Config.Settings.Logging.Enable ? (Action<string>)((string text) => Trace.WriteLine(text, nameof(AddFilters))) : null;

            var filters = types.Where(x => typeof(Mapper.GlobalFilterMapping).IsAssignableFrom(x)).ToArray();
            logger?.Invoke($"Registering {filters.Length} repository global filters.");
            foreach (var fmapType in filters)
            {
                if (fmapType.IsAbstract)
                    continue;
                var map = (Mapper.GlobalFilterMapping)Activator.CreateInstance(fmapType);
                foreach (var fd in map.filters)
                    cfg.FilterDefinitions.Add(fd);
            }
        }
        private static void AddEvents(Configuration cfg, IEnumerable<Type> types)
        {
            var logger = Alma.Common.Config.Settings.Logging.Enable ? (Action<string>)((string text) => Trace.WriteLine(text, nameof(AddEvents))) : null;

            logger?.Invoke($"Registering {nameof(SavedDataEventHandler)} for listeners.");

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
            hbm.defaultlazy = Alma.Common.Config.Settings.EnableLazyLoad;

            cfg.AddMapping(hbm);
            if (Alma.Common.Config.Settings.ExecuteMigrations)
                cfg.AddSchemaValidationAndMigration();
        }


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
