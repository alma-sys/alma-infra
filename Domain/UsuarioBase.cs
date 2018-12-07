using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Alma.Common;

namespace Alma.Dominio
{
    public abstract class UsuarioBase : Entidade, IId
    {
        public UsuarioBase() { }

        public virtual bool Bloqueado { get; protected set; }
        public virtual bool Expirado { get; protected set; }
        public virtual bool Privado { get; protected set; }


        protected IList<Perfil> _perfis = new List<Perfil>();
        public virtual IReadOnlyList<Perfil> Perfis => new ReadOnlyCollection<Perfil>(_perfis);


        public virtual void AssociarPerfil(params Perfil[] perfil)
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

        public virtual void RemoverPerfil(params Perfil[] perfil)
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
            this.Bloqueado = false;
        }

        public virtual void Bloquear()
        {
            this.Bloqueado = true;
        }

        public virtual void DefinirExpirado()
        {
            this.Expirado = true;
        }

        public virtual void RemoverExpirado()
        {
            this.Expirado = false;
        }

    }
}
