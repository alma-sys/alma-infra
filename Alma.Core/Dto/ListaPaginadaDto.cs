using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Alma.Core.Dto
{
    class ListaPaginadaDto<T> : IListaPaginada<T> where T : class
    {
        public const int TamanhoPaginaPadrao = 10;

        public ListaPaginadaDto(int pagina, long totalItens, IList<T> lista, int tamanhoPagina = TamanhoPaginaPadrao)
        {
            this.Pagina = pagina;
            this.TamanhoPagina = tamanhoPagina;
            this.TotalItens = totalItens;
            this.Lista = new ReadOnlyCollection<T>(lista);
        }

        public virtual int Pagina { get; private set; }
        public virtual int TamanhoPagina { get; private set; }
        public virtual long TotalItens { get; private set; }
        public virtual IReadOnlyList<T> Lista { get; private set; }
    }
}
