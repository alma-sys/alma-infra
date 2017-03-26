namespace Alma.Core.Dto
{
    public class CodigoNome : ICodigo<int>, INome
    {
        public CodigoNome(int codigo, string nome)
        {
            this.Codigo = codigo;
            this.Nome = nome;
        }
        public int Codigo { get; private set; }
        public string Nome { get; private set; }
        public override string ToString()
        {
            return Nome;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CodigoNome);
        }

        public virtual bool Equals(CodigoNome obj)
        {
            return obj != null && string.Equals(obj.Codigo, Codigo);
        }
        public override int GetHashCode()
        {
            return Codigo != 0 ? Codigo.GetHashCode() : base.GetHashCode();
        }
    }
}
