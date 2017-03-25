using Alma.Core;

namespace Alma.Dominio
{
    public abstract class Entidade : Entidade<int>
    {

    }
    public abstract class Entidade<T> : EntidadeBase, IId<T> where T : struct
    {
        public virtual T Id { get; protected set; }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;
            var other = obj as Entidade<T>;
            return other != null && !this.Id.Equals(default(T)) && other.Id.Equals(this.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            if (typeof(IIdNome<T>).IsAssignableFrom(this.GetType()))
                return ((IIdNome<T>)this).Nome;
            else
                return $"{base.ToString()}_{GetHashCode()}";
        }
    }

    public abstract class EntidadeBase
    {

    }
}
