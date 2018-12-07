using Alma.Common;
using System.Collections.Generic;

namespace Alma.Dominio.Repositorios
{
    public interface IRepositorioCrud<T> where T : class, IId
    {
        IList<T> Listar();

        T Obter(long id);

        IList<T> Obter(IList<long> id);

        void Remover(T item);

        void Salvar(IList<T> itens);

        void Salvar(T item);
    }
}