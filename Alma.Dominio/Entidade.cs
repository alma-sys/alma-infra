using Alma.Core;
using System;

namespace Alma.Dominio
{
    /// <summary>
    /// Classe base de entidades com ID
    /// </summary>
    public abstract class Entidade : Entidade<long>
    {

    }

    /// <summary>
    /// Classe base de entidades com ID
    /// </summary>
    public abstract class Entidade<T> : EntidadeBase, IId<T> where T : struct
    {
        /// <summary>
        /// Id de referencia da entidade. 
        /// </summary>
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
            if (typeof(INome).IsAssignableFrom(this.GetType()))
                return ((INome)this).Nome;
            else if (typeof(IDescricao).IsAssignableFrom(this.GetType()))
                return ((IDescricao)this).Descricao;
            else
                return $"{base.ToString()}_{GetHashCode()}";
        }
    }

    //Classe base de entidades sem ID.
    public abstract class EntidadeBase
    {

    }
}
