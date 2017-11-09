using Alma.Dominio;

namespace Alma.Dados.Hooks
{
    /// <summary>
    /// Representa um evento de dados para leitura e hidratação de entidade
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ILoadDataHook<TEntity> : IDataHook<TEntity> where TEntity : Entidade
    {
    }
}
