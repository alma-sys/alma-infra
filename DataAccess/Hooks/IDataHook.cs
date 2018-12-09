using System;
using Alma.Domain;

namespace Alma.DataAccess.Hooks
{
    /// <summary>
    /// Representa um evento de dados.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataHook<T> where T : Entity
    {
        void Handle(T entity);
    }
}
