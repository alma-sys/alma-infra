using Autofac;
using FluentValidation;
using System.Linq;

namespace Alma.Domain.Validators
{
    public class ValidatorsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = Common.Config.MappedAssemblies.Values.SelectMany(x => x).ToArray();
            foreach (var ass in assemblies)
            {
                var findValidatorsInAssembly = AssemblyScanner.FindValidatorsInAssembly(ass);

                foreach (AssemblyScanner.AssemblyScanResult item in findValidatorsInAssembly)
                {
                    var iface = typeof(FluentValidation.IValidator<>).MakeGenericType(item.InterfaceType.GetGenericArguments()[0]);
                    builder
                        .RegisterType(item.ValidatorType)
                        .AsImplementedInterfaces()
                        .As(iface);

                }

            }

            builder
                .RegisterGeneric(typeof(Validator<>))
                .InstancePerLifetimeScope()
                .As(typeof(IValidator<>));


        }
    }
}
