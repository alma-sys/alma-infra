using System.Collections.Generic;

namespace Alma.Domain.Validators
{
    internal class ValidationException : Common.ValidationException
    {
        public ValidationException(string message = null, IDictionary<string, string> errors = null)
            : base(message, errors)
        { }
    }
}
