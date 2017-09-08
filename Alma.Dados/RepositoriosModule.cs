using System.Linq;
using Autofac;

namespace Alma.Dados
{
    public class RepositoriosModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var ass = Core.Config.AssembliesMapeadas.Values.SelectMany(x => x).ToArray();

            builder
                .RegisterAssemblyTypes(ass)
                .Where(p => p.Name.StartsWith("RepositorioDe"))
                .AsImplementedInterfaces()
                .InstancePerRequest();

        }
    }
}
