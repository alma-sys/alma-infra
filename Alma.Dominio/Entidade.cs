using Alma.Core;
using System;

namespace Alma.Dominio
{
    public abstract class Entidade : Entidade<int>
    {

    }
    public abstract class Entidade<T> : EntidadeBase, IId<T> where T : struct
    {
        [Obsolete("Set de ID tem que ser protected")]
        public virtual T Id { get; /*protected*/ set; }

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
            if (typeof(INome).IsAssignableFrom(this.GetType()))
                return ((INome)this).Nome;
            else if (typeof(IDescricao).IsAssignableFrom(this.GetType()))
                return ((IDescricao)this).Descricao;
            else
                return $"{base.ToString()}_{GetHashCode()}";
        }
    }

    public abstract class EntidadeBase
    {

    }
}
