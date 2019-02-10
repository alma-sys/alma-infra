using Alma.Common;
using Alma.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Alma.DataAccess
{
    public abstract class CrudRepository<T> : ICrudRepository<T> where T : class, IId
    {
        private IRepository<T> storageRepository;

        public CrudRepository(IRepository<T> storageRepository)
        {
            this.storageRepository = storageRepository;
        }

        public async Task<IList<T>> ListAsync()
        {
            var query = this.storageRepository.AsQueryable();

            //if (typeof(INome).IsAssignableFrom(typeof(T)))
            //    query = query.Cast<INome>().OrderBy(x => x.Nome).Cast<T>().AsQueryable();

            var list = await query.ToListAsync();
            return list;
        }

        public async Task<T> GetAsync(long id)
        {
            var query = storageRepository.Where(t => t.Id == id);

            var instance = await query.SingleOrDefaultAsync();
            return instance;
        }

        public async Task<IList<T>> GetAsync(IList<long> id)
        {
            if (id == null)
                throw new System.ArgumentNullException(nameof(id));

            var query = storageRepository.Where(x => id.Contains(x.Id));
            var list = await query.ToListAsync();
            return list;
        }

        public async Task DeleteAsync(T item)
        {
            await storageRepository.DeleteAsync(item);
        }

        public async Task SaveAsync(IList<T> items)
        {
            if (items == null || items.Count == 0)
                return;
            using (var t = new TransactionScope())
            {
                var tasks = items.Select(i => SaveAsync(i));
                await Task.WhenAll(tasks);
                t.Complete();
            }
        }

        public async Task SaveAsync(T item)
        {
            if (item.Id <= 0)
                await storageRepository.CreateAsync(item);
            else
                await storageRepository.SaveAsync(item);
        }
    }
}
