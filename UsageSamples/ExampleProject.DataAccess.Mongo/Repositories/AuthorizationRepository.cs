using Alma.DataAccess;
using Alma.Domain;
using Alma.ExampleProject.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Alma.ExampleProject.DataAccess.Mongo.Repositories
{
    class AuthorizationRepository : IAuthorizationRepository
    {
        IRepository<Access> repository;

        public AuthorizationRepository(IRepository<Access> repository)
        {
            this.repository = repository;
        }

        public void UpdateAvailableAuthorizations(IList<string> authorizations)
        {
            using (var t = new TransactionScope())
            {
                var lista_banco = repository.Where(x => authorizations.Contains(x.Key)).ToList();
                var lista_banco_chaves = lista_banco.Select(x => x.Key).ToList();

                var lista_nova = authorizations.Except(lista_banco_chaves)
                    .Select(p => new Access(p, null, p, true)).ToList();
                if (lista_nova.Any())
                    repository.Create(lista_nova);

                t.Complete();
            }
        }


        public async Task<IList<Access>> List()
        {
            return await repository.ToListAsync();
        }
    }
}
