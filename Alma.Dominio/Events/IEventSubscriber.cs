using System;

namespace Alma.Dominio.Events
{
    /// <summary>
    /// Representa um subscriber de evento
    /// </summary>
    public interface IEventSubscriber<T> where T : IDomainEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        void OnHandle(T arg);
    }
}
