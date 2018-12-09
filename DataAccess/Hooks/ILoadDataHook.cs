using Alma.Domain;

namespace Alma.DataAccess.Hooks
{
    /// <summary>
    /// Representa um evento de dados para leitura e hidratação de entidade
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ILoadDataHook<TEntity> : IDataHook<TEntity> where TEntity : Entity
    {
    }
}
