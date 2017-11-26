using Alma.Dados.Hooks;
using Alma.Dominio;
using Autofac;
using NHibernate.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Alma.Dados.OrmNHibernate.Events
{
    internal class SavedDataEventHandler : IPostUpdateEventListener, IPostInsertEventListener, IPostDeleteEventListener, IPostLoadEventListener
    {
        //TODO: Tentar fazer com que isso seja via SCOPE.

        public static IContainer Container { get; internal set; }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            var tipo = @event.Entity.GetType();
            var entity = @event.Entity;
            Dispatch(tipo, typeof(IDeletedDataHook<>), entity);
        }

        public async Task OnPostDeleteAsync(PostDeleteEvent @event, CancellationToken cancellationToken)
        {
            await Task.Run(() => OnPostDelete(@event), cancellationToken);
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            var tipo = @event.Entity.GetType();
            var entity = @event.Entity;
            Dispatch(tipo, typeof(ICreatedDataHook<>), entity);
        }

        public async Task OnPostInsertAsync(PostInsertEvent @event, CancellationToken cancellationToken)
        {
            await Task.Run(() => OnPostInsert(@event), cancellationToken);
        }

        public void OnPostLoad(PostLoadEvent @event)
        {
            var tipo = @event.Entity.GetType();
            var entity = @event.Entity;
            Dispatch(tipo, typeof(ILoadDataHook<>), entity);
        }

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            var tipo = @event.Entity.GetType();
            var entity = @event.Entity;
            Dispatch(tipo, typeof(IUpdatedDataHook<>), entity);
        }

        public async Task OnPostUpdateAsync(PostUpdateEvent @event, CancellationToken cancellationToken)
        {
            await Task.Run(() => OnPostUpdate(@event), cancellationToken);
        }

        private void Dispatch(Type tipo, Type interfaceType, object entity)
        {
            if (!typeof(Entidade).IsAssignableFrom(tipo))
                return;

            var handle = typeof(IDataHook<>).MakeGenericType(tipo)
                .GetMethod(nameof(IDataHook<Entidade>.Handle));

            var concreteIFaceType = interfaceType.MakeGenericType(tipo);
            var inumerable = typeof(IEnumerable<>).MakeGenericType(concreteIFaceType);
            var handlers = Container.Resolve(inumerable) as IEnumerable;
            foreach (var item in handlers)
            {
                if (Config.AtivarLog)
                    Trace.WriteLine($"Executado handler {item} para {interfaceType.Name}...", nameof(SavedDataEventHandler));
                handle.Invoke(item, new object[] { entity }); //devo tratar exceptions?
            }
        }
    }
}
