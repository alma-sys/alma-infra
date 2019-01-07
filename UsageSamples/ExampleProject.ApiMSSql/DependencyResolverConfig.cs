using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Alma.ExampleProject.ApiMSSql
{
    internal class DependencyResolverConfig
    {
        public static Autofac.IContainer SetDependencyResolver(IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var builder = new ContainerBuilder();


            builder.Populate(services);

            var assembly = typeof(DependencyResolverConfig).Assembly;

            Alma.Common.Config.Boot(configuration, builder);

            var container = builder.Build();

            return container;
        }

    }
}