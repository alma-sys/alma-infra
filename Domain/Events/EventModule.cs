using Autofac;
using System.Linq;
using Alma.Common;

namespace Alma.Dominio.Events
{
    public class EventModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = Common.Config.AssembliesMapeadas.Values.SelectMany(x => x).ToArray();

            var handlerType = typeof(IEventSubscriber<>);

            builder.RegisterAssemblyTypes(assemblies)
                .AsClosedTypesOf(handlerType)
                .InstancePerLifetimeScope();

            builder
                .RegisterType<EventDispatcher>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
                //.SingleInstance()
                //.AutoActivate()
                //.OnActivated(x => Dispatcher.Current = x.Instance);

        }


    }
}
