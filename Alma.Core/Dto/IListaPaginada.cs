using System.Collections.Generic;

namespace Alma.Core.Dto
{
    public interface IListaPaginada<T> : IListaPaginada where T : class
    {
        IList<T> Lista { get; }
    }

    public interface IListaPaginada
    {
        int Pagina { get; }
        int TamanhoPagina { get; }
        int TotalItens { get; }
    }
}