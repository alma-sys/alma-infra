using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Alma.Common
{
    public static class Config
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public const string cfgRoot = "alma:";
        public const string cfgAssemblies = cfgRoot + "mapeamentoentidades";
        public const string cfgConnection = cfgRoot + "conexao";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member


        private static Dictionary<string, Assembly[]> ListarAssembliesDeMapeamento()
        {
            var maps = new List<string>();
            var cons = new List<string>();
            for (int i = 0; i <= 5; i++)
            {
                var ass = ConfigurationManager.AppSettings[cfgAssemblies + (i == 0 ? "" : i.ToString())];
                var cnn = ConfigurationManager.AppSettings[cfgConnection + (i == 0 ? "" : i.ToString())];
                if (string.IsNullOrWhiteSpace(ass) || string.IsNullOrWhiteSpace(cnn))
                    continue;
                maps.Add(ass);
                cons.Add(cnn);
            }

            var exMessage = $"Missing or invalid {cfgAssemblies} App Setting. Check your .config file. Valid values: semi-colon (;) separated assembly names that contains entities and mapping.";
            exMessage += $"Each {cfgAssemblies} must have a corresponding {cfgConnection}. Eg.: <add name=\"{cfgAssemblies}2\">, <add name=\"{cfgConnection}2\">";
            if (maps.Count == 0)
                throw new ConfigurationErrorsException(exMessage);

            var dict = new Dictionary<string, Assembly[]>();

            for (var i = 0; i < cons.Count; i++)
            {
                var ass = maps[i];
                if (!ass.Contains("Alma.Core;")) //Colocando assemblies basicos para busca de modulos
                    ass += ";Alma.Core;";
                if (!ass.Contains("Alma.Dados;"))
                    ass += ";Alma.Dados;";
                //TODO: ver como colocar automaticamente o assembly do orm.

                var list = new List<Assembly>();
                foreach (var a in ass.Split(';'))
                {
                    if (!string.IsNullOrWhiteSpace(a))
                    {
                        try
                        {
                            var assembly = Assembly.Load(new AssemblyName(a.Trim()));
                            list.Add(assembly);
                        }
                        catch (Exception ex)
                        {
                            throw new ConfigurationErrorsException(exMessage + " Could not load '" + a + "'.", ex);
                        }
                    }
                }
                dict.Add(cons[i], list.ToArray());
            }

            return dict;

        }


        private static IDictionary<string, Assembly[]> _AssembliesCached = null;
        /// <summary>
        /// Lista de assemblies que foram mapeadas para a plataforma.
        /// </summary>
        public static IDictionary<string, Assembly[]> AssembliesMapeadas
        {
            get
            {
                if (_AssembliesCached == null)
                    _AssembliesCached = ListarAssembliesDeMapeamento();
                return _AssembliesCached;
            }
        }


        internal static IEnumerable<Assembly> GetReferencingAssemblies(string assemblyName = null)
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (string.IsNullOrWhiteSpace(assemblyName) || IsCandidateLibrary(library, assemblyName))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
            }
            return assemblies;
        }

        internal static bool IsCandidateLibrary(RuntimeLibrary library, string assemblyName)
        {
            return library.Name == (assemblyName)
                || library.Dependencies.Any(d => d.Name.StartsWith(assemblyName));
        }
    }
}
