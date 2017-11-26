namespace Alma.Core.Dto
{
    public class IdNome : IIdNome
    {
        public IdNome() { }

        public IdNome(long id, string nome) : this()
        {
            this.Id = id;
            this.Nome = nome;
        }

        public IdNome(IIdNome src) : this(src.Id, src.Nome) { }

        public long Id { get; set; }

        public string Nome { get; set; }

        public override string ToString()
        {
            return Nome;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IdNome);
        }

        public virtual bool Equals(IdNome obj)
        {
            return obj != null && string.Equals(obj.Id, Id);
        }
        public override int GetHashCode()
        {
            return Id != 0 ? Id.GetHashCode() : base.GetHashCode();
        }
    }
}
