using System.Linq;
using Autofac;
using FluentValidation;

namespace Alma.Dominio.Validadores
{
    public class ValidadoresModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = Common.Config.MappedAssemblies.Values.SelectMany(x => x).ToArray();
            foreach (var ass in assemblies)
            {
                var findValidatorsInAssembly = AssemblyScanner.FindValidatorsInAssembly(ass);

                foreach (AssemblyScanner.AssemblyScanResult item in findValidatorsInAssembly)
                {
                    var iface = typeof(IValidator<>).MakeGenericType(item.InterfaceType.GetGenericArguments()[0]);
                    builder
                        .RegisterType(item.ValidatorType)
                        .AsImplementedInterfaces()
                        .As(iface);

                }

            }

            builder
                .RegisterGeneric(typeof(Validador<>))
                .InstancePerLifetimeScope()
                .As(typeof(IValidador<>));


        }
    }
}
