namespace Alma.Dominio
{
    public abstract class ValueObject
    {
        public override abstract string ToString();

        public abstract override int GetHashCode();
        public override abstract bool Equals(object obj);
    }

    public abstract class ValueObject<TOrigin> : ValueObject
    {

        protected abstract TOrigin ConvertTo();

        public static implicit operator TOrigin(ValueObject<TOrigin> v) { return v != null ? v.ConvertTo() : default(TOrigin); }

        //public static implicit operator ValueObject<TOrigin>(TOrigin v) { return new ValueObject<TOrigin>(v); }
        //protected abstract ValueObject<TOrigin> ConvertFrom(TOrigin origin);
    }
}
