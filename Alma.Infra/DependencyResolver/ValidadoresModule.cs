//using Autofac;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

using System.Linq;
using Alma.Infra.Validadores;
using Autofac;
using FluentValidation;
namespace Alma.Infra.DependencyResolver
{
    public class ValidadoresModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = Alma.Infra.Config.AssembliesMapeadas.Values.SelectMany(x => x).ToArray();
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
