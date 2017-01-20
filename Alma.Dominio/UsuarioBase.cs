using System.Collections.Generic;
using Alma.Core;

namespace Alma.Dominio
{
    public abstract class UsuarioBase : Entidade<int>, IId
    {
        public UsuarioBase()
        {
            Perfis = new List<Perfil>();
        }

        public virtual bool Bloqueado { get; set; }
        public virtual bool Expirado { get; set; }
        public virtual bool Privado { get; protected set; }

        public virtual IList<Perfil> Perfis { get; set; }
    }
}
