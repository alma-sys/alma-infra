using Alma.Common;

namespace Alma.Domain.Repositories
{
    public interface IDomainRepository<T> where T : class, IId
    {
    }
}