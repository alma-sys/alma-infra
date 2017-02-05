using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Alma.Core
{
    public class ValidacaoException : ApplicationException
    {
        public ValidacaoException(string message = null, IDictionary<string, string> errors = null)
            : base(message)
        {
            this.Errors = new ReadOnlyDictionary<string, string>(errors ?? new Dictionary<string, string>());
        }

        public IDictionary<string, string> Errors { get; private set; }
    }
}
