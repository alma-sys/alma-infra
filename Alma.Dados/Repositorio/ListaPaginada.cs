using System.Collections.Generic;
using Alma.Core.Dto;

namespace Alma.Dados
{
    class ListaPaginada<T> : IListaPaginada<T> where T : class
    {
        public const int TamanhoPaginaPadrao = 10;

        public ListaPaginada(int pagina, int totalItens, IList<T> lista, int tamanhoPagina = TamanhoPaginaPadrao)
        {
            this.Pagina = pagina;
            this.TamanhoPagina = tamanhoPagina;
            this.TotalItens = totalItens;
            this.Lista = lista;
        }

        public virtual int Pagina { get; private set; }
        public virtual int TamanhoPagina { get; private set; }
        public virtual int TotalItens { get; private set; }
        public virtual IList<T> Lista { get; private set; }
    }
}
