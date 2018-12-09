using System;
using Alma.Domain;

namespace Alma.DataAccess.Hooks
{
    /// <summary>
    /// Representa um evento de dados para criação de entidade
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICreatedDataHook<T> : IDataHook<T> where T : Entity
    {
    }
    /// <summary>
    /// Representa um evento de dados para atualização de entidade
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUpdatedDataHook<T> : IDataHook<T> where T : Entity
    {
    }

    /// <summary>
    /// Representa um evento de dados para exclusão física de entidade
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeletedDataHook<T> : IDataHook<T> where T : Entity
    {
    }
}
