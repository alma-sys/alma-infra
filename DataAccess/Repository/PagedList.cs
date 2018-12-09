using System.Collections.Generic;
using System.Collections.ObjectModel;
using Alma.Common;

namespace Alma.DataAccess
{
    class PagedList<T> : IPagedList<T> where T : class
    {
        public const int DefaultPageSize = 10;

        public PagedList(int pagina, long totalItens, IList<T> lista, int tamanhoPagina = DefaultPageSize)
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
