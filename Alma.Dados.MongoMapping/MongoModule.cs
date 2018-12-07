using Alma.Dados.Hooks;
using Alma.Dados.MongoMapping.Conventions;
using Alma.Dados.MongoMapping.Dados;
using Autofac;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Configuration;
using System.Linq;

namespace Alma.Dados.MongoMapping
{
    public class MongoModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            SetupConventions();

            SetupLinq();
            SetupLog();


            var assemblies = Config.AssembliesMapeadas;
            var mappingType = typeof(ClassMapping<>);

            if (assemblies.Keys.Count > 1)
            {
                foreach (var key in assemblies.Keys)
                {
                    builder.Register(c => GetClient(key))
                        .Named<IMongoClient>(key)
                        .InstancePerLifetimeScope();

                    builder.Register(c => GetDatabase(c.ResolveNamed<IMongoClient>(key), key))
                        .Named<IMongoDatabase>(key)
                        .InstancePerLifetimeScope();

                    builder.RegisterGeneric(typeof(Repositorio<>))
                         .AsImplementedInterfaces()
                         .WithParameter(new Autofac.Core.ResolvedParameter(
                             (pi, c) => pi.ParameterType == typeof(IMongoDatabase),
                             (pi, c) => c.ResolveNamed<IMongoDatabase>(
                                 Config.ResolveConnectionName(pi.Member.DeclaringType.GetGenericArguments()[0]))))
                         .InstancePerLifetimeScope();



                    builder.RegisterAssemblyTypes(assemblies[key])
                        .AsClosedTypesOf(mappingType)
                        .SingleInstance()
                        .AutoActivate();

                }
            }
            else
            {
                var key = assemblies.Keys.First();

                builder.Register(c => GetClient(key))
                    .As<IMongoClient>()
                    .InstancePerLifetimeScope();

                builder.Register(c => GetDatabase(c.Resolve<IMongoClient>(), key))
                    .As<IMongoDatabase>()
                    .InstancePerLifetimeScope();

                builder.RegisterGeneric(typeof(Repositorio<>))
                    .As(typeof(IQueryable<>), typeof(IRepositorio<>))
                     .InstancePerLifetimeScope();

                builder.RegisterAssemblyTypes(assemblies[key])
                    .AsClosedTypesOf(mappingType)
                    .SingleInstance()
                    .AutoActivate();
            }


            var handlerType = typeof(IDataHook<>);
            foreach (var k in assemblies.Keys)
            {
                var assembly = assemblies[k];

                builder.RegisterAssemblyTypes(assembly)
                    .AsClosedTypesOf(handlerType)
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }

            builder.RegisterBuildCallback(container =>
            {
                //Events.SavedDataEventHandler.Container = container;
            });

            //builder.RegisterBuildCallback(container =>
            //{
            //    NHibernate.Cfg.Environment.BytecodeProvider =
            //        new AutofacBytecodeProvider(container, new DefaultProxyFactoryFactory(), new DefaultCollectionTypeFactory());
            //});
        }

        private void SetupConventions()
        {
            var cp = new ConventionPack
             {
              new SeperateWordsNamingConvention(),
              new LowerCaseElementNameConvetion()
             };
            ConventionRegistry.Register("nomenclatura", cp, type => true /*pack para todos */);
        }

        private static void SetupLinq()
        {
            if (Config.ORM == ORM.MongoMapping)
            {
                //LinqExtensions.Current = new MongoLinqExtensions();

            }
        }

        private static void SetupLog()
        {
            if (Config.AtivarLog)
            {
                //var hierarchy = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository();
                //// Remove any other appenders
                //hierarchy.Root.RemoveAllAppenders();
                //// define some basic settings for the root
                //var rootLogger = hierarchy.Root;
                //rootLogger.Level = log4net.Core.Level.Error;

                //// declare a TraceAppender with 5MB per file and max. 10 files
                //var pattern = new log4net.Layout.PatternLayout("%message%newline");
                //pattern.Header = "";
                //var appender = new log4net.Appender.TraceAppender();
                //appender.Layout = pattern;
                //rootLogger.AddAppender(appender);

                //// This is required, so that we can access the Logger by using 
                //// LogManager.GetLogger("NHibernate.SQL") and it can used by NHibernate
                //var loggerNH = hierarchy.GetLogger("NHibernate.SQL") as log4net.Repository.Hierarchy.Logger;
                //loggerNH.Level = log4net.Core.Level.Debug;


                //// this is required to tell log4net that we're done 
                //// with the configuration, so the logging can start
                //hierarchy.Configured = true;
                //log4net.Config.BasicConfigurator.Configure(hierarchy);
            }
        }

        private static IMongoClient GetClient(string connectionKey)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionKey];
            if (connectionString == null || string.IsNullOrWhiteSpace(connectionString.ConnectionString))
                throw new ConfigurationErrorsException($"Cannot find connection string setting for {connectionKey}");

            var session = new MongoClient(connectionString.ConnectionString);

            return session;
        }

        private static IMongoDatabase GetDatabase(IMongoClient client, string key)
        {
            var db_name = GetConnectionDatabase(key);

            var db = client.GetDatabase(db_name);

            return db;
        }

        private static string GetConnectionString(string connectionKey)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionKey];
            if (connectionString == null || string.IsNullOrWhiteSpace(connectionString.ConnectionString))
                throw new ConfigurationErrorsException($"Cannot find connection string setting for {connectionKey}");
            return connectionString.ConnectionString;
        }

        private static string GetConnectionDatabase(string connectionKey)
        {
            var connectionString = GetConnectionString(connectionKey);
            var str = connectionString.Split('/');
            if (str.Length < 2)
                throw new ConfigurationErrorsException($"Cannot find database on connection string setting for {connectionKey}");

            var db_name = str.Last();

            if (db_name.Contains("?"))
                db_name = db_name.Substring(0, db_name.IndexOf("?"));

            return db_name;
        }

        //public static void SetConnectionStringResolver(Func<string, string> connectionStringResolver)
        //{
        //    Repositorio.SetConnectionStringResolver(connectionStringResolver);
        //}

    }
}
