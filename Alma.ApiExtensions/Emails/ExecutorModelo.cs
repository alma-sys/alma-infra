using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Razor;
using Alma.Core;

namespace Alma.Infra.Emails
{

    public static class ExecutorModelo
    {


        public static string Executar<T>(Assembly assemblyComModelo, string nomeModelo, T model) where T : class
        {
            var res = assemblyComModelo.GetManifestResourceNames();
            var modelo = res.Where(x => x.Equals(nomeModelo)).SingleOrDefault();
            if (modelo == null)
                throw new ApplicationException(string.Format("O modelo '{0}' não foi encontrado.", nomeModelo));

            var template = new StreamReader(assemblyComModelo.GetManifestResourceStream(modelo));
            var modelType = model.GetType();

            var language = RazorCodeLanguage.GetLanguageByExtension(System.IO.Path.GetExtension(nomeModelo));
            var host = new RazorEngineHost(language)
            {
                DefaultBaseClass = string.Format("TemplateBase<{0}>", typeof(T).FullName),
                DefaultClassName = "Template",
                DefaultNamespace = typeof(TemplateBase<>).Namespace,
            };
            host.NamespaceImports.Add("System");
            var engine = new RazorTemplateEngine(host);

            var razorResult = engine.GenerateCode(template);

            var referencedAssemblies = new List<string>();
            referencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            referencedAssemblies.AddRange(GetReferencedAssemblies<T>());
            referencedAssemblies.AddRange(GetReferencedAssemblies<IIdNome>());
            var compilerParameters = new CompilerParameters(referencedAssemblies.Distinct().ToArray());

            var codeProvider = (CodeDomProvider)Activator.CreateInstance(language.CodeDomProviderType);
            var compilerResults = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResult.GeneratedCode);

            if (compilerResults.Errors.HasErrors)
            {
                throw new AggregateException("Pã", compilerResults.Errors.OfType<CompilerError>()
                    .Select(x => new Exception(x.ToString())).ToArray());

            }

            var templateInstance = (TemplateBase<T>)compilerResults.CompiledAssembly.CreateInstance(string.Format("{0}.{1}", host.DefaultNamespace, host.DefaultClassName));
            templateInstance.Model = model;
            return templateInstance.ToString();
        }

        private static IEnumerable<string> GetReferencedAssemblies<T>()
        {
            var declaringAssembly = typeof(T).Assembly;
            yield return declaringAssembly.Location;

            foreach (var assemblyName in declaringAssembly.GetReferencedAssemblies())
            {
                yield return Assembly.ReflectionOnlyLoad(assemblyName.FullName).Location;
            }
        }
    }

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class TemplateBase<T>
    {
        public abstract void Execute();

        private StringBuilder builder;

        public T Model { get; set; }

        public void Write(object value)
        {
            WriteLiteral(value);
        }

        public void WriteLiteral(object value)
        {
            builder.Append(value);
        }

        public override string ToString()
        {
            builder = new StringBuilder();
            Execute();
            return builder.ToString();
        }

        //public virtual void WriteAttribute(string name, PositionTagged<string> prefix, PositionTagged<string> suffix, params AttributeValue[] values)
        //{
        //    bool first = true;
        //    bool wroteSomething = false;
        //    if (values.Length == 0)
        //    {
        //        // Explicitly empty attribute, so write the prefix and suffix
        //        WritePositionTaggedLiteral(prefix);
        //        WritePositionTaggedLiteral(suffix);
        //    }
        //    else
        //    {
        //        for (int i = 0; i < values.Length; i++)
        //        {
        //            AttributeValue attrVal = values[i];
        //            PositionTagged<object> val = attrVal.Value;

        //            bool? boolVal = null;
        //            if (val.Value is bool)
        //            {
        //                boolVal = (bool)val.Value;
        //            }

        //            if (val.Value != null && (boolVal == null || boolVal.Value))
        //            {
        //                string valStr = val.Value as string;
        //                string valToString = valStr;
        //                if (valStr == null)
        //                {
        //                    valToString = val.Value.ToString();
        //                }
        //                if (boolVal != null)
        //                {
        //                    valToString = name;
        //                }

        //                if (first)
        //                {
        //                    WritePositionTaggedLiteral(prefix);
        //                    first = false;
        //                }
        //                else
        //                {
        //                    WritePositionTaggedLiteral(attrVal.Prefix);
        //                }

        //                if (attrVal.Literal)
        //                {
        //                    WriteLiteral(valToString);
        //                }
        //                else
        //                {
        //                    if (/*val.Value is IEncodedString && */boolVal == null)
        //                    {
        //                        Write(val.Value); // Write value
        //                    }
        //                    else
        //                    {
        //                        Write(valToString); // Write value
        //                    }
        //                }
        //                wroteSomething = true;
        //            }
        //        }
        //        if (wroteSomething)
        //        {
        //            WritePositionTaggedLiteral(suffix);
        //        }
        //    }
        //}

        //private void WritePositionTaggedLiteral(PositionTagged<string> value)
        //{
        //    WriteLiteral(value.Value);
        //}

    }


}
