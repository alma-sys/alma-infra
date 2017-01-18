
using System;
namespace Alma.Infra.Validadores
{
    public interface IValidador<T> where T : class
    {

        Exception ConvertDatabaseValidation(Exception ex, string constraint, string message);
        Exception ConvertDatabaseValidation(Exception ex, string constraint, string key, string message);

        void Validar(T instance);
        void Validar(T instance, params string[] ruleSet);
        void Validar(T instance, bool executeCommonRules, params string[] ruleSet);
    }
}
