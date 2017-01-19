using Alma.Core;

namespace Alma.Dominio.Entidades
{
    public class Permissao : Entidade<int>, IIdNome
    {
        public virtual string Nome { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Chave { get; set; }
        public virtual bool Privado { get; protected set; }
    }
}
