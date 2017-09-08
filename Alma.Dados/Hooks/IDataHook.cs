using Alma.Dominio;

namespace Alma.Dados.Hooks
{
    public interface IDataHook
    {

    }
    public interface IDataHook<TEntity> : IDataHook where TEntity : Entidade
    {
    }
}
