using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Alma.DataAccess.OrmNHibernate
{
    public sealed class NhLinqExtensions : ILinqExtensions
    {
        public Task<long> CountAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public IQueryable<TQueried> Fetch<TQueried, TRelated>(IQueryable<TQueried> query, Expression<Func<TQueried, TRelated>> relatedObjectSelector)
        {
            return EagerFetchingExtensionMethods.Fetch<TQueried, TRelated>(query, relatedObjectSelector);
        }

        public Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TResult> MaxAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TSource> MaxAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TSource> MinAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TResult> MinAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<List<TSource>> ToListAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
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
