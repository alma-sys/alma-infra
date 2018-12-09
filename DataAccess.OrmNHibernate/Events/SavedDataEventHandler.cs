using Alma.DataAccess.Hooks;
using Alma.Domain;
using Autofac;
using NHibernate.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Alma.DataAccess.OrmNHibernate.Events
{
    internal class SavedDataEventHandler : IPostUpdateEventListener, IPostInsertEventListener, IPostDeleteEventListener, IPostLoadEventListener
    {
        //TODO: Try to do it by scope

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
            if (!typeof(Entity).IsAssignableFrom(tipo))
                return;

            var handle = typeof(IDataHook<>).MakeGenericType(tipo)
                .GetMethod(nameof(IDataHook<Entity>.Handle));

            var concreteIFaceType = interfaceType.MakeGenericType(tipo);
            var inumerable = typeof(IEnumerable<>).MakeGenericType(concreteIFaceType);
            var handlers = Container.Resolve(inumerable) as IEnumerable;
            foreach (var item in handlers)
            {
                if (Config.EnableLog)
                    Trace.WriteLine($"Executing handler {item} to {interfaceType.Name}...", nameof(SavedDataEventHandler));
                handle.Invoke(item, new object[] { entity }); //devo tratar exceptions?
            }
        }
    }
}
