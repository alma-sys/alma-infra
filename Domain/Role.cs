using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Alma.Common;

namespace Alma.Domain
{
    public class Role : Entity, IIdName, IDescription
    {
        protected Role() { }
        public Role(string name, string description, bool enabled = true)
        {
            this.Enabled = enabled;

            this.ChangeName(name);
            this.ChangeDescription(description);
        }

        public virtual string Name { get; protected set; }
        public virtual string Description { get; protected set; }
        public virtual bool Enabled { get; protected set; }
        public virtual bool Private { get; protected set; }

        protected IList<Access> _accessList = new List<Access>();
        public virtual IReadOnlyList<Access> AccessList => new ReadOnlyCollection<Access>(_accessList);

        public virtual void AddAccess(params Access[] access)
        {
            if (access == null)
                throw new ArgumentNullException(nameof(access));

            lock (_accessList)
            {
                foreach (var p in access)
                {
                    if (!_accessList.Contains(p))
                        _accessList.Add(p);
                }
            }
        }

        public virtual void RemoveAccess(params Access[] access)
        {
            if (access == null)
                throw new ArgumentNullException(nameof(access));

            lock (_accessList)
            {
                foreach (var p in access)
                {
                    if (_accessList.Contains(p))
                        _accessList.Remove(p);
                }
            }

        }

        public virtual void Enable()
        {
            this.Enabled = true;
        }

        public virtual void Disable()
        {
            this.Enabled = false;
        }

        public virtual void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            int maxSize = 50;
            if (name.Length > maxSize)
                throw new ArgumentException($"{nameof(Name)} must have at maximum {maxSize} characters.");

            this.Name = name;
        }

        public virtual void ChangeDescription(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException(nameof(descricao));

            int maxSize = 500;
            if (descricao.Length > maxSize)
                throw new ArgumentException($"{nameof(Description)} must have at maximum {maxSize} characters.");

            this.Description = descricao;
        }
    }
}
