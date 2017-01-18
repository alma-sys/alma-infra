using System;
using System.Data;
using System.Linq;
using System.Reflection;
using Alma.Infra.Dados;
using Autofac;
using NHibernate;
//[assembly: PreApplicationStartMethod(typeof(Alma.Dados.OrmNHibernate.Config), "Start")]

namespace Alma.Dados.OrmNHibernate
{
    public static class Config
    {
        private static bool _startWasCalled = false;
        public static void Start(ContainerBuilder builder)
        {
            //normalmente eu coloco esse método pra ser chamado automaticamente
            //quando só temos coisas de persistencia no Container do AutoFac.
            //por enquanto será chamado pelo App_Start do web.

            if (_startWasCalled) return;
            _startWasCalled = true;

            if (Alma.Infra.Config.ORM != Alma.Infra.ORM.NHibernate)
                throw new InvalidOperationException("This is not a NHibernate application.");

            Config.SetupLog();

            Config.SetupIoC(builder);
            Alma.Infra.Config.ConfigurarIoC(builder); //chamar o IoC comum a qualquer OrM.

            Config.SetupLinq();
        }

        public static void SetConnectionStringResolver(Func<string, string> connectionStringResolver)
        {
            Repositorio.SetConnectionStringResolver(connectionStringResolver);
        }

        private static void SetupLog()
        {
#if DEBUG
            var logger = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository();
            logger.Name = "NHibernate.SQL";

            var pattern = new log4net.Layout.PatternLayout("%message%newline");
            pattern.Header = "";
            var appender = new log4net.Appender.TraceAppender();
            appender.Layout = pattern;

            logger.Root.Level = log4net.Core.Level.Info;
            logger.Root.RemoveAllAppenders();
            logger.Root.AddAppender(appender);

            log4net.Config.BasicConfigurator.Configure(logger);
#endif
        }


        private static void SetupIoC(ContainerBuilder builder)
        {
            var assemblies = Alma.Infra.Config.AssembliesMapeadas;
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
                    builder.RegisterGeneric(typeof(Repositorio<>))
                         .As(typeof(IRepositorio<>))
                         .WithParameter(new Autofac.Core.ResolvedParameter(
                             (pi, c) => pi.ParameterType == typeof(ISession),
                             (pi, c) => c.ResolveNamed<ISession>(
                                 ResolveConnectionName(pi.Member.DeclaringType.GetGenericArguments()[0]))));
                }
            }
            else
            {
                var key = assemblies.Keys.First();
                builder.RegisterInstance(GetFactory(key, assemblies[key])).As<ISessionFactory>();
                builder.Register(t => GetSession(t.Resolve<ISessionFactory>())).As<ISession>().InstancePerLifetimeScope();
                builder.Register(t => GetCommand(t.Resolve<ISession>())).As<IDbCommand>().InstancePerLifetimeScope();
                builder.RegisterGeneric(typeof(Repositorio<>)).As(typeof(IRepositorio<>));
            }

        }

        internal static string ResolveConnectionName(Type type)
        {
            var assemblies = Alma.Infra.Config.AssembliesMapeadas;
            var assembly = type.Assembly;
            foreach (var key in assemblies.Keys)
            {
                if (assemblies[key].Contains(assembly))
                    return key;
            }
            return null;
        }

        private static void SetupLinq()
        {
            if (Alma.Infra.Config.ORM == Alma.Infra.ORM.NHibernate)
            {
                LinqExtensions.Current = new NhLinqExtensions();

            }
        }

        private static ISessionFactory GetFactory(string connectionKey, Assembly[] assemblies)
        {
            return Repositorio.GetSessionFactory(connectionKey, assemblies);
        }

        private static IDbCommand GetCommand(ISession session)
        {
            return session.Connection.CreateCommand();
        }

        private static ISession GetSession(ISessionFactory factory)
        {
            return factory.OpenSession();
        }
    }
}
