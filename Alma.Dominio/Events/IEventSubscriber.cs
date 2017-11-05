using System;

namespace Alma.Dominio.Events
{
    /// <summary>
    /// Representa um subscriber de evento
    /// </summary>
    public interface IEventSubscriber
    {
        Type SubscriptionSubject { get; }
        void Handle(IDomainEvent arg);
    }
}
