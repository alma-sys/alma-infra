using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Alma.Core
{
    /// <summary>
    /// Classe de exceção de validação de domínio. 
    /// A validação deve ser lançada pelo contexto de domínio.
    /// </summary>
    public abstract class ValidacaoException : ApplicationException
    {
        public ValidacaoException(string message)
            : this(message, null)
        { }
        public ValidacaoException(IDictionary<string, string> errors)
            : this(null, errors)
        { }

        public ValidacaoException(string message, IDictionary<string, string> errors)
            : base(GetDefaultMessage(message, errors))
        {
            this.Errors = new ReadOnlyDictionary<string, string>(errors ?? new Dictionary<string, string>());
        }

        private static string GetDefaultMessage(string message, IDictionary<string, string> errors)
        {
            if (!string.IsNullOrWhiteSpace(message))
                return message;
            //else if (errors != null && errors.Count == 1)
            //    return errors.Values.First();
            else
                return null;
        }

        public IDictionary<string, string> Errors { get; private set; }
    }
}
