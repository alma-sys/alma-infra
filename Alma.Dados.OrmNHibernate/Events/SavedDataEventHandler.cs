using NHibernate.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alma.Dados.Hooks;
using Alma.Dominio;

namespace Alma.Dados.OrmNHibernate.Events
{
    public class SavedDataEventHandler : ISaveOrUpdateEventListener
    {
        private readonly IEnumerable<ISavedDataHook> hooks;

        public SavedDataEventHandler(IEnumerable<ISavedDataHook> hooks)
        {
            this.hooks = hooks;
        }

        public void OnSaveOrUpdate(SaveOrUpdateEvent @event)
        {
            var tipo = @event.Entity.GetType();
            foreach (var item in hooks)
            {
                if (item.targetType.IsAssignableFrom(tipo))
                {
                    item.Handle(@event.Entity);
                } 
            }
        }
    }
}
