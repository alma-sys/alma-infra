using System.Collections.Generic;

namespace Alma.Core.Dto
{
    public interface IListaPaginada<T> where T : class
    {
        IList<T> Lista { get; }
        int Pagina { get; }
        int TamanhoPagina { get; }
        int TotalItens { get; }
    }
}