using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Alma.DataAccess
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// List of supported ORM's
    /// </summary>
    public enum ORM
    {
        Undefined,
        NHibernate,
        EntityFramework,
        MongoMapping
    }

    /// <summary>
    /// List of supported databases and storages.
    /// </summary>
    public enum DBMS
    {
        Undefined,
        MsSql,
        Oracle,
        MySql,
        PostgreSql,
        SqLite,
        MongoDB
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Configurações da plataforma.
    /// </summary>
    public static class Config
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public const string cfgOrm = Common.Config.cfgRoot + "orm";
        public const string cfgExecuteMigrations = cfgOrm + ":run-migrations";
        public const string cfgPrepareCommands = cfgOrm + ":prepare-commands";
        public const string cfgLazy = cfgOrm + ":lazy";
        public const string cfgIsolationLevel = cfgOrm + ":isolation-level";
        public const string cfgLog = cfgOrm + ":log";
        public const string cfgMiniProfiler = cfgOrm + ":miniprofiler";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Determina qual o ORM a ser usado.
        /// </summary>
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
                        $"Missing or invalid {cfgOrm} App Setting. Check your .config or appsettings.json file. Valid values: " +
                        possibleValues, ex);
                }
            }
        }

        /// <summary>
        /// Choose the right DBMS used on your connection
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DBMS DetectDBMS(string key)
        {
            var cn = ConfigurationManager.ConnectionStrings[key];
            if (cn == null)
                throw new ConfigurationErrorsException("Cannot find connection string: " + key);
            if (cn.ProviderName.ToLower().Contains("sqlclient"))
                return DBMS.MsSql;
            else if (cn.ProviderName.ToLower().Contains("oracle"))
                return DBMS.Oracle;
            else if (cn.ProviderName.ToLower().Contains("mysql"))
                return DBMS.MySql;
            else if (cn.ProviderName.ToLower().Contains("mongo"))
                return DBMS.MongoDB;
            else
                throw new NotImplementedException("Not implemented provider: " + cn.ProviderName);

        }

        /// <summary>
        /// Returns if default orm migrations can be run.
        /// </summary>
        public static bool ExecuteMigrations
        {
            get
            {
                bool valor = true;
                try
                {
                    var opt = ConfigurationManager.AppSettings[cfgExecuteMigrations];
                    if (!string.IsNullOrWhiteSpace(opt))
                        valor = Convert.ToBoolean(opt);
                }
                catch { }
                Trace.WriteLine(valor, nameof(ExecuteMigrations));
                return valor;
            }
        }


        /// <summary>
        /// Returns if DbCommands should be 'prepared'. This is specific to some ORMs
        /// </summary>
        public static bool PrepareCommands
        {
            get
            {
                bool valor = false;
                try
                {
                    var opt = ConfigurationManager.AppSettings[cfgPrepareCommands];
                    if (!string.IsNullOrWhiteSpace(opt))
                        valor = Convert.ToBoolean(opt);
                }
                catch { }
                Trace.WriteLine(valor, nameof(PrepareCommands));
                return valor;
            }
        }

        /// <summary>
        /// Returns if this connection will be created using Oracle Managed Drivers.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsManagedOracle(string key)
        {
            return DetectDBMS(key) == DBMS.Oracle &&
                ConfigurationManager.ConnectionStrings[key].ProviderName.ToLower().Contains("managed");

        }

        /// <summary>
        /// List of all mapped assemblies.
        /// </summary>
        public static IDictionary<string, Assembly[]> MappedAssemblies => Common.Config.MappedAssemblies;

        /// <summary>
        /// Returns if lazy loading will be enabled at ORM level.
        /// </summary>
        public static bool EnableLazyLoad
        {
            get
            {
                bool valor = true;
                try
                {
                    var opt = ConfigurationManager.AppSettings[cfgLazy];
                    if (!string.IsNullOrWhiteSpace(opt))
                        valor = Convert.ToBoolean(opt);
                }
                catch { }
                Trace.WriteLine(valor, nameof(EnableLazyLoad));
                return valor;

            }
        }

        /// <summary>
        /// Returns the isolation level of each transaction.
        /// </summary>
        public static IsolationLevel? IsolationLevel
        {
            get
            {
                IsolationLevel? valor = null;
                try
                {
                    var opt = ConfigurationManager.AppSettings[cfgIsolationLevel];
                    if (!string.IsNullOrWhiteSpace(opt))
                        valor = (IsolationLevel)Enum.Parse(typeof(IsolationLevel), opt);
                }
                catch { throw new ConfigurationErrorsException($"Invalid value for {cfgIsolationLevel}"); }
                Trace.WriteLine(valor, nameof(IsolationLevel));
                return valor;
            }
        }

        /// <summary>
        /// Returns if ORM logging will be enabled.
        /// </summary>
        public static bool EnableLog
        {
            get
            {
                bool valor = false;
                try
                {
                    var opt = ConfigurationManager.AppSettings[cfgLog];
                    if (!string.IsNullOrWhiteSpace(opt))
                        valor = Convert.ToBoolean(opt);
                }
                catch { }
                Trace.WriteLine(valor, nameof(EnableLog));
                return valor;
            }
        }

        public static bool EnableMiniProfiler
        {
            get
            {
                bool valor = false;
                try
                {
                    var opt = ConfigurationManager.AppSettings[cfgMiniProfiler];
                    if (!string.IsNullOrWhiteSpace(opt))
                        valor = Convert.ToBoolean(opt);
                }
                catch { }
                Trace.WriteLine(valor, nameof(EnableMiniProfiler));
                return valor;
            }
        }

        /// <summary>
        /// Resolve the connection name by assembly type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ResolveConnectionName(Type type)
        {
            var assemblies = MappedAssemblies;
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
