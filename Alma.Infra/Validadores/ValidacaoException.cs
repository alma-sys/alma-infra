using System;
using System.Collections.Generic;

namespace Alma.Infra.Validadores
{
    public class ValidacaoException : ApplicationException
    {
        public ValidacaoException(string message = null, IDictionary<string, string> errors = null)
            : base(message)
        {
            errors = errors ?? new Dictionary<string, string>();
            this.Errors = errors;
        }

        public IDictionary<string, string> Errors { get; private set; }
    }
}
