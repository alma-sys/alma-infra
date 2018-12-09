using Alma.Common;
using Alma.DataAccess;
using Alma.Exemplo.Dominio.Entidades;
using Alma.Exemplo.Dominio.Repositorios;
using System.Collections.Generic;
using System.Linq;

namespace Alma.Exemplo.Dados.Sql.Repositorios
{
    class RepositorioDeUsuario : IRepositorioDeUsuario
    {
        IRepository<Usuario> repositorio;

        public RepositorioDeUsuario(IRepository<Usuario> repositorio)
        {
            this.repositorio = repositorio;
        }

        public Usuario GetPorEmail(string email)
        {
            return this.repositorio.SingleOrDefault(t => t.Email.Endereco == email);
        }

        public Usuario GetPorDomainUser(string user)
        {
            return this.repositorio.SingleOrDefault(t => t.DomainUser == user);
        }

        public Usuario GetPorEmailOuUser(string user)
        {
            var tratado = user.ToLower();
            return this.repositorio.SingleOrDefault(t => t.DomainUser.ToLower() == tratado || t.Email.Endereco == tratado);
        }

        public Usuario Salvar(Usuario usuario)
        {
            if (usuario.Id <= 0)
                this.repositorio.Create(usuario);
            else
                this.repositorio.Save(usuario);

            return usuario;
        }

        public IList<Usuario> Listar()
        {
            var query = this.repositorio.AsQueryable();
            return query.ToList();
        }

        public IPagedList<Usuario> Consultar(string termo = null, int pagina = 1, int tamanhoPagina = 10)
        {
            var query = this.repositorio.AsQueryable();

            if (!string.IsNullOrWhiteSpace(termo))
            {
                termo = termo.ToUpper();
                query = query.Where(g =>
                                        g.Name.ToUpper().Contains(termo)
                                     || g.Sobrenome.ToUpper().Contains(termo)
                                     || g.Telefone.ToUpper().Contains(termo)
                                     || g.Email.Endereco.ToUpper().Contains(termo)
                                     || g.DomainUser.ToUpper().Contains(termo));
            }

            query = query.OrderBy(x => x.Name);

            var lista = query.ToPagedList(pagina, tamanhoPagina);
            return lista;
        }



    }
}
