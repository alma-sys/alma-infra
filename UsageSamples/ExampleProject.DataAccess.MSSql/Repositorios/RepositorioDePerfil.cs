using Alma.DataAccess;
using Alma.Domain;
using Alma.Exemplo.Dominio.Repositorios;
using System.Linq;

namespace Alma.Exemplo.Dados.Sql.Repositorios
{
    class RepositorioDePerfil : Alma.DataAccess.CrudRepository<Role>, IRepositorioDePerfil
    {
        private IRepository<Role> repositorio;
        public RepositorioDePerfil(IRepository<Role> repositorio) : base(repositorio)
        {
            this.repositorio = repositorio;
        }

        public Role ObterPorNome(string nome)
        {
            var obj = repositorio.Where(x => x.Name == nome).SingleOrDefault();

            return obj;
        }
    }
}
