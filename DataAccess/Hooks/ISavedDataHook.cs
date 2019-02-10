using Alma.Domain;

namespace Alma.DataAccess.Hooks
{
    /// <summary>
    /// Represents a data event for creating an entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICreatedDataHook<T> : IDataHook<T> where T : Entity
    {
    }
    /// <summary>
    /// Represents a data event for updating an entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUpdatedDataHook<T> : IDataHook<T> where T : Entity
    {
    }

    /// <summary>
    /// Represents a data event for deleting an entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeletedDataHook<T> : IDataHook<T> where T : Entity
    {
    }
}
