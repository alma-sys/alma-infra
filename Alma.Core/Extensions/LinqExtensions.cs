using Alma.Core.Dto;
using System;
using System.Linq;

namespace Alma.Core
{
    public static class LinqExtensions
    {
        public static IListaPaginada<TResult> Select<T, TResult>(this IListaPaginada<T> query, Func<T, TResult> select)
            where T : class
            where TResult : class
        {
            var paged = new ListaPaginadaDto<TResult>(query.Pagina, query.TotalItens, query.Lista.Select(select).ToList(), query.TamanhoPagina);
            return paged;
        }
    }
}
