using Alma.Common.Dto;
using System;
using System.Linq;

namespace Alma.Common
{
    public static class LinqExtensions
    {
        public static IPagedList<TResult> Select<T, TResult>(this IPagedList<T> query, Func<T, TResult> select)
            where T : class
            where TResult : class
        {
            var paged = new PagedListDto<TResult>(query.CurrentPage, query.TotalCount, query.List.Select(select).ToList(), query.PageSize);
            return paged;
        }
    }
}
