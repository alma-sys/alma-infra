using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace Alma.Core
{
    public static class Config
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public const string cfgRoot = "alma:";
        public const string cfgMapeamentoEntidades = cfgRoot + "mapeamentoentidades";
        public const string cfgConexao = cfgRoot + "conexao";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member


        private static Dictionary<string, Assembly[]> ListarAssembliesDeMapeamento()
        {
            var maps = new List<string>();
            var cons = new List<string>();
            for (int i = 0; i <= 5; i++)
            {
                var ass = ConfigurationManager.AppSettings[cfgMapeamentoEntidades + (i == 0 ? "" : i.ToString())];
                var cnn = ConfigurationManager.AppSettings[cfgConexao + (i == 0 ? "" : i.ToString())];
                if (string.IsNullOrWhiteSpace(ass) || string.IsNullOrWhiteSpace(cnn))
                    continue;
                maps.Add(ass);
                cons.Add(cnn);
            }

            var exMessage = $"Missing or invalid {cfgMapeamentoEntidades} App Setting. Check your .config file. Valid values: semi-colon (;) separated assembly names that contains entities and mapping.";
            exMessage += $"Each {cfgMapeamentoEntidades} must have a corresponding {cfgConexao}. Eg.: <add name=\"{cfgMapeamentoEntidades}2\">, <add name=\"{cfgConexao}2\">";
            if (maps.Count == 0)
                throw new ConfigurationErrorsException(exMessage);

            var dict = new Dictionary<string, Assembly[]>();

            for (var i = 0; i < cons.Count; i++)
            {
                var ass = maps[i];
                var list = new List<Assembly>();
                foreach (var a in ass.Split(';'))
                {
                    if (!string.IsNullOrWhiteSpace(a))
                    {
                        try
                        {
                            var assembly = Assembly.Load(a.Trim());
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


    }
}
