//using Autofac;
//using NHibernate;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Alma.Infra.DependencyResolver
//{
//    public class NHibernateModule : Autofac.Module
//    {
//        protected override void Load(ContainerBuilder builder)
//        {
//            builder
//               .Register(i => BuildSessionFactory())
//               .As<ISessionFactory>()
//               .SingleInstance();

//            builder
//                .Register(i => OpenSession(i.Resolve<ISessionFactory>()))
//                .As<ISession>()
//                .InstancePerRequest();
//        }

//        private ISessionFactory BuildSessionFactory()
//        {
//            return new Alma.Infra.Nhibernate.SessionFactoryBuilder()
//                .WithConnectionString(System.Configuration.ConfigurationManager.ConnectionStrings["ALMA"])
//                //TODO:Melhorar isso
//                .WithMappings(Assembly.Load("RenameThis.Dados").GetExportedTypes())
//                .Build();
//        }

//        private static ISession OpenSession(ISessionFactory sessionFactory)
//        {
//            var session = sessionFactory.OpenSession();

//            session.FlushMode = FlushMode.Commit;

//            return session;
//        }
//    }
//}
