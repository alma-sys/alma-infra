using System.Data;
using System.Linq;
using Autofac;
using NHibernate;

namespace Alma.Dados.OrmNHibernate
{
    public class NHibernateModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            SetupLinq();
            SetupLog();


            var assemblies = Alma.Dados.Config.AssembliesMapeadas;
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
                    builder.RegisterGeneric(typeof(Repositorio<>))
                         .AsImplementedInterfaces()
                         .WithParameter(new Autofac.Core.ResolvedParameter(
                             (pi, c) => pi.ParameterType == typeof(ISession),
                             (pi, c) => c.ResolveNamed<ISession>(
                                 Config.ResolveConnectionName(pi.Member.DeclaringType.GetGenericArguments()[0]))))
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


                builder.RegisterGeneric(typeof(Repositorio<>))
                    .As(typeof(IQueryable<>), typeof(IRepositorio<>))
                    .InstancePerLifetimeScope();
            }



        }


        private static void SetupLinq()
        {
            if (Alma.Dados.Config.ORM == Alma.Dados.ORM.NHibernate)
            {
                LinqExtensions.Current = new NhLinqExtensions();

            }
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





        private static ISessionFactory GetFactory(string connectionKey, System.Reflection.Assembly[] assemblies)
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

        //public static void SetConnectionStringResolver(Func<string, string> connectionStringResolver)
        //{
        //    Repositorio.SetConnectionStringResolver(connectionStringResolver);
        //}

    }
}
