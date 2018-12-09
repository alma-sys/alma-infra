using Alma.Common;
using System;

namespace Alma.Domain
{
    public class Access : Entity, IIdName, IDescription
    {
        protected Access() { }

        public Access(string name, string description, string key, bool @private)
        {
            this.Name = name;
            this.Description = description;
            this.Key = key;
            this.Private = @private;
        }

        public virtual string Name { get; protected set; }
        public virtual string Description { get; protected set; }
        public virtual string Key { get; protected set; }
        public virtual bool Private { get; protected set; }

        public virtual void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            int maxSize = 50;
            if (name.Length > maxSize)
                throw new ArgumentException($"{nameof(Name)} must have at maximum {maxSize} characters.");

            this.Name = name;
        }

        public virtual void ChangeDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException(nameof(description));

            int maxSize = 500;
            if (description.Length > maxSize)
                throw new ArgumentException($"{nameof(Description)} must have at maximum {maxSize} characters.");

            this.Description = description;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;
            var other = obj as Access;
            return other != null && (this.Key.Equals(other.Key) || other.Id.Equals(this.Id));
        }

        public override int GetHashCode()
        {
            return $"{Id}_{Key}".GetHashCode();
        }
    }
}
