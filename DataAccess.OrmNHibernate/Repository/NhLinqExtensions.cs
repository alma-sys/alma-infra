using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Alma.DataAccess.OrmNHibernate
{
    public sealed class NhLinqExtensions : ILinqExtensions
    {
        public Task<long> CountAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.LongCountAsync(source, cancellationToken);

        public Task<long> CountAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.LongCountAsync(source, predicate, cancellationToken);

        public IQueryable<TQueried> Fetch<TQueried, TRelated>(IQueryable<TQueried> query, Expression<Func<TQueried, TRelated>> relatedObjectSelector)
            => EagerFetchingExtensionMethods.Fetch<TQueried, TRelated>(query, relatedObjectSelector);

        public Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.FirstAsync(source, predicate, cancellationToken);

        public Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.FirstAsync(source, cancellationToken);

        public Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.FirstOrDefaultAsync(source, predicate, cancellationToken);

        public Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.FirstOrDefaultAsync(source, cancellationToken);

        public Task<TResult> MaxAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.MaxAsync(source, selector, cancellationToken);

        public Task<TSource> MaxAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.MaxAsync(source, cancellationToken);

        public Task<TSource> MinAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.MinAsync(source, cancellationToken);

        public Task<TResult> MinAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.MinAsync(source, selector, cancellationToken);

        public Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.SingleAsync(source, predicate, cancellationToken);

        public Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.SingleAsync(source, cancellationToken);

        public Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.SingleOrDefaultAsync(source, predicate, cancellationToken);

        public Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.SingleOrDefaultAsync(source, cancellationToken);

        public Task<List<TSource>> ToListAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => LinqExtensionMethods.ToListAsync(source, cancellationToken);

    }
}
