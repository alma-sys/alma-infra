using Alma.Dados;
using Alma.Dominio;
using Alma.Exemplo.Dominio.Repositorios;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Alma.Exemplo.Dados.Sql.Repositorios
{
    class RepositorioDePermissao : IRepositorioDePermissao
    {
        IRepositorio<Permissao> repositorio;

        public RepositorioDePermissao(IRepositorio<Permissao> repositorio)
        {
            this.repositorio = repositorio;
        }

        public void AtualizarPermissoes(IList<string> permissoes)
        {
            using (var t = new TransactionScope())
            {
                var lista_banco = repositorio.Where(x => permissoes.Contains(x.Chave)).ToList();
                var lista_banco_chaves = lista_banco.Select(x => x.Chave).ToList();

                var lista_nova = permissoes.Except(lista_banco_chaves)
                    .Select(p => new Permissao(p, null, p, true)).ToList();
                if (lista_nova.Any())
                    repositorio.Create(lista_nova);

                t.Complete();
            }
        }


        public IList<Permissao> Listar()
        {
            return repositorio.ToList();
        }
    }
}
