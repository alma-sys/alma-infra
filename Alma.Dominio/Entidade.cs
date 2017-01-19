using Alma.Core;

namespace Alma.Dominio.Entidades
{
    public abstract class Entidade : Entidade<int>
    {

    }
    public abstract class Entidade<T> : IId<T> where T : struct
    {
        public virtual T Id { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Entidade<T>;
            return other != null && other.Id.Equals(this.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
