using Alma.Common;
using Alma.ExampleProject.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alma.ExampleProject.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByDomainUser(string user);
        Task<User> GetByDomainUserOrEmail(string termo);


        Task<User> Save(User user);
        Task<IList<User>> List();
        Task<IPagedList<User>> Search(string query = null, int page = 1, int pageSize = 10);

    }
}
