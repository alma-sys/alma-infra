using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Alma.Exemplo.Api
{
    internal class DependencyResolverConfig
    {
        public static Autofac.IContainer SetDependencyResolver(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            builder.Populate(services);

            var assembly = typeof(DependencyResolverConfig).Assembly;

            builder.RegisterAssemblyModules(Alma.Core.Config.AssembliesMapeadas.SelectMany(x => x.Value).ToArray());

            builder.RegisterModule<Alma.Dados.MongoMapping.MongoModule>();


            var container = builder.Build();

            return container;
        }

    }
}