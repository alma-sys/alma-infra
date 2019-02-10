using Alma.DataAccess.Hooks;
using Autofac;
using NHibernate;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using System.Data;
using System.Linq;

namespace Alma.DataAccess.OrmNHibernate
{
    public class NHibernateModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            SetupLinq();
            SetupLog();


            var assemblies = Alma.Common.Config.MappedAssemblies;
            if (assemblies.Keys.Count > 1)
            {
                foreach (var key in assemblies.Keys)
                {
                    builder.Register
                        (x => GetFactory(key, assemblies[key]))
                        .Named<ISessionFactory>(key)
                        .SingleInstance();

                    builder.Register(c => GetSession(c.ResolveNamed<ISessionFactory>(key)))
                        .Named<ISession>(key)
                        .InstancePerLifetimeScope();

                    //.InstancePerLifetimeScope();
                    builder.RegisterGeneric(typeof(Repository<>))
                         .AsImplementedInterfaces()
                         .WithParameter(new Autofac.Core.ResolvedParameter(
                             (pi, c) => pi.ParameterType == typeof(ISession),
                             (pi, c) => c.ResolveNamed<ISession>(
                                 Alma.Common.Config.ResolveConnectionName(pi.Member.DeclaringType.GetGenericArguments()[0]))))
                         .InstancePerLifetimeScope();
                }
            }
            else
            {
                var key = assemblies.Keys.First();
                builder.RegisterInstance(GetFactory(key, assemblies[key]))
                    .As<ISessionFactory>()
                    .SingleInstance();

                builder.Register(t => GetSession(t.Resolve<ISessionFactory>()))
                    .As<ISession>()
                    .InstancePerLifetimeScope();

                builder.Register(t => GetCommand(t.Resolve<ISession>()))
                    .As<IDbCommand>()
                    .InstancePerLifetimeScope();


                builder.RegisterGeneric(typeof(Repository<>))
                    .As(typeof(IQueryable<>), typeof(IRepository<>))
                    .InstancePerLifetimeScope();
            }


            //builder.RegisterAssemblyTypes(typeof(NHibernateModule).Assembly)
            //   .AssignableTo<IPostDeleteEventListener>()
            //   .AssignableTo<IPostInsertEventListener>()
            //   .AssignableTo<IPostUpdateEventListener>()
            //   .AssignableTo<IPostLoadEventListener>()
            //   .AsSelf()
            //   .InstancePerLifetimeScope();


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
                Events.SavedDataEventHandler.Container = container;
            });

            //builder.RegisterBuildCallback(container =>
            //{
            //    NHibernate.Cfg.Environment.BytecodeProvider =
            //        new AutofacBytecodeProvider(container, new DefaultProxyFactoryFactory(), new DefaultCollectionTypeFactory());
            //});


        }


        private static void SetupLinq()
        {
            if (Alma.Common.Config.Settings.ORM == Common.ORM.NHibernate)
            {
                LinqExtensions.Current = new NhLinqExtensions();

            }
        }

        private static void SetupLog()
        {
            if (Alma.Common.Config.Settings.Logging.Enable)
            {
                var hierarchy = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository(System.Reflection.Assembly.GetExecutingAssembly());
                // Remove any other appenders
                hierarchy.Root.RemoveAllAppenders();
                // define some basic settings for the root
                var rootLogger = hierarchy.Root;
                rootLogger.Level = log4net.Core.Level.Error;


                // This is required, so that we can access the Logger by using 
                // LogManager.GetLogger("NHibernate.SQL") and it can used by NHibernate
                var loggerNH = hierarchy.GetLogger("NHibernate.SQL") as log4net.Repository.Hierarchy.Logger;
                loggerNH.Level = log4net.Core.Level.Debug;
                loggerNH.RemoveAllAppenders();

                // declare a TraceAppender with 5MB per file and max. 10 files
                var pattern = new log4net.Layout.PatternLayout("%message%newline");
                pattern.Header = "";
                var appender = new log4net.Appender.TraceAppender();
                appender.Layout = pattern;
                loggerNH.AddAppender(appender);



                // this is required to tell log4net that we're done 
                // with the configuration, so the logging can start
                hierarchy.Configured = true;
                log4net.Config.BasicConfigurator.Configure(hierarchy);
            }
        }

        private static ISessionFactory GetFactory(string connectionKey, System.Reflection.Assembly[] assemblies)
        {
            return SessionFactory.GetSessionFactory(connectionKey, assemblies);
        }

        private static IDbCommand GetCommand(ISession session)
        {
            var command = session.Connection.CreateCommand();
            if (Alma.Common.Config.Settings.EnableProfiling && command != null && command.GetType() != typeof(ProfiledDbCommand) && MiniProfiler.Current != null)
                command = new ProfiledDbCommand(command, command.Connection, MiniProfiler.Current);

            return command;
        }

        private static ISession GetSession(ISessionFactory factory)
        {
            return factory.WithOptions()
                .OpenSession();
        }

    }
}
