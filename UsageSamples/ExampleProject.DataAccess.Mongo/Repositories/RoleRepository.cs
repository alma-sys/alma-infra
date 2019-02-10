using Alma.DataAccess;
using Alma.Domain;
using Alma.ExampleProject.Domain.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace Alma.ExampleProject.DataAccess.Mongo.Repositories
{
    class RoleRepository : Alma.DataAccess.CrudRepository<Role>, IRoleRepository
    {
        private IRepository<Role> repository;
        public RoleRepository(IRepository<Role> repository) : base(repository)
        {
            this.repository = repository;
        }

        public async Task<Role> GetByName(string name)
        {
            var obj = await repository.Where(x => x.Name == name).SingleOrDefaultAsync();

            return obj;
        }
    }
}
