using Alma.Dominio;

namespace Alma.Exemplo.Dominio.Repositorios
{
    public interface IRepositorioDePerfil : Alma.Dominio.Repositorios.IRepositorioCrud<Perfil>
    {
        Perfil ObterPorNome(string nome);
    }
}
