namespace Alma.Common.Dto
{
    public class CodeName : ICode, IName
    {
        public CodeName(string code, string name)
        {
            this.Code = code;
            this.Name = name;
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CodeName);
        }

        public virtual bool Equals(CodeName obj)
        {
            return obj != null && string.Equals(obj.Code, Code);
        }
        public override int GetHashCode()
        {
            return !string.IsNullOrWhiteSpace(Code) ? Code.GetHashCode() : base.GetHashCode();
        }
    }
}
