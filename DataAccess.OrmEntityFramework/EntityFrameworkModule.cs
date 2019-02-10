using Autofac;
using System.Linq;

namespace Alma.DataAccess.OrmEntityFramework
{
    public class EntityFrameworkModule : Module
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
                        (x => new EFDbContext(key))
                        .Named<EFDbContext>(key)
                        .SingleInstance();

                    //.InstancePerLifetimeScope();
                    builder.RegisterGeneric(typeof(Repository<>))
                         .As(typeof(IRepository<>))
                         .WithParameter(parameter: new Autofac.Core.ResolvedParameter(
                             (pi, c) => pi.ParameterType == typeof(EFDbContext),
                             (pi, c) => c.ResolveNamed<EFDbContext>(
                                 Alma.Common.Config.ResolveConnectionName(pi.Member.DeclaringType.GetGenericArguments()[0]))))
                                 .InstancePerRequest();
                }
            }
            else
            {
                var key = assemblies.Keys.First();
                builder.Register(x => new EFDbContext(key))
                    .As<EFDbContext>()
                    .SingleInstance();
                builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>))
                                                     .InstancePerRequest();

            }
        }


        private static void SetupLinq()
        {
            if (Alma.Common.Config.Settings.ORM == Common.ORM.EntityFramework)
            {
                LinqExtensions.Current = new EfLinqExtensions();
            }
        }

        private static void SetupLog()
        {
#if DEBUG
#endif
        }

    }
}
