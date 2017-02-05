using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Alma.Core
{
    public abstract class ValidacaoException : ApplicationException
    {
        public ValidacaoException(string message = null, IDictionary<string, string> errors = null)
            : base(GetDefaultMessage(message, errors))
        {
            this.Errors = new ReadOnlyDictionary<string, string>(errors ?? new Dictionary<string, string>());
        }

        private static string GetDefaultMessage(string message, IDictionary<string, string> errors)
        {
            if (!string.IsNullOrWhiteSpace(message))
                return message;
            else if (errors != null && errors.Count > 0)
                return errors.Values.First();
            else
                return null;
        }

        public IDictionary<string, string> Errors { get; private set; }
    }
}
