using Alma.Common;

namespace Alma.Domain
{
    /// <summary>
    /// Base class for entities with numeric Id
    /// </summary>
    public abstract class Entity : Entity<long>
    {

    }

    /// <summary>
    /// Base class for entities with Id of any type.
    /// </summary>
    public abstract class Entity<T> : EntityBase, IId<T> where T : struct
    {
        /// <summary>
        /// Id de referencia da entidade. 
        /// </summary>
        public virtual T Id { get; protected set; }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;
            var other = obj as Entity<T>;
            return other != null && !this.Id.Equals(default(T)) && other.Id.Equals(this.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            if (typeof(IName).IsAssignableFrom(this.GetType()))
                return ((IName)this).Name;
            else if (typeof(IDescription).IsAssignableFrom(this.GetType()))
                return ((IDescription)this).Description;
            else
                return $"{base.ToString()}_{GetHashCode()}";
        }
    }

    /// <summary>
    /// Base class for entities with no Id.
    /// </summary>
    public abstract class EntityBase
    {

    }
}
