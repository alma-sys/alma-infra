using Autofac;
using System.Collections.Generic;
using System.Linq;

namespace Alma.Dominio.Events
{
    public interface IEventDispatcher
    {
        void Dispatch<TEvent>(TEvent eventToDispatch) where TEvent : IDomainEvent;
    }

    //public static class Dispatcher
    //{
    //    internal static IEventDispatcher Current { get; set; }

    //    /// <summary>
    //    /// Dispatch singleton events only
    //    /// </summary>
    //    /// <typeparam name="TEvent"></typeparam>
    //    /// <param name="eventToRaise"></param>
    //    public static void Raise<TEvent>(TEvent eventToRaise) where TEvent : IDomainEvent
    //    {
    //        Current.Dispatch(eventToRaise);
    //    }
    //}

    internal class EventDispatcher : IEventDispatcher
    {
        private readonly ILifetimeScope scope;

        public EventDispatcher(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        public void Dispatch<TEvent>(TEvent eventToDispatch) where TEvent : IDomainEvent
        {
            var handlers = scope.Resolve<IEnumerable<IEventSubscriber<TEvent>>>().ToList();
            handlers.ForEach(handler => handler.Handle(eventToDispatch));
        }
    }
}
