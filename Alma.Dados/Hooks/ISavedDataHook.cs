using System;
using Alma.Dominio;

namespace Alma.Dados.Hooks
{
    /// <summary>
    /// Representa um evento de dados para criação de entidade
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICreatedDataHook<T> : IDataHook<T> where T : Entidade
    {
    }
    /// <summary>
    /// Representa um evento de dados para atualização de entidade
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUpdatedDataHook<T> : IDataHook<T> where T : Entidade
    {
    }

    /// <summary>
    /// Representa um evento de dados para exclusão física de entidade
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeletedDataHook<T> : IDataHook<T> where T : Entidade
    {
    }
}
