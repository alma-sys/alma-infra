using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Alma.Dominio.Events
{
    internal interface IEventAggregator
    {
        void Publish<TMessageType>(TMessageType message) where TMessageType : IDomainEvent;
        //ISubscription<TMessageType> Subscribe<TMessageType>(Action<TMessageType> action);
        void Subscribe(Type subject, IEventSubscriber subscriber);
        void UnSubscribe(Type subject, IEventSubscriber subscriber);
    }

    internal class EventAggregator : IEventAggregator
    {
        //Essa treta do static, eu preferia que fosse um service no DI
        private static readonly IEventAggregator staticCurrent;

        public static IEventAggregator Current => staticCurrent;
        static EventAggregator() { staticCurrent = new EventAggregator(); }

        private readonly object lockObj = new object();
        private Dictionary<Type, IList> subscribers;

        public EventAggregator()
        {
            subscribers = new Dictionary<Type, IList>();
        }

        public void Publish<TMessageType>(TMessageType message) where TMessageType : IDomainEvent
        {
            Type t = typeof(TMessageType);
            IList sublst;
            if (subscribers.ContainsKey(t))
            {
                lock (lockObj)
                {
                    var lista = subscribers[t];
                    sublst = new List<IEventSubscriber>(lista.Cast<IEventSubscriber>());
                }

                foreach (IEventSubscriber sub in sublst)
                {
                    sub.Handle(message);
                }
            }
        }

        public void Subscribe(Type subject, IEventSubscriber subscriber)
        {
            IList actionlst;

            lock (lockObj)
            {
                if (!subscribers.TryGetValue(subject, out actionlst))
                {
                    actionlst = new List<IEventSubscriber>();
                    subscribers.Add(subject, actionlst);
                }

                actionlst.Add(subscriber);
            }
        }


        public void UnSubscribe(Type subject, IEventSubscriber subscriber)
        {
            IList actionlst;

            lock (lockObj)
            {
                if (subscribers.TryGetValue(subject, out actionlst))
                {
                    actionlst.Remove(subscriber);
                }
            }
        }
    }
}
