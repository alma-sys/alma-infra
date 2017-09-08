using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        public const string cfgOrm = Core.Config.cfgRoot + "orm";
        public const string cfgExecutarMigracoes = Core.Config.cfgRoot + cfgOrm + ":executar-migracoes";
        public const string cfgLazy = Core.Config.cfgRoot + cfgOrm + ":lazy";
        public const string cfgIsolationLevel = Core.Config.cfgRoot + cfgOrm + ":isolation-level";
        public const string cfgLog = Core.Config.cfgRoot + cfgOrm + ":log";

        public static ORM ORM
        {
            get
            {
                try
                {
                    var orm = (ORM)Enum.Parse(typeof(ORM), ConfigurationManager.AppSettings[cfgOrm]);
                    return orm;
                }
                catch (Exception ex)
                {
                    var possibleValues =
                        string.Join(", ", (from ORM e in Enum.GetValues(typeof(ORM))
                                           select e.ToString()));

                    throw new ConfigurationErrorsException(
                        $"Missing or invalid {cfgOrm} App Setting. Check your .config file. Valid values: " +
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
                    var opt = ConfigurationManager.AppSettings[cfgExecutarMigracoes];
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

        public static IDictionary<string, Assembly[]> AssembliesMapeadas => Core.Config.AssembliesMapeadas;

        public static bool AtivarLazy
        {
            get
            {
                try
                {
                    var opt = ConfigurationManager.AppSettings[cfgLazy];
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

        /// <summary>
        /// Retorna o tipo de isolamento de transação
        /// </summary>
        public static IsolationLevel? IsolationLevel
        {
            get
            {
                try
                {
                    var opt = ConfigurationManager.AppSettings[cfgIsolationLevel];
                    if (string.IsNullOrWhiteSpace(opt))
                        return null;
                    else
                        return (IsolationLevel)Enum.Parse(typeof(IsolationLevel), opt);
                }
                catch (Exception)
                {
                    throw new ConfigurationErrorsException($"Valor inválido para {cfgIsolationLevel}");
                }
            }
        }

        public static bool AtivarLog
        {
            get
            {
                try
                {
                    var opt = ConfigurationManager.AppSettings[cfgLog];
                    if (string.IsNullOrWhiteSpace(opt))
                        return false;
                    else
                        return Convert.ToBoolean(opt);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static string ResolveConnectionName(Type type)
        {
            var assemblies = AssembliesMapeadas;
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
