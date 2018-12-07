using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Alma.Common
{
    /// <summary>
    /// Validation exception class to be thrown from the domain context.
    /// </summary>
    public abstract class ValidationException : ApplicationException
    {
        public ValidationException(string message)
            : this(message, null)
        { }
        public ValidationException(IDictionary<string, string> errors)
            : this(null, errors)
        { }

        public ValidationException(string message, IDictionary<string, string> errors)
            : base(GetDefaultMessage(message, errors))
        {
            this.Errors = new ReadOnlyDictionary<string, string>(errors ?? new Dictionary<string, string>());
        }

        private static string GetDefaultMessage(string message, IDictionary<string, string> errors)
        {
            if (!string.IsNullOrWhiteSpace(message))
                return message;
            else if (errors != null && errors.Count == 1)
                return errors.Values.First();
            else
                return null;
        }

        public IDictionary<string, string> Errors { get; private set; }
    }
}
