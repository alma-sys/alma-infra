namespace Alma.Core.Dto
{
    public class CodigoDescricao : ICodigo, IDescricao
    {
        public CodigoDescricao(string codigo, string descricao)
        {
            this.Codigo = codigo;
            this.Descricao = descricao;
        }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public override string ToString()
        {
            return Descricao;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CodigoDescricao);
        }

        public virtual bool Equals(CodigoDescricao obj)
        {
            return obj != null && string.Equals(obj.Codigo, Codigo);
        }
        public override int GetHashCode()
        {
            return Codigo != null ? Codigo.GetHashCode() : base.GetHashCode();
        }
    }
}
