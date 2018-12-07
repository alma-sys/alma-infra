using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;

namespace Alma.Dados.OrmNHibernate
{
    public sealed class NhLinqExtensions : ILinqExtensions
    {
        public IQueryable<TQueried> Fetch<TQueried, TRelated>(IQueryable<TQueried> query, Expression<Func<TQueried, TRelated>> relatedObjectSelector)
        {
            return EagerFetchingExtensionMethods.Fetch<TQueried, TRelated>(query, relatedObjectSelector);
        }

        //por enquanto somente Fetch.
        //as demais para implementação e teste futuro.

        //public IQueryable<TQueried> FetchMany<TQueried, TRelated>(IQueryable<TQueried> query, Expression<Func<TQueried, IEnumerable<TRelated>>> relatedObjectSelector)
        //{
        //    return EagerFetchingExtensionMethods.FetchMany<TQueried, TRelated>(query, relatedObjectSelector);
        //}

        //public IQueryable<TQueried> ThenFetch<TQueried, TFetch, TRelated>(IQueryable<TQueried> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector)
        //{
        //    var queryNh = query as INhFetchRequest<TQueried, TFetch>;
        //    return EagerFetchingExtensionMethods.ThenFetch<TQueried, TFetch, TRelated>(queryNh, relatedObjectSelector);
        //}

        //public IQueryable<TQueried> ThenFetchMany<TQueried, TFetch, TRelated>(IQueryable<TQueried> query, Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
        //{
        //    var queryNh = query as INhFetchRequest<TQueried, TFetch>;
        //    return EagerFetchingExtensionMethods.ThenFetchMany<TQueried, TFetch, TRelated>(queryNh, relatedObjectSelector);
        //}
    }
}
