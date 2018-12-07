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

        public RepositoriosModule() : this(null)
        {

        }

        public RepositoriosModule(Func<IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>, object[], IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>> lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var ass = Common.Config.MappedAssemblies.Values.SelectMany(x => x).ToArray();

            var reg = builder
                .RegisterAssemblyTypes(ass)
                .Where(p => p.Name.StartsWith("RepositorioDe")) //por convenção
                .AsImplementedInterfaces();
            if (this.lifetimeScope != null)
            {
                this.lifetimeScope(reg, new object[] { });
            }
            else
                reg.InstancePerLifetimeScope();
        }
    }
}
