using System;
using Alma.Dominio;

namespace Alma.Dados.Hooks
{
    [Obsolete("Use o IDataHook<T>")]
    public interface IDataHook
    { }
#pragma warning disable 0618

    public interface IDataHook<T> : IDataHook where T : Entidade
    {
        void OnHandle(T entity);
    }
#pragma warning restore 0618
}
