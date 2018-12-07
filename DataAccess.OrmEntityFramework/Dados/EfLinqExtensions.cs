using System;
using System.Linq;
using System.Linq.Expressions;

namespace Alma.Dados.OrmEntityFramework
{
    public sealed class EfLinqExtensions : ILinqExtensions
    {
        public IQueryable<TQueried> Fetch<TQueried, TRelated>(IQueryable<TQueried> query, Expression<Func<TQueried, TRelated>> relatedObjectSelector)
        {
            //return query.Include(relatedObjectSelector);
            throw new NotSupportedException();
        }

    }
}
