using Alma.DataAccess.Hooks;
using Alma.DataAccess.MongoMapping.Conventions;
using Autofac;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Linq;

namespace Alma.DataAccess.MongoMapping
{
    public class MongoModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            SetupConventions();

            SetupLinq();
            SetupLog();


            var assemblies = Config.MappedAssemblies;
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

                    builder.RegisterGeneric(typeof(Repository<>))
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

                builder.RegisterGeneric(typeof(Repository<>))
                    .As(typeof(IQueryable<>), typeof(IRepository<>))
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

        }

        private void SetupConventions()
        {
            var namingPack = new ConventionPack
             {
              new SeperateWordsNamingConvention(),
              new LowerCaseElementNameConvetion()
             };
            ConventionRegistry.Register(nameof(namingPack), namingPack, type => true /*pack for all */);
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
            if (Config.EnableLog)
            {

            }
        }

        private static IMongoClient GetClient(string connectionKey)
        {
            var connectionString = Alma.Common.Config.ConnectionStrings[connectionKey];
            if (connectionString == null || string.IsNullOrWhiteSpace(connectionString.ConnectionString))
                throw new System.Configuration.ConfigurationErrorsException($"Cannot find connection string setting for {connectionKey}");

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
            var connectionString = Alma.Common.Config.ConnectionStrings[connectionKey];
            if (connectionString == null || string.IsNullOrWhiteSpace(connectionString.ConnectionString))
                throw new System.Configuration.ConfigurationErrorsException($"Cannot find connection string setting for {connectionKey}");
            return connectionString.ConnectionString;
        }

        private static string GetConnectionDatabase(string connectionKey)
        {
            var connectionString = GetConnectionString(connectionKey);
            var str = connectionString.Split('/');
            if (str.Length < 2)
                throw new System.Configuration.ConfigurationErrorsException($"Cannot find database on connection string setting for {connectionKey}");

            var db_name = str.Last();

            if (db_name.Contains("?"))
                db_name = db_name.Substring(0, db_name.IndexOf("?"));

            return db_name;
        }

    }
}
