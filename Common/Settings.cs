using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Alma.Common
{
    public sealed class Settings
    {
        public Settings()
        {
            //Default values
            PrepareCommands = true;
            EnableLazyLoad = true;

            ConnectionStrings = new List<ConnectionSetting>();
        }


        public ORM ORM { get; set; }

        public List<ConnectionSetting> ConnectionStrings { get; set; }

        public bool ExecuteMigrations { get; set; }
        public bool PrepareCommands { get; set; }
        public bool EnableLazyLoad { get; set; }
        public IsolationLevel? IsolationLevel { get; set; }
        public bool EnableProfiling { get; set; }

        public bool Https { get; set; }


        public LogSetting Logging { get; set; }
        public JwtSetting Jwt { get; set; }
        public SmtpSetting Smtp { get; set; }


        internal void Validate()
        {

            if (!System.Enum.IsDefined(typeof(ORM), ORM))
            {
                var possibleValues =
                    string.Join(", ", (from ORM e in System.Enum.GetValues(typeof(ORM))
                                       select e.ToString()));

                throw new System.Configuration.ConfigurationErrorsException(
                    $"Missing or invalid {nameof(ORM)} App Setting. Check your appsettings.json file. Valid values: " +
                    possibleValues);

            }

            if (ConnectionStrings != null)
            {
                foreach (var cnn in ConnectionStrings)
                {
                    if (cnn == null)
                        throw new System.Configuration.ConfigurationErrorsException(
                           $"Missing or invalid {nameof(ConnectionStrings)} App Setting. Check your appsettings.json file.");
                    if (!System.Enum.IsDefined(typeof(DBMS), cnn.Provider))
                    {
                        var possibleValues =
                            string.Join(", ", (from DBMS e in System.Enum.GetValues(typeof(DBMS))
                                               select e.ToString()));

                        throw new System.Configuration.ConfigurationErrorsException(
                            $"Missing or invalid {nameof(cnn.Provider)} App Setting. Check your appsettings.json file. Valid values: " +
                            possibleValues);

                    }
                    if (string.IsNullOrWhiteSpace(cnn.Name))
                        throw new System.Configuration.ConfigurationErrorsException(
                            $"Missing or invalid {nameof(cnn.Name)} App Setting. Check your appsettings.json file.");
                    if (string.IsNullOrWhiteSpace(cnn.ConnectionString))
                        throw new System.Configuration.ConfigurationErrorsException(
                            $"Missing or invalid {nameof(cnn.ConnectionString)} App Setting. Check your appsettings.json file.");

                }
            }

        }

        public ConnectionSetting GetConnectionString(string connectionKey)
        {
            foreach (var c in ConnectionStrings)
                if (c.Name == connectionKey)
                    return c;
            return null;
        }

        public ConnectionSetting GetConnectionString(Type type)
        {
            var name = Alma.Common.Config.ResolveConnectionName(type);
            return GetConnectionString(name);
        }
    }

    public sealed class ConnectionSetting
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public DBMS Provider { get; set; }
        public IReadOnlyList<string> Assemblies { get; set; }
    }

    public struct JwtSetting
    {
        public string Base64Key { get; set; }
        public string Issuer { get; set; }
        public int ExpiryInMintures { get; set; }
        public IList<string> Audiences { get; set; }
    }

    public struct SmtpSetting
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public struct LogSetting
    {
        public System.Net.Mail.MailAddressCollection MailDestinations { get; set; }
        public System.Net.Mail.MailAddress MailFrom { get; set; }
        public int MaxEmailsPerDay => 65;
        public bool Enable { get; set; }

    }

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


}
