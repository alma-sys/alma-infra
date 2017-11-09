using System;
using Alma.Dominio;

namespace Alma.Dados.Hooks
{
    /// <summary>
    /// Representa um evento de dados.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataHook<T> where T : Entidade
    {
        void Handle(T entity);
    }
}
