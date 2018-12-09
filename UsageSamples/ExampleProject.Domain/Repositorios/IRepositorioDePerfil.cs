using Alma.Domain;

namespace Alma.Exemplo.Dominio.Repositorios
{
    public interface IRepositorioDePerfil : Alma.Domain.Repositories.ICrudRepository<Role>
    {
        Role ObterPorNome(string nome);
    }
}
