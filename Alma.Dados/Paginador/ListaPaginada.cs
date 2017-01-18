using System.Collections.Generic;

namespace Alma.Dados.Paginador
{
    public class ListaPaginada<T> where T : class
    {
        public const int TamanhoPaginaPadrao = 10;

        public ListaPaginada(int pagina, int tamanhoPagina, int totalItens, IList<T> lista)
        {
            this.Pagina = pagina;
            this.TamanhoPagina = tamanhoPagina;
            this.TotalItens = totalItens;
            this.Lista = lista;
        }

        public readonly int Pagina;
        public readonly int TamanhoPagina;
        public readonly int TotalItens;
        public readonly IList<T> Lista;
    }
}
