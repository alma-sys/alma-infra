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


        protected IList<Role> _roles = new List<Role>();
        public virtual IReadOnlyList<Role> Roles => new ReadOnlyCollection<Role>(_roles);


        public virtual void AddRole(params Role[] role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            lock (_roles)
            {
                foreach (var p in role)
                {
                    if (!_roles.Contains(p))
                        _roles.Add(p);
                }
            }
        }

        public virtual void RemoveRole(params Role[] role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            lock (_roles)
            {
                foreach (var p in role)
                {
                    if (_roles.Contains(p))
                        _roles.Remove(p);
                }
            }

        }

        public virtual void Unblock()
        {
            this.Blocked = false;
        }

        public virtual void Block()
        {
            this.Blocked = true;
        }

        public virtual void SetAsExpired()
        {
            this.Expired = true;
        }

        public virtual void UnsetAsExpired()
        {
            this.Expired = false;
        }

    }
}
