using Alma.Infra.Seguranca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alma.Infra.Dominio.Entidades
{
    public abstract class UsuarioBase 
    {
        public UsuarioBase()
        {
            Perfis = new List<Perfil>();
        }

        public virtual int Id
        {
            get; set;
        }
        
        public virtual bool Bloqueado { get; set; }
        public virtual bool Expirado { get; set; }
        public virtual bool Privado { get; protected set; }
       
        public virtual IList<Perfil> Perfis { get; set; }
    }
}
