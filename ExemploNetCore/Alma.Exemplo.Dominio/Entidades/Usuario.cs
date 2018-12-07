using Alma.Core;
using Alma.Dominio;
using Alma.Exemplo.Dominio.Entidades.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Alma.Exemplo.Dominio.Entidades
{
    public class Usuario : Entidade, IIdName
    {
        internal Usuario() { }

        public Usuario(long personUId, string nome, string sobrenome, string endereco, string email, string telefone, string domainUser)
        {
            if (default(int) == personUId)
                throw new ArgumentException(nameof(personUId));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(nameof(email));

            //if (string.IsNullOrWhiteSpace(domainUser))
            //    throw new ArgumentException(nameof(domainUser));

            this.PersonUId = personUId;
            this.Email = email.ToLower().Trim();


            Atualizar(nome, sobrenome, endereco, telefone, domainUser);
        }

        public virtual void AtualizarUltimoAcesso(DateTime dataUltimoAcesso)
        {
            if (this.UltimoAcesso.HasValue && this.UltimoAcesso.Value > dataUltimoAcesso)
                throw new ArgumentException(nameof(dataUltimoAcesso));

            this.UltimoAcesso = dataUltimoAcesso;
        }

        public virtual void Atualizar(string nome, string sobrenome, string endereco, string telefone, string domainUser)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException(nameof(nome));

            //if (string.IsNullOrWhiteSpace(sobrenome))
            //    throw new ArgumentException(nameof(sobrenome));

            this.Name = nome?.Trim();
            this.DomainUser = domainUser?.Trim().ToLower();
            this.Sobrenome = !string.IsNullOrWhiteSpace(sobrenome) ? sobrenome.Trim() : null;
            this.Endereco = !string.IsNullOrWhiteSpace(endereco) ? endereco.Trim() : null; ;
            this.Telefone = !string.IsNullOrWhiteSpace(telefone) ? telefone.Trim() : null; ;
            this.DomainUser = string.IsNullOrWhiteSpace(domainUser) ? null : domainUser.Trim();
        }
        //TODO: Não sei se realmente precisamos deste campo.
        public virtual long PersonUId { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual string Sobrenome { get; protected set; }
        public virtual string Endereco { get; protected set; }
        public virtual Email Email { get; protected set; }
        public virtual string Telefone { get; protected set; }
        public virtual string DomainUser { get; protected set; }
        public virtual DateTime? UltimoAcesso { get; protected set; }

        private IList<Perfil> _perfis = new List<Perfil>();
        public virtual IList<Perfil> Perfis => new ReadOnlyCollection<Perfil>(_perfis);


        public virtual Usuario UsuarioPai { get; protected set; }

        public virtual void DefinirUsuarioPai(Usuario usuario)
        {
            this.UsuarioPai = usuario;
        }
    }
}
