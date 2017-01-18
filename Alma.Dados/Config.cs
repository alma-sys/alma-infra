using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Alma.Dados
{
    public enum ORM
    {
        NaoDefinido,
        NHibernate,
        EntityFramework
    }


    public enum DBMS
    {
        NaoDefinido,
        MsSql,
        Oracle,
        MySql,
        PostgreSql,
        SqLite
    }

    public static class Config
    {
        private static bool _startWasCalled;

        public static void Iniciar()
        {
            if (_startWasCalled) return;
            _startWasCalled = true;


            if (ORM != ORM.NHibernate)
                throw new NotImplementedException(ORM.ToString() + " was not yet implemented.");
            //DynamicModuleUtility.RegisterModule(typeof(HandleErrorModule));
            //DynamicModuleUtility.RegisterModule(typeof(AlmaConfigAfterGlobal));

            //.ConfigurarValidator();
            //.ConfigurarSeguranca();
        }


        public static ORM ORM
        {
            get
            {
                try
                {
                    var orm = (ORM)Enum.Parse(typeof(ORM), ConfigurationManager.AppSettings["alma:orm"]);
                    return orm;
                }
                catch (Exception ex)
                {
                    var possibleValues =
                        string.Join(", ", (from ORM e in Enum.GetValues(typeof(ORM))
                                           select e.ToString()));

                    throw new ConfigurationErrorsException(
                        "Missing or invalid alma:orm App Setting. Check your .config file. Valid values: " +
                        possibleValues, ex);
                }
            }
        }

        public static DBMS DeterminarDBMS(string key)
        {
            var cn = ConfigurationManager.ConnectionStrings[key];
            if (cn == null)
                throw new ConfigurationErrorsException("Cannot find connection string: " + key);
            if (cn.ProviderName.Contains("SqlClient"))
                return DBMS.MsSql;
            else if (cn.ProviderName.Contains("Oracle"))
                return DBMS.Oracle;
            else if (cn.ProviderName.ToLower().Contains("mysql"))
                return DBMS.MySql;
            else
                throw new NotImplementedException("Not implemented provider: " + cn.ProviderName);

        }

        public static bool ExecutarMigracoes
        {
            get
            {
                try
                {
                    var opt = ConfigurationManager.AppSettings["alma:orm:executar-migracoes"];
                    if (string.IsNullOrWhiteSpace(opt))
                        return true;
                    else
                        return Convert.ToBoolean(opt);
                }
                catch (Exception)
                {
                    return true;
                }
            }
        }

        public static bool IsManagedOracle(string key)
        {
            return DeterminarDBMS(key) == DBMS.Oracle &&
                ConfigurationManager.ConnectionStrings[key].ProviderName.Contains("Managed");

        }


        private static Dictionary<string, Assembly[]> ListarAssembliesDeMapeamento()
        {
            var maps = new List<string>();
            var cons = new List<string>();
            for (int i = 0; i <= 5; i++)
            {
                var ass = ConfigurationManager.AppSettings["alma:orm:mapeamentoentidades" + (i == 0 ? "" : i.ToString())];
                var cnn = ConfigurationManager.AppSettings["alma:orm:conexao" + (i == 0 ? "" : i.ToString())];
                if (string.IsNullOrWhiteSpace(ass) || string.IsNullOrWhiteSpace(cnn))
                    continue;
                maps.Add(ass);
                cons.Add(cnn);
            }

            var exMessage = "Missing or invalid alma:orm:mapeamentoentidades App Setting. Check your .config file. Valid values: semi-colon (;) separated assembly names that contains entities and mapping.";
            exMessage += "Each alma:orm:mapeamentoentidades must have a corresponding alma:orm:conexao. Eg.: <add name=\"alma:orm:mapeamentoentidades2\">, <add name=\"alma:orm:conexao2\">";
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
