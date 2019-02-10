using Alma.Common;
using Alma.DataAccess;
using Alma.ExampleProject.Domain.Entities;
using Alma.ExampleProject.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alma.ExampleProject.DataAccess.MSSql.Repositories
{
    class UserRepository : IUserRepository
    {
        IRepository<User> repository;

        public UserRepository(IRepository<User> repository)
        {
            this.repository = repository;
        }

        public async Task<User> GetByEmail(string email)
        {
            return await this.repository.Where(t => t.Email.Address == email).SingleOrDefaultAsync();
        }

        public async Task<User> GetByDomainUser(string user)
        {
            return await this.repository.Where(t => t.DomainUser == user).SingleOrDefaultAsync();
        }

        public async Task<User> GetByDomainUserOrEmail(string user)
        {
            var sanitized = user.ToLower();
            return await this.repository.Where(t => t.DomainUser.ToLower() == sanitized || t.Email.Address == sanitized).SingleOrDefaultAsync();
        }

        public async Task<User> Save(User user)
        {
            if (user.Id <= 0)
                await this.repository.CreateAsync(user);
            else
                await this.repository.SaveAsync(user);

            return user;
        }

        public async Task<IList<User>> List()
        {
            var query = this.repository.AsQueryable();
            return await query.ToListAsync();
        }

        public async Task<IPagedList<User>> Search(string queryTerms = null, int page = 1, int pageSize = 10)
        {
            var query = this.repository.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryTerms))
            {
                queryTerms = queryTerms.ToLower();
                query = query.Where(g =>
                                        g.Name.ToLower().Contains(queryTerms)
                                     || g.FamilyName.ToLower().Contains(queryTerms)
                                     || g.Telephone.ToLower().Contains(queryTerms)
                                     || g.Email.Address.ToLower().Contains(queryTerms)
                                     || g.DomainUser.ToLower().Contains(queryTerms));
            }

            query = query.OrderBy(x => x.Name);

            var lista = await query.ToPagedListAsync(page, pageSize);
            return lista;
        }

    }
}
