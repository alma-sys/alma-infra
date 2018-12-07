using Alma.Core;
using Alma.Dominio.Repositorios;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Alma.Dados
{
    public abstract class RepositorioCrud<T> : IRepositorioCrud<T> where T : class, IId
    {
        private IRepositorio<T> repositorio;

        public RepositorioCrud(IRepositorio<T> repositorio)
        {
            this.repositorio = repositorio;
        }

        public IList<T> Listar()
        {
            var query = this.repositorio.AsQueryable();

            //if (typeof(INome).IsAssignableFrom(typeof(T)))
            //    query = query.Cast<INome>().OrderBy(x => x.Nome).Cast<T>().AsQueryable();

            var lista = query.ToList();
            return lista;
        }

        public T Obter(long id)
        {
            var res = repositorio.SingleOrDefault(t => t.Id == id);
            return res;
        }

        public IList<T> Obter(IList<long> id)
        {
            if (id == null)
                throw new System.ArgumentNullException(nameof(id));

            var aplicacao = repositorio.Where(x => id.Contains(x.Id)).ToList();
            return aplicacao;
        }

        public void Remover(T item)
        {
            repositorio.Delete(item);
        }

        public void Salvar(IList<T> itens)
        {
            if (itens == null || itens.Count == 0)
                return;
            using (var t = new TransactionScope())
            {
                foreach (var i in itens)
                    Salvar(i);
                t.Complete();
            }
        }

        public void Salvar(T item)
        {
            if (item.Id <= 0)
                repositorio.Create(item);
            else
                repositorio.Save(item);
        }
    }
}
