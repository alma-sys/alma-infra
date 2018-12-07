using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Alma.Exemplo.ApiMSSql
{
    internal class DependencyResolverConfig
    {
        public static Autofac.IContainer SetDependencyResolver(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            builder.Populate(services);

            var assembly = typeof(DependencyResolverConfig).Assembly;

            builder.RegisterAssemblyModules(Alma.Common.Config.MappedAssemblies.SelectMany(x => x.Value).ToArray());

            //builder.RegisterModule<Alma.Dominio.Events.EventModule>(); // por enquanto só no autofac do hangfire.
            builder.RegisterModule<Alma.Dados.OrmNHibernate.NHibernateModule>();

            var container = builder.Build();

            return container;
        }

    }
}