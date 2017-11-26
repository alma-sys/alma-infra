using System.Collections.Generic;

namespace Alma.Core
{
    public interface IListaPaginada<T> : IListaPaginada where T : class
    {
        IReadOnlyList<T> Lista { get; }
    }

    public interface IListaPaginada
    {
        int Pagina { get; }
        int TamanhoPagina { get; }
        long TotalItens { get; }
    }
}