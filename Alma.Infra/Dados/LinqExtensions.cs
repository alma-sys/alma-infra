﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Alma.Infra.Paginador;

namespace Alma.Infra.Dados
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
                    throw new NotSupportedException("Fetch query is currently not supported when using " + Config.ORM.ToString());
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
        public static ListaPaginada<T> ParaListaPaginada<T>(this IQueryable<T> query, int paginaAtual, int tamanhoPagina = ListaPaginada<T>.TamanhoPaginaPadrao) where T : class
        {
            var recordCount = query.Count();
            IList<T> pageRecords;
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
            
            return new ListaPaginada<T>(paginaAtual, tamanhoPagina, recordCount, pageRecords);
        }


        public static ListaPaginada<TResult> Select<T, TResult>(this ListaPaginada<T> query, Func<T, TResult> select)
            where T : class
            where TResult : class
        {
            var paged = new ListaPaginada<TResult>(query.Pagina, query.TamanhoPagina, query.TotalItens, query.Lista.Select(select).ToList());
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
