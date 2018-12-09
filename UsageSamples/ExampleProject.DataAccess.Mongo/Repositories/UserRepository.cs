using Alma.Common;
using Alma.DataAccess;
using Alma.ExampleProject.Domain.Entities;
using Alma.ExampleProject.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alma.ExampleProject.DataAccess.Mongo.Repositories
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
            var tratado = user.ToLower();
            return await this.repository.Where(t => t.DomainUser.ToLower() == tratado || t.Email == tratado).SingleOrDefaultAsync();
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

        public async Task<IPagedList<User>> Search(string termo = null, int pagina = 1, int tamanhoPagina = 10)
        {
            var query = this.repository.AsQueryable();

            if (!string.IsNullOrWhiteSpace(termo))
            {
                termo = termo.ToUpper();
                query = query.Where(g =>
                                        g.Name.ToUpper().Contains(termo)
                                     || g.FamilyName.ToUpper().Contains(termo)
                                     || g.Telephone.ToUpper().Contains(termo)
                                     || g.Email.Address.ToUpper().Contains(termo)
                                     || g.DomainUser.ToUpper().Contains(termo));
            }

            query = query.OrderBy(x => x.Name);

            var lista = await query.ToPagedListAsync(pagina, tamanhoPagina);
            return lista;
        }



    }
}
