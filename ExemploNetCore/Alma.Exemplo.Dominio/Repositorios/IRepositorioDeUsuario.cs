using Alma.Core;
using Alma.Exemplo.Dominio.Entidades;
using System.Collections.Generic;

namespace Alma.Exemplo.Dominio.Repositorios
{
    public interface IRepositorioDeUsuario
    {
        Usuario GetPorEmail(string email);
        Usuario GetPorDomainUser(string user);
        Usuario GetPorEmailOuUser(string termo);


        Usuario Salvar(Usuario usuario);
        IList<Usuario> Listar();
        IListaPaginada<Usuario> Consultar(string termo = null, int pagina = 1, int tamanhoPagina = 10);

    }
}
