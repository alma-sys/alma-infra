using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Alma.Core.Dto
{
    class PagedListDto<T> : IPagedList<T> where T : class
    {
        public const int TamanhoPaginaPadrao = 10;

        public PagedListDto(int pagina, long totalItens, IList<T> lista, int tamanhoPagina = TamanhoPaginaPadrao)
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
