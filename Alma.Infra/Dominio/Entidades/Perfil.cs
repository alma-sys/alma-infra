using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alma.Infra.Dominio.Entidades
{
    public class Perfil
    {
        public Perfil()
        {
            Permissoes = new List<Permissao>();
        }
        public virtual int Id { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Descricao { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual bool Privado { get; protected set; }
        public virtual IList<Permissao> Permissoes { get; set; }
    }
}
