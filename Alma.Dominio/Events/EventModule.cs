using Autofac;
using Autofac.Core;
using System.Linq;
using Alma.Core;

namespace Alma.Dominio.Events
{
    public class EventModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = Core.Config.AssembliesMapeadas.Values.SelectMany(x => x).ToArray();

            builder
             .RegisterAssemblyTypes(assemblies)
             .Where(p => p.IsGenericTypeOf(typeof(IEventSubscriber<>))) //por convenção ao invés de por interfaces.
             .AsImplementedInterfaces()
             .SingleInstance()

             .AutoActivate()
             .OnActivated(Subscribe);
        }

        private void Subscribe(IActivatedEventArgs<object> obj)
        {
            var sub = obj.Instance;
            var ifaces = sub.GetType().GetGenericInterfaces(typeof(IEventSubscriber<>));
            foreach (var iface in ifaces)
                EventAggregator.Current.Subscribe(iface.GetGenericArguments()[0], sub);
        }

        /// <summary>
        /// Dispara um evento de domínio manualmente.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evento"></param>
        public static void Raise<TEvent>(TEvent evento) where TEvent : IDomainEvent
        {
            var registry = EventAggregator.Current;

            registry.Publish(evento);
        }
    }
}
