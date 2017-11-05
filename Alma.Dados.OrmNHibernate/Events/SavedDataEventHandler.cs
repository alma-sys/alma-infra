using NHibernate.Event;
using System;
using System.Collections.Generic;
using Alma.Dados.Hooks;
using Alma.Dominio;

namespace Alma.Dados.OrmNHibernate.Events
{
    internal class SavedDataEventHandler : ISaveOrUpdateEventListener, IPostUpdateEventListener, IPostInsertEventListener, IPostDeleteEventListener
    {
#pragma warning disable 0618
        private readonly IEnumerable<IDataHook> hooks;

        public SavedDataEventHandler(IEnumerable<IDataHook> hooks)
        {
            this.hooks = hooks;
        }
#pragma warning restore 0618

        public void OnPostDelete(PostDeleteEvent @event)
        {
            var tipo = @event.Entity.GetType();
            var entity = @event.Entity;
            FireEvents(tipo, entity);
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            var tipo = @event.Entity.GetType();
            var entity = @event.Entity;
            FireEvents(tipo, entity);
        }

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            var tipo = @event.Entity.GetType();
            var entity = @event.Entity;
            FireEvents(tipo, entity);
        }

        public void OnSaveOrUpdate(SaveOrUpdateEvent @event)
        {
            var tipo = @event.Entity.GetType();
            var entity = @event.Entity;
            FireEvents(tipo, entity);
        }

        private void FireEvents(Type tipo, object entity)
        {
            if (!typeof(Entidade).IsAssignableFrom(tipo))
                return;

            foreach (var item in hooks)
            {
                var ifaceType = typeof(IDataHook<>).MakeGenericType(tipo);
                var ifaces = item.GetType().GetGenericInterfaces(ifaceType);

                foreach (var i in ifaces)
                    if (i.GetGenericArguments()[0].IsAssignableFrom(tipo))
                {
                        var handle = item.GetType().GetMethod(nameof(IDataHook<Dominio.Entidade>.OnHandle), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                        handle.Invoke(item, new object[] { entity }); //devo tratar exceptions?
                } 
            }
        }
    }
}
