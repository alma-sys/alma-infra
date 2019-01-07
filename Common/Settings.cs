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
        }


        public ORM ORM { get; private set; }

        public IReadOnlyList<ConnectionSetting> ConnectionStrings { get; private set; }

        public bool ExecuteMigrations { get; private set; }
        public bool PrepareCommands { get; private set; }
        public bool EnableLazyLoad { get; private set; }
        public IsolationLevel? IsolationLevel { get; private set; }
        public bool EnableProfiling { get; private set; }

        public bool Https { get; private set; }


        public LogSetting Logging { get; private set; }
        public JwtSetting Jwt { get; private set; }
        public SmtpSetting Smtp { get; private set; }


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
        public string Name { get; private set; }
        public string ConnectionString { get; private set; }
        public DBMS Provider { get; private set; }
        public IReadOnlyList<string> Assemblies { get; private set; }
    }

    public struct JwtSetting
    {
        public string Base64Key { get; private set; }
        public string Issuer { get; private set; }
        public int ExpiryInMintures { get; private set; }
        public IList<string> Audiences { get; private set; }
    }

    public struct SmtpSetting
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        public bool Ssl { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
    }

    public struct LogSetting
    {
        public System.Net.Mail.MailAddressCollection MailDestinations { get; private set; }
        public System.Net.Mail.MailAddress MailFrom { get; private set; }
        public int MaxEmailsPerDay => 65;
        public bool Enable { get; private set; }

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
