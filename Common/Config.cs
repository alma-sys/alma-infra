using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Alma.Common
{
    public static class Config
    {
        private const string commonAssembly = "Alma.Common";
        private const string dataAssembly = "Alma.DataAccess";

        private static Dictionary<string, Assembly[]> ListMappedAssemblies()
        {
            var assemblies = new List<string>();
            var connections = new List<string>();


            var dict = new Dictionary<string, Assembly[]>();
            foreach (var cnn in Settings.ConnectionStrings)
            {
                var list = new List<Assembly>();
                var stringList = new List<string>(cnn.Assemblies);

                foreach (var basicAssembly in new[] { commonAssembly, dataAssembly, $"{dataAssembly}.Orm{cnn.Provider.ToString()}" })
                {
                    if (!stringList.Contains($"{basicAssembly}")) // Injecting basic assemblies. 
                        stringList.Add(basicAssembly);
                }

                foreach (var a in stringList)
                {
                    try
                    {
                        var assembly = Assembly.Load(new AssemblyName(a.Trim()));
                        list.Add(assembly);
                    }
                    catch (Exception ex)
                    {
                        var exMessage = $"Missing or invalid {nameof(cnn.Assemblies)} App Setting. Check your appsettings.json file. Valid values: array of assembly names that contains entities and mapping.";

                        throw new System.Configuration.ConfigurationErrorsException(exMessage + " Could not load '" + a + "'.", ex);
                    }
                }
                dict.Add(cnn.Name, list.ToArray());
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


        /// <summary>
        /// Must be called by the Startup class of an ASPNET application.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="builder"></param>
        /// <param name="dataAccessModule"></param>
        public static void Boot(IConfiguration configuration, ContainerBuilder builder)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            Settings = configuration.GetSection("Alma").Get<Settings>();
            Settings.Validate();
            builder.RegisterInstance<Settings>(Settings).AsSelf();


            builder.RegisterAssemblyModules(MappedAssemblies.SelectMany(x => x.Value).ToArray());

        }


        public static Settings Settings { get; private set; }


        /// <summary>
        /// Resolve the connection name by assembly type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ResolveConnectionName(Type type)
        {
            var assemblies = Alma.Common.Config.MappedAssemblies;
            var assembly = type.Assembly;
            foreach (var key in assemblies.Keys)
            {
                if (assemblies[key].Contains(assembly))
                    return key;
            }
            return null;
        }

    }

}
