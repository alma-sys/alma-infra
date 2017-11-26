namespace Alma.Core.Dto
{
    public class CodigoNome : ICodigo, INome
    {
        public CodigoNome(string codigo, string nome)
        {
            this.Codigo = codigo;
            this.Nome = nome;
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }
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
            return !string.IsNullOrWhiteSpace(Codigo) ? Codigo.GetHashCode() : base.GetHashCode();
        }
    }
}
