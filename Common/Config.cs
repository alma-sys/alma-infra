using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace Alma.Common
{
    public static class Config
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public const string cfgRoot = "alma:";
        public const string cfgAssemblies = cfgRoot + "assemblies";
        public const string cfgConnection = cfgRoot + "connection";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private const string commonAssembly = "Alma.Common";
        private const string dataAssembly = "Alma.DataAccess";

        private static Dictionary<string, Assembly[]> ListMappedAssemblies()
        {
            var assemblies = new List<string>();
            var connections = new List<string>();
            for (int i = 0; i <= 5; i++)
            {
                var ass = Config.AppSettings[cfgAssemblies + (i == 0 ? "" : i.ToString())];
                var cnn = Config.AppSettings[cfgConnection + (i == 0 ? "" : i.ToString())];
                if (string.IsNullOrWhiteSpace(ass) || string.IsNullOrWhiteSpace(cnn))
                    continue;
                assemblies.Add(ass);
                connections.Add(cnn);
            }

            var exMessage = $"Missing or invalid {cfgAssemblies} App Setting. Check your .config file. Valid values: semi-colon (;) separated assembly names that contains entities and mapping.";
            exMessage += $"Each {cfgAssemblies} must have a corresponding {cfgConnection}. Eg.: <add name=\"{cfgAssemblies}2\">, <add name=\"{cfgConnection}2\">";
            if (assemblies.Count == 0)
                throw new System.Configuration.ConfigurationErrorsException(exMessage);

            var dict = new Dictionary<string, Assembly[]>();

            for (var i = 0; i < connections.Count; i++)
            {
                var ass = assemblies[i];

                foreach (var basicAssembly in new[] { commonAssembly, dataAssembly })
                {
                    if (!ass.Contains($"{basicAssembly};")) // Injecting basic assemblies. 
                        ass += $";{basicAssembly};";
                }
                //TODO: Find a way to automatically inject ORM assembly.

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
                            throw new System.Configuration.ConfigurationErrorsException(exMessage + " Could not load '" + a + "'.", ex);
                        }
                    }
                }
                dict.Add(connections[i], list.ToArray());
            }

            return dict;

        }


        private static IDictionary<string, Assembly[]> _AssembliesCached = null;
        /// <summary>
        /// List all mapped assemblies that represents dependencies
        /// </summary>
        public static IDictionary<string, Assembly[]> MappedAssemblies
        {
            get
            {
                if (_AssembliesCached == null)
                    _AssembliesCached = ListMappedAssemblies();
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


        public static NameValueCollection AppSettings
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings;
            }
        }

        public static System.Configuration.ConnectionStringSettingsCollection ConnectionStrings
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings;
            }
        }
    }

}
