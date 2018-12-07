using Alma.Dominio;
using System;
using System.Text.RegularExpressions;

namespace Alma.Exemplo.Dominio.Entidades.ValueObjects
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

            this.Endereco = endereco;
        }

        public virtual string Endereco { get; protected set; }

        public virtual string User
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Endereco))
                {
                    var regex = new Regex(@"\A.*@");
                    var user = regex.Match(this.Endereco).ToString();
                    return user.Replace("@", string.Empty);
                }
                return string.Empty;
            }
        }
        public virtual string Domain
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Endereco))
                {
                    var regex = new Regex(@"@.*\z");
                    var domain = regex.Match(this.Endereco).ToString();
                    return domain.Replace("@", string.Empty);
                }
                return string.Empty;
            }
        }

        public override string ToString()
        {
            return this.Endereco;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Email && (obj as Email).Endereco == this.Endereco;
        }

        public override int GetHashCode()
        {
            return this.Endereco.GetHashCode();
        }

        protected override string ConvertTo()
        {
            return this.ToString();
        }

        public static implicit operator Email(string v) { return new Email(v); }
    }
}