using System.Collections.Generic;
using System.Collections.ObjectModel;
using Alma.Common;

namespace Alma.Dados
{
    class ListaPaginada<T> : IPagedList<T> where T : class
    {
        public const int TamanhoPaginaPadrao = 10;

        public ListaPaginada(int pagina, long totalItens, IList<T> lista, int tamanhoPagina = TamanhoPaginaPadrao)
        {
            this.CurrentPage = pagina;
            this.PageSize = tamanhoPagina;
            this.TotalCount = totalItens;
            this.List = new ReadOnlyCollection<T>(lista);
        }

        public virtual int CurrentPage { get; private set; }
        public virtual int PageSize { get; private set; }
        public virtual long TotalCount { get; private set; }
        public virtual IReadOnlyList<T> List { get; private set; }
    }
}
