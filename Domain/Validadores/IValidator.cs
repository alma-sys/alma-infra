using System;

namespace Alma.Domain.Validators
{
    public interface IValidator<T> where T : class
    {

        Exception ConvertDatabaseValidation(Exception ex, string constraint, string message);
        Exception ConvertDatabaseValidation(Exception ex, string constraint, string key, string message);

        void Validate(T instance);
        void Validate(T instance, params string[] ruleSet);
        void Validate(T instance, bool executeCommonRules, params string[] ruleSet);
    }
}
