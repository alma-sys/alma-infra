using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Alma.Dados
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// Lista de ORM's suportados.
    /// </summary>
    public enum ORM
    {
        NaoDefinido,
        NHibernate,
        EntityFramework,
        MongoMapping
    }

    /// <summary>
    /// Lista de tipos de banco de dados
    /// </summary>
    public enum DBMS
    {
        NaoDefinido,
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
        public const string cfgExecutarMigracoes = cfgOrm + ":executar-migracoes";
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
                        $"Missing or invalid {cfgOrm} App Setting. Check your .config file. Valid values: " +
                        possibleValues, ex);
                }
            }
        }

        /// <summary>
        /// Determina qual DBMS será usado para a conexão
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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
            else if (cn.ProviderName.ToLower().Contains("mongo"))
                return DBMS.MongoDB;
            else
                throw new NotImplementedException("Not implemented provider: " + cn.ProviderName);

        }

        /// <summary>
        /// Retorna se o lazy load está ativo.
        /// </summary>
        public static bool ExecutarMigracoes
        {
            get
            {
                bool valor = true;
                try
                {
                    var opt = ConfigurationManager.AppSettings[cfgExecutarMigracoes];
                    if (!string.IsNullOrWhiteSpace(opt))
                        valor = Convert.ToBoolean(opt);
                }
                catch { }
                Trace.WriteLine(valor, nameof(ExecutarMigracoes));
                return valor;
            }
        }


        /// <summary>
        /// Preparar comandos.
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
        /// Retorna se a configuração do oracle é Managed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsManagedOracle(string key)
        {
            return DeterminarDBMS(key) == DBMS.Oracle &&
                ConfigurationManager.ConnectionStrings[key].ProviderName.Contains("Managed");

        }

        /// <summary>
        /// Lista de assemblies que foram mapeadas para a plataforma.
        /// </summary>
        public static IDictionary<string, Assembly[]> AssembliesMapeadas => Common.Config.MappedAssemblies;

        /// <summary>
        /// Retorna se o lazy load está ativo.
        /// </summary>
        public static bool AtivarLazy
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
                Trace.WriteLine(valor, nameof(AtivarLazy));
                return valor;

            }
        }

        /// <summary>
        /// Retorna o tipo de isolamento de transação
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
                catch { throw new ConfigurationErrorsException($"Valor inválido para {cfgIsolationLevel}"); }
                Trace.WriteLine(valor, nameof(IsolationLevel));
                return valor;
            }
        }

        /// <summary>
        /// Retorna se deve ativar os logs de configuração
        /// </summary>
        public static bool AtivarLog
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
                Trace.WriteLine(valor, nameof(AtivarLog));
                return valor;
            }
        }

        public static bool AtivarMiniProfiler
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
                Trace.WriteLine(valor, nameof(AtivarMiniProfiler));
                return valor;
            }
        }

        /// <summary>
        /// Resolver nome da conexão pelo tipo.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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
