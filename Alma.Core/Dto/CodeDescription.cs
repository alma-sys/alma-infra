namespace Alma.Core.Dto
{
    public class CodeDescription : ICode, IDescription
    {
        public CodeDescription(string codigo, string descricao)
        {
            this.Code = codigo;
            this.Description = descricao;
        }
        public string Code { get; set; }
        public string Description { get; set; }
        public override string ToString()
        {
            return Description;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CodeDescription);
        }

        public virtual bool Equals(CodeDescription obj)
        {
            return obj != null && string.Equals(obj.Code, Code);
        }
        public override int GetHashCode()
        {
            return Code != null ? Code.GetHashCode() : base.GetHashCode();
        }
    }
}
