using Alma.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alma.ExampleProject.Domain.Repositories
{
    public interface IAuthorizationRepository
    {
        void UpdateAvailableAuthorizations(IList<string> authorizations);
        Task<IList<Access>> List();
    }
}
