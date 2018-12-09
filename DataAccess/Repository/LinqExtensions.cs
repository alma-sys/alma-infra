using Alma.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Alma.DataAccess
{
    public interface ILinqExtensions
    {
        IQueryable<TQueried> Fetch<TQueried, TRelated>(IQueryable<TQueried> query, Expression<Func<TQueried, TRelated>> relatedObjectSelector);

        //IQueryable<TQueried> FetchMany<TQueried, TRelated>(IQueryable<TQueried> query, Expression<Func<TQueried, IEnumerable<TRelated>>> relatedObjectSelector);

        //IQueryable<TQueried> ThenFetch<TQueried, TFetch, TRelated>(IQueryable<TQueried> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector);

        //IQueryable<TQueried> ThenFetchMany<TQueried, TFetch, TRelated>(IQueryable<TQueried> query, Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector);


        Task<long> CountAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken));
        Task<long> CountAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
        Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
        Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken));
        Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
        Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken));
        Task<TResult> MaxAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken));
        Task<TSource> MaxAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken));
        Task<TSource> MinAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken));
        Task<TResult> MinAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken));
        Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
        Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken));
        Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
        Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TSource>> ToListAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken));

    }


    public static class LinqExtensions
    {
        private static ILinqExtensions _Current = null;
        public static ILinqExtensions Current
        {
            get
            {
                if (_Current == null)
                    throw new NotSupportedException("Linq extensions is not supported when using this ORM.");
                else
                    return _Current;
            }
            set
            {
                _Current = value;
            }
        }

        public static IQueryable<TOriginating> Fetch<TOriginating, TRelated>(this IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
            => Current.Fetch(query, relatedObjectSelector);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="currentPage">1-based current page index</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> query, int currentPage, int pageSize = PagedList<T>.DefaultPageSize) where T : class
        {
            var recordCount = query.Count();
            IList<T> pageRecords;

            if (pageSize != 0)
            {
                //currentPage starts from 1
                do
                {
                    pageRecords = query
                        .Skip((currentPage - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    if (pageRecords.Count == 0)
                        currentPage--;

                } while (pageRecords.Count == 0 && currentPage != 0);
            }
            else
            {
                pageRecords = new List<T>();
            }


            return new PagedList<T>(currentPage, recordCount, pageRecords, pageSize);
        }




        public static Task<long> CountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => Current.CountAsync(source, cancellationToken);

        public static Task<long> CountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
            => Current.CountAsync(source, predicate, cancellationToken);

        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
            => Current.FirstAsync(source, predicate, cancellationToken);

        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => Current.FirstAsync(source, cancellationToken);

        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
            => Current.FirstOrDefaultAsync(source, predicate, cancellationToken);

        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => Current.FirstOrDefaultAsync(source, cancellationToken);

        public static Task<TResult> MaxAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
            => Current.MaxAsync(source, selector, cancellationToken);

        public static Task<TSource> MaxAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => Current.MaxAsync(source, cancellationToken);

        public static Task<TSource> MinAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => Current.MinAsync(source, cancellationToken);

        public static Task<TResult> MinAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
            => Current.MinAsync(source, selector, cancellationToken);

        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
            => Current.SingleAsync(source, predicate, cancellationToken);

        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => Current.SingleAsync(source, cancellationToken);

        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
            => Current.SingleOrDefaultAsync(source, predicate, cancellationToken);

        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => Current.SingleOrDefaultAsync(source, cancellationToken);

        public static Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
            => Current.ToListAsync(source, cancellationToken);


    }

}
