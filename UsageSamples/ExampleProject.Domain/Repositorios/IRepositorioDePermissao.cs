using Alma.Domain;
using System.Collections.Generic;

namespace Alma.Exemplo.Dominio.Repositorios
{
    public interface IRepositorioDePermissao
    {
        void AtualizarPermissoes(IList<string> permissoes);
        IList<Access> Listar();
    }
}
