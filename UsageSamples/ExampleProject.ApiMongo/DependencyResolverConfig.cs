using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Alma.ExampleProject.Api
{
    internal class DependencyResolverConfig
    {
        public static Autofac.IContainer SetDependencyResolver(IServiceCollection services, IConfiguration configuration)
        {
            var builder = new ContainerBuilder();

            builder.Populate(services);

            Alma.Common.Config.Boot(configuration, builder);


            var container = builder.Build();

            return container;
        }

    }
}