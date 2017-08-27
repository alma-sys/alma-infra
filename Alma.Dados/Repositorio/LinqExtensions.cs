using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Alma.Core;
using Alma.Core.Dto;

namespace Alma.Dados
{
    public interface ILinqExtensions
    {
        IQueryable<TQueried> Fetch<TQueried, TRelated>(IQueryable<TQueried> query, Expression<Func<TQueried, TRelated>> relatedObjectSelector);

        //IQueryable<TQueried> FetchMany<TQueried, TRelated>(IQueryable<TQueried> query, Expression<Func<TQueried, IEnumerable<TRelated>>> relatedObjectSelector);

        //IQueryable<TQueried> ThenFetch<TQueried, TFetch, TRelated>(IQueryable<TQueried> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector);

        //IQueryable<TQueried> ThenFetchMany<TQueried, TFetch, TRelated>(IQueryable<TQueried> query, Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector);
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
        {
            return Current.Fetch(query, relatedObjectSelector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="paginaAtual">1-based current page index</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IListaPaginada<T> ParaListaPaginada<T>(this IQueryable<T> query, int paginaAtual, int tamanhoPagina = ListaPaginada<T>.TamanhoPaginaPadrao) where T : class
        {
            var recordCount = query.Count();
            IList<T> pageRecords;

            if (tamanhoPagina != 0)
            {
                //currentPage começa de 1
                do
                {
                    pageRecords = query
                        .Skip((paginaAtual - 1) * tamanhoPagina)
                        .Take(tamanhoPagina)
                        .ToList();

                    if (pageRecords.Count == 0)
                        paginaAtual--;

                } while (pageRecords.Count == 0 && paginaAtual != 0);
            }
            else
            {
                pageRecords = new List<T>();
            }


            return new ListaPaginada<T>(paginaAtual, recordCount, pageRecords, tamanhoPagina);
        }


        public static IListaPaginada<TResult> Select<T, TResult>(this IListaPaginada<T> query, Func<T, TResult> select)
            where T : class
            where TResult : class
        {
            var paged = new ListaPaginada<TResult>(query.Pagina, query.TotalItens, query.Lista.Select(select).ToList(), query.TamanhoPagina);
            return paged;
        }

        //para implementacao e teste futuro.
        //public static IQueryable<TOriginating> FetchMany<TOriginating, TRelated>(this IQueryable<TOriginating> query, Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
        //{
        //    return Current.FetchMany(query, relatedObjectSelector);
        //}

        //public static IQueryable<TOriginating> ThenFetch<TOriginating, TFetch, TRelated>(this IQueryable<TOriginating> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector)
        //{
        //    return Current.ThenFetch<TOriginating, TFetch, TRelated>(query, relatedObjectSelector);
        //}

        //public static IQueryable<TOriginating> ThenFetchMany<TOriginating, TFetch, TRelated>(this IQueryable<TOriginating> query, Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
        //{
        //    return Current.ThenFetchMany<TOriginating, TFetch, TRelated>(query, relatedObjectSelector);
        //}
    }

}
