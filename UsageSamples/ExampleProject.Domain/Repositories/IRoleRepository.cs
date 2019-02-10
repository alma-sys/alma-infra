using Alma.Domain;
using System.Threading.Tasks;

namespace Alma.ExampleProject.Domain.Repositories
{
    public interface IRoleRepository : Alma.Domain.Repositories.ICrudRepository<Role>
    {
        Task<Role> GetByName(string nome);
    }
}
