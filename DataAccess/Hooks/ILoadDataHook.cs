﻿using Alma.Domain;

namespace Alma.DataAccess.Hooks
{
    /// <summary>
    /// Represents a data event after reading and hidrating an entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ILoadDataHook<TEntity> : IDataHook<TEntity> where TEntity : Entity
    {
    }
}
