//using System;
//using System.Collections.Generic;
using System.Linq;
using Autofac;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
namespace Alma.Infra.DependencyResolver
{
    public class RepositoriosModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var ass = Alma.Infra.Config.AssembliesMapeadas.Values.SelectMany(x => x).ToArray();

            builder
                .RegisterAssemblyTypes(ass)
                .Where(p => p.Name.StartsWith("RepositorioDe"))
                .AsImplementedInterfaces()
                .InstancePerRequest();

        }
    }
}
