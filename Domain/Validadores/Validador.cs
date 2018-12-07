using System;
using System.Collections.Generic;
using System.Linq;
using Alma.Common;
using FluentValidation.Internal;
using FluentValidation.Results;

namespace Alma.Dominio.Validadores
{
    internal sealed class Validador<T> : IValidador<T> where T : class
    {
        private FluentValidation.IValidator<T> validator;


        public Validador(FluentValidation.IValidator<T> validator)
        {
            this.validator = validator;

        }



        public void Validar(T instance)
        {
            Validar(instance, false, null);
        }
        public void Validar(T instance, params string[] ruleSet)
        {
            Validar(instance, false, ruleSet);
        }
        public void Validar(T instance, bool executeCommonRules, params string[] ruleSet)
        {

            if (instance == null)
                return;
            ValidationResult result = null;
            if (ruleSet != null)
            {
                ruleSet = ruleSet.Where(x => !string.IsNullOrWhiteSpace(x) && !"default".Equals(x.Trim().ToLower())).Select(x => x.Trim()).ToArray();
                if (executeCommonRules)
                    ruleSet = new string[] { "default" }.Concat(ruleSet).ToArray();
            }

            if (ruleSet == null || ruleSet.Length == 0)
                result = validator.Validate(instance);
            else
            {
                var context = new FluentValidation.ValidationContext(
                    instance,
                    new PropertyChain(),
                    new RulesetValidatorSelector(ruleSet));
                result = validator.Validate(context);
            }


            if (result != null && !result.IsValid)
            {
                var errors = result.Errors.ToArray();

                throw new ValidacaoException(string.Format("A entidade {0} é inválida.", typeof(T).Name),
                    result.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(e => e.Key, e => string.Join("; ", e.ToArray().Select(x => x.ErrorMessage)))
                    );
            }

        }

        public Exception ConvertDatabaseValidation(Exception ex, string constraint, string message)
        {
            return ConvertDatabaseValidation(ex, constraint, null, message);
        }

        public Exception ConvertDatabaseValidation(Exception ex, string constraint, string key, string message)
        {
            if (ex.Message.Contains(constraint))
            {
                var dict = new Dictionary<string, string>();
                dict.Add(key, message);
                return new ValidacaoException(message, dict);
            }
            else if (ex.InnerException != null)
            {
                return ConvertDatabaseValidation(ex.InnerException, constraint, key, message);
            }
            else
                return ex;
        }
    }


}
