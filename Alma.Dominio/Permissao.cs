using Alma.Core;

namespace Alma.Dominio
{
    public class Permissao : Entidade<int>, IIdNome
    {
        protected Permissao() { }

        public Permissao(string nome, string descricao, string chave, bool privado)
        {
            this.Nome = nome;
            this.Descricao = descricao;
            this.Chave = chave;
            this.Privado = privado;
        }

        public virtual string Nome { get; protected set; }
        public virtual string Descricao { get; protected set; }
        public virtual string Chave { get; protected set; }
        public virtual bool Privado { get; protected set; }


        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;
            var other = obj as Permissao;
            return other != null && (this.Chave.Equals(other.Chave) || other.Id.Equals(this.Id));
        }

        public override int GetHashCode()
        {
            return $"{Id}_{Chave}".GetHashCode();
        }
    }
}
