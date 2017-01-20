using System.Collections.Generic;
using Alma.Core;

namespace Alma.Dominio
{
    public class Perfil : Entidade<int>, IIdNome
    {
        public Perfil()
        {
            Permissoes = new List<Permissao>();
        }
        public virtual string Nome { get; set; }
        public virtual string Descricao { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual bool Privado { get; protected set; }
        public virtual IList<Permissao> Permissoes { get; set; }
    }
}
