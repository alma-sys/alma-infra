using Autofac;
using Autofac.Builder;
using Autofac.Features.Scanning;
using System;
using System.Linq;

namespace Alma.Dados
{
    public class RepositoriosModule : Module
    {
        private Func<IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>, object[], IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>> lifetimeScope;

        public RepositoriosModule() : this(Autofac.RegistrationExtensions.InstancePerRequest<object, ScanningActivatorData, DynamicRegistrationStyle>)
        {

        }

        public RepositoriosModule(Func<IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>, object[], IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>> lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var ass = Core.Config.AssembliesMapeadas.Values.SelectMany(x => x).ToArray();

            var reg = builder
                .RegisterAssemblyTypes(ass)
                .Where(p => p.Name.StartsWith("RepositorioDe")) //por convenção
                .AsImplementedInterfaces();

            this.lifetimeScope(reg, new object[] { });

        }
    }
}
