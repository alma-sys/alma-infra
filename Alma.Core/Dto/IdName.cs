namespace Alma.Core.Dto
{
    public class IdName : IIdName
    {
        public IdName() { }

        public IdName(long id, string name) : this()
        {
            this.Id = id;
            this.Name = name;
        }

        public IdName(IIdName src) : this(src.Id, src.Name) { }

        public long Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IdName);
        }

        public virtual bool Equals(IdName obj)
        {
            return obj != null && string.Equals(obj.Id, Id);
        }
        public override int GetHashCode()
        {
            return Id != 0 ? Id.GetHashCode() : base.GetHashCode();
        }
    }
}
