using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
//using FluentValidation;
//[assembly: PreApplicationStartMethod(typeof(Alma.Infra.Config), "Iniciar")]

namespace Alma.Core
{
    public static class Config
    {
        private static Dictionary<string, Assembly[]> ListarAssembliesDeMapeamento()
        {
            var maps = new List<string>();
            var cons = new List<string>();
            for (int i = 0; i <= 5; i++)
            {
                var ass = ConfigurationManager.AppSettings["alma:mapeamentoentidades" + (i == 0 ? "" : i.ToString())];
                var cnn = ConfigurationManager.AppSettings["alma:conexao" + (i == 0 ? "" : i.ToString())];
                if (string.IsNullOrWhiteSpace(ass) || string.IsNullOrWhiteSpace(cnn))
                    continue;
                maps.Add(ass);
                cons.Add(cnn);
            }

            var exMessage = "Missing or invalid alma:mapeamentoentidades App Setting. Check your .config file. Valid values: semi-colon (;) separated assembly names that contains entities and mapping.";
            exMessage += "Each alma:mapeamentoentidades must have a corresponding alma:orm:conexao. Eg.: <add name=\"alma:mapeamentoentidades2\">, <add name=\"alma:conexao2\">";
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
