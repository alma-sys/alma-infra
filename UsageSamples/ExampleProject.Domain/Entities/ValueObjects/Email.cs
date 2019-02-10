using Alma.Domain;
using System;
using System.Text.RegularExpressions;

namespace Alma.ExampleProject.Domain.Entities.ValueObjects
{
    public class Email : ValueObject<string>
    {
        protected Email() : base() { }

        public Email(string endereco)
        {
            if (string.IsNullOrWhiteSpace(endereco))
                throw new ArgumentNullException(nameof(endereco));

            endereco = endereco.Trim();

            if (endereco.Length > 254)
                throw new ArgumentOutOfRangeException(nameof(endereco));

            this.Address = endereco;
        }

        public virtual string Address { get; protected set; }

        public virtual string User
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Address))
                {
                    var regex = new Regex(@"\A.*@");
                    var user = regex.Match(this.Address).ToString();
                    return user.Replace("@", string.Empty);
                }
                return string.Empty;
            }
        }
        public virtual string Domain
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Address))
                {
                    var regex = new Regex(@"@.*\z");
                    var domain = regex.Match(this.Address).ToString();
                    return domain.Replace("@", string.Empty);
                }
                return string.Empty;
            }
        }

        public override string ToString()
        {
            return this.Address;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Email && (obj as Email).Address == this.Address;
        }

        public override int GetHashCode()
        {
            return this.Address.GetHashCode();
        }

        protected override string ConvertTo()
        {
            return this.ToString();
        }

        public static implicit operator Email(string v) { return new Email(v); }
    }
}