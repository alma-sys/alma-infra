using Alma.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Alma.Domain
{
    public abstract class UserBase : Entity, IId
    {
        public UserBase() { }

        public virtual bool Blocked { get; protected set; }
        public virtual bool Expired { get; protected set; }
        public virtual bool Private { get; protected set; }


        protected IList<Role> _perfis = new List<Role>();
        public virtual IReadOnlyList<Role> Perfis => new ReadOnlyCollection<Role>(_perfis);


        public virtual void AssociarPerfil(params Role[] perfil)
        {
            if (perfil == null)
                throw new ArgumentNullException(nameof(perfil));

            lock (_perfis)
            {
                foreach (var p in perfil)
                {
                    if (!_perfis.Contains(p))
                        _perfis.Add(p);
                }
            }
        }

        public virtual void RemoverPerfil(params Role[] perfil)
        {
            if (perfil == null)
                throw new ArgumentNullException(nameof(perfil));

            lock (_perfis)
            {
                foreach (var p in perfil)
                {
                    if (_perfis.Contains(p))
                        _perfis.Remove(p);
                }
            }

        }

        public virtual void Desbloquear()
        {
            this.Blocked = false;
        }

        public virtual void Bloquear()
        {
            this.Blocked = true;
        }

        public virtual void DefinirExpirado()
        {
            this.Expired = true;
        }

        public virtual void RemoverExpirado()
        {
            this.Expired = false;
        }

    }
}
