using System;
using Alma.Dominio;

namespace Alma.Dados.Hooks
{
    /// <summary>
    /// Cria eventos de domínio para salvar entidades
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISavedDataHook<T> : IDataHook<T> where T : Entidade
    {
    }

    /// <summary>
    /// Cria eventos de domínio para excluír entidades
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeletedDataHook<T> : IDataHook<T> where T : Entidade
    {
    }
}
