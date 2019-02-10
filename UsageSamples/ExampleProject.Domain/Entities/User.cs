using Alma.Common;
using Alma.Domain;
using Alma.ExampleProject.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Alma.ExampleProject.Domain.Entities
{
    public class User : Entity, IIdName
    {
        internal User() { }

        public User(long personUId, string name, string familyname, string address, string email, string telefone, string domainUser)
        {
            if (default(int) == personUId)
                throw new ArgumentException(nameof(personUId));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(nameof(email));

            //if (string.IsNullOrWhiteSpace(domainUser))
            //    throw new ArgumentException(nameof(domainUser));

            this.PersonUId = personUId;
            this.Email = email.ToLower().Trim();


            Update(name, familyname, address, telefone, domainUser);
        }

        public virtual void UpdateLastAccess(DateTime dataUltimoAcesso)
        {
            if (this.LastAccess.HasValue && this.LastAccess.Value > dataUltimoAcesso)
                throw new ArgumentException(nameof(dataUltimoAcesso));

            this.LastAccess = dataUltimoAcesso;
        }

        public virtual void Update(string nome, string sobrenome, string endereco, string telefone, string domainUser)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException(nameof(nome));

            //if (string.IsNullOrWhiteSpace(sobrenome))
            //    throw new ArgumentException(nameof(sobrenome));

            this.Name = nome?.Trim();
            this.DomainUser = domainUser?.Trim().ToLower();
            this.FamilyName = !string.IsNullOrWhiteSpace(sobrenome) ? sobrenome.Trim() : null;
            this.Address = !string.IsNullOrWhiteSpace(endereco) ? endereco.Trim() : null; ;
            this.Telephone = !string.IsNullOrWhiteSpace(telefone) ? telefone.Trim() : null; ;
            this.DomainUser = string.IsNullOrWhiteSpace(domainUser) ? null : domainUser.Trim();
        }
        //TODO: Não sei se realmente precisamos deste campo.
        public virtual long PersonUId { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual string FamilyName { get; protected set; }
        public virtual string Address { get; protected set; }
        public virtual Email Email { get; protected set; }
        public virtual string Telephone { get; protected set; }
        public virtual string DomainUser { get; protected set; }
        public virtual DateTime? LastAccess { get; protected set; }

        private IList<Role> _roles = new List<Role>();
        public virtual IList<Role> Roles => new ReadOnlyCollection<Role>(_roles);


        public virtual User Creator { get; protected set; }

        public virtual void SetCreator(User user)
        {
            this.Creator = user;
        }
    }
}
