using Alma.Domain.Repositories;
using Autofac;
using Autofac.Builder;
using Autofac.Features.Scanning;
using System;
using System.Linq;

namespace Alma.DataAccess
{
    public class DataAccessModule : Module
    {
        private Func<IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>, object[], IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>> lifetimeScope;

        public DataAccessModule() : this(null)
        {

        }

        public DataAccessModule(Func<IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>, object[], IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>> lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var ass = Common.Config.MappedAssemblies.Values.SelectMany(x => x).ToArray();

            var reg = builder
                .RegisterAssemblyTypes(ass)
                .AsClosedTypesOf(typeof(IDomainRepository<>))
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
