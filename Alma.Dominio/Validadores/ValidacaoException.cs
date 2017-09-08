using System.Collections.Generic;

namespace Alma.Dominio.Validadores
{
    internal class ValidacaoException : Core.ValidacaoException
    {
        public ValidacaoException(string message = null, IDictionary<string, string> errors = null)
            : base(message, errors)
        { }
    }
}
