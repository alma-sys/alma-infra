using System.Collections.Generic;

namespace Alma.Dominio.Validadores
{
    internal class ValidacaoException : Common.ValidationException
    {
        public ValidacaoException(string message = null, IDictionary<string, string> errors = null)
            : base(message, errors)
        { }
    }
}
