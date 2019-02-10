using Alma.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alma.Domain.Repositories
{
    public interface ICrudRepository<T> : IDomainRepository<T> where T : class, IId
    {
        Task<IList<T>> ListAsync();

        Task<T> GetAsync(long id);

        Task<IList<T>> GetAsync(IList<long> id);

        Task DeleteAsync(T item);

        Task SaveAsync(IList<T> itens);

        Task SaveAsync(T item);
    }
}