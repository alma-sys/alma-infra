using Alma.Dados;
using Alma.Dominio;
using Alma.Exemplo.Dominio.Repositorios;
using System.Linq;

namespace Alma.Exemplo.Dados.Mongo.Repositorios
{
    class RepositorioDePerfil : Alma.Dados.RepositorioCrud<Perfil>, IRepositorioDePerfil
    {
        private IRepositorio<Perfil> repositorio;
        public RepositorioDePerfil(IRepositorio<Perfil> repositorio) : base(repositorio)
        {
            this.repositorio = repositorio;
        }

        public Perfil ObterPorNome(string nome)
        {
            var obj = repositorio.Where(x => x.Name == nome).SingleOrDefault();

            return obj;
        }
    }
}
