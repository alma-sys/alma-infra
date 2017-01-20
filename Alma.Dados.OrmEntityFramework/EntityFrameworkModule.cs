using System.Linq;
using Autofac;

namespace Alma.Dados.OrmEntityFramework
{
    public class EntityFrameworkModule : Module
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
                        (x => new Contexto(key))
                        .Named<IContexto>(key)
                        .SingleInstance();

                    //.InstancePerLifetimeScope();
                    builder.RegisterGeneric(typeof(Repositorio<>))
                         .As(typeof(IRepositorio<>))
                         .WithParameter(new Autofac.Core.ResolvedParameter(
                             (pi, c) => pi.ParameterType == typeof(IContexto),
                             (pi, c) => c.ResolveNamed<IContexto>(
                                 Config.ResolveConnectionName(pi.Member.DeclaringType.GetGenericArguments()[0]))))
                                 .InstancePerRequest();
                }
            }
            else
            {
                var key = assemblies.Keys.First();
                builder.Register(x => new Contexto(key))
                    .As<IContexto>()
                    .SingleInstance();
                builder.RegisterGeneric(typeof(Repositorio<>)).As(typeof(IRepositorio<>))
                                                     .InstancePerRequest();

            }
        }


        private static void SetupLinq()
        {
            if (Alma.Dados.Config.ORM == Alma.Dados.ORM.EntityFramework)
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
