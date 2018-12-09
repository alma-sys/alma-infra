using Alma.DataAccess;
using Alma.Domain;
using Alma.Exemplo.Dominio.Repositorios;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Alma.Exemplo.Dados.Sql.Repositorios
{
    class RepositorioDePermissao : IRepositorioDePermissao
    {
        IRepository<Access> repositorio;

        public RepositorioDePermissao(IRepository<Access> repositorio)
        {
            this.repositorio = repositorio;
        }

        public void AtualizarPermissoes(IList<string> permissoes)
        {
            using (var t = new TransactionScope())
            {
                var lista_banco = repositorio.Where(x => permissoes.Contains(x.Key)).ToList();
                var lista_banco_chaves = lista_banco.Select(x => x.Key).ToList();

                var lista_nova = permissoes.Except(lista_banco_chaves)
                    .Select(p => new Access(p, null, p, true)).ToList();
                if (lista_nova.Any())
                    repositorio.Create(lista_nova);

                t.Complete();
            }
        }


        public IList<Access> Listar()
        {
            return repositorio.ToList();
        }
    }
}
