using Autofac;
using Autofac.Core;
using System.Linq;

namespace Alma.Dominio.Events
{
    public class EventModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = Core.Config.AssembliesMapeadas.Values.SelectMany(x => x).ToArray();

            builder
             .RegisterAssemblyTypes(assemblies)
             .Where(p => typeof(IEventSubscriber).IsAssignableFrom(p)) //por convenção ao invés de por interfaces.
             .AsImplementedInterfaces()
             .SingleInstance()

             .AutoActivate()
             .OnActivated(Subscribe);
        }

        private void Subscribe(IActivatedEventArgs<object> obj)
        {
            var sub = (IEventSubscriber)obj.Instance;
            EventAggregator.Current.Subscribe(sub.SubscriptionSubject, sub);
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
