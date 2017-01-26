using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.SqlServer.Dac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Alma.ApiExtensions.Tests
{
    static class ConfigDatabase
    {


        private static ConnectionStringSettings ConnectionString
        {
            get
            {
                var config = ConfigurationManager.AppSettings["alma:conexao"];
                Assert.IsNotNull(config, "Conexão alma:conexao não encontrada.");
                var cn = ConfigurationManager.ConnectionStrings[config];
                Assert.IsNotNull(cn, "Conexão alma:conexao não encontrada.");
                return cn;
            }
        }

        private static string DatabaseName
        {
            get
            {
                var cnn = ConnectionString;
                var connectionSb = DbProviderFactories.GetFactory(cnn.ProviderName)
                    .CreateConnectionStringBuilder();
                connectionSb.ConnectionString = cnn.ConnectionString;

                object dbNameobj = null;
                if (!connectionSb.TryGetValue("Initial Catalog", out dbNameobj))
                    connectionSb.TryGetValue("Database", out dbNameobj);

                var dbname = dbNameobj as string;
                Assert.IsNotNull(dbname, "Database não encontrado.");
                return dbname;
            }
        }

        private static string Server
        {
            get
            {
                var cnn = ConnectionString;
                var connectionSb = DbProviderFactories.GetFactory(cnn.ProviderName)
                    .CreateConnectionStringBuilder();
                connectionSb.ConnectionString = cnn.ConnectionString;

                object dbNameobj = null;
                if (!connectionSb.TryGetValue("Data Source", out dbNameobj))
                    connectionSb.TryGetValue("Server", out dbNameobj);

                var dbname = dbNameobj as string;
                Assert.IsNotNull(dbname, "Server não encontrado.");
                return dbname;
            }
        }


        public static void BuildAndDeploy(TestContext testContext, string projetoBanco)
        {
            Trace.Write("Levantando banco de dados...");

            var projectPath = Path.Combine(projetoBanco, projetoBanco + ".sqlproj");

            var dbProject = new DirectoryInfo(testContext.DeploymentDirectory).Parent.Parent.Parent.FullName;
            dbProject = Path.Combine(dbProject, projectPath);

            Assert.IsTrue(File.Exists(dbProject), "Arquivo " + dbProject + " não encontrado.");


            var sw = new Stopwatch();
            sw.Restart();

            var project = ProjectCollection.GlobalProjectCollection.LoadProject(dbProject, "14.0");
            var projectName = project.GetPropertyValue("Name");
            ExcluirDacs(project);
            if (!project.Build(new Logger()
            {
                Verbosity = LoggerVerbosity.Minimal
            }))
                Assert.Fail(string.Format("Não foi possível compilar o projeto {0}", projectName));
            //at this point the .dacpac is built and put in the debug folder for the project

            sw.Stop();
            Trace.WriteLine(string.Format("Project {0} build Complete.  Total time: {1}", projectName, sw.Elapsed.ToString()));

            var dacFileName = LocalizarDacs(project).Single();

            StartLocalDB();

            //Class responsible for the deployment. (Connection string supplied by Trace input for now)
            var connectionForCreate = ConnectionString.ConnectionString.Replace(DatabaseName, "master");
            var conn = DbProviderFactories.GetFactory(ConnectionString.ProviderName).CreateConnection();
            conn.ConnectionString = connectionForCreate;
            conn.Open();

            var dbServices = new DacServices(connectionForCreate);

            //Wire up events for Deploy messages and for task progress (For less verbose output, don't subscribe to Message Event (handy for debugging perhaps?)
            dbServices.Message += new EventHandler<DacMessageEventArgs>(dbServices_Message);
            dbServices.ProgressChanged += new EventHandler<DacProgressEventArgs>(dbServices_ProgressChanged);


            //This Snapshot should be created by our build process using MSDeploy
            Trace.WriteLine("Snapshot Path: " + dacFileName);
            DacPackage dbPackage = DacPackage.Load(dacFileName);




            DacDeployOptions dbDeployOptions = new DacDeployOptions();
            //Cut out a lot of options here for configuring deployment, but are all part of DacDeployOptions
            dbDeployOptions.SqlCommandVariableValues.Add("debug", "false");
            dbDeployOptions.AllowIncompatiblePlatform = true;
            dbDeployOptions.BlockOnPossibleDataLoss = false;

            sw.Restart();
            dbServices.Deploy(dbPackage, DatabaseName, true, dbDeployOptions);
            sw.Stop();
            Trace.WriteLine(string.Format("Project {0} deploy Complete.  Total time: {1}", projectName, sw.Elapsed.ToString()));


        }
        private class Logger : ILogger
        {
            public string Parameters
            {
                get; set;
            }

            public LoggerVerbosity Verbosity
            {
                get; set;
            }

            public void Initialize(IEventSource eventSource)
            {
                eventSource.MessageRaised += EventSource_MessageRaised;
                eventSource.ErrorRaised += EventSource_ErrorRaised;
            }

            private void EventSource_ErrorRaised(object sender, BuildErrorEventArgs e)
            {
                Trace.WriteLine(e.Message, e.Code);
            }

            private void EventSource_MessageRaised(object sender, BuildMessageEventArgs e)
            {
                Trace.WriteLine(e.Message, e.Code);
            }

            public void Shutdown()
            {
            }
        }

        private static string[] LocalizarDacs(Project project)
        {
            var arquivos = new DirectoryInfo(project.DirectoryPath).GetFiles("*.dacpac", SearchOption.AllDirectories);
            return arquivos.Select(x => x.FullName).ToArray();
        }

        private static void ExcluirDacs(Project project)
        {
            var dacs = LocalizarDacs(project);
            foreach (var d in dacs)
                File.Delete(d);
        }

        private static void dbServices_ProgressChanged(object sender, DacProgressEventArgs e)
        {
            Trace.WriteLine(e.Message, e.Status.ToString());
        }

        private static void dbServices_Message(object sender, DacMessageEventArgs e)
        {
            Trace.WriteLine(e.Message);
        }


        public static void DropDatabase()
        {
            StartLocalDB();
            var connectionForCreate = ConnectionString.ConnectionString.Replace(DatabaseName, "master");
            var conn = DbProviderFactories.GetFactory(ConnectionString.ProviderName).CreateConnection();
            conn.ConnectionString = connectionForCreate;
            conn.Open();
            var cmd = conn.CreateCommand();


            cmd.CommandText = string.Format(@"
IF EXISTS(select * from sys.databases where name='{0}')
ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE"
, DatabaseName);
            cmd.ExecuteNonQuery();

            cmd.CommandText = string.Format(@"
IF EXISTS(select * from sys.databases where name='{0}')
DROP DATABASE [{0}]", DatabaseName);
            cmd.ExecuteNonQuery();
        }

        private static void StartLocalDB()
        {
            var localdbInstance = Regex.Replace(Server, @"\(localdb\)\\", "", RegexOptions.IgnoreCase);
            var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions\12.0");
            Assert.IsNotNull(key);
            var path = new FileInfo(key.GetValue("InstanceAPIPath") as string).Directory.Parent.Parent;

            var path_sqlLocaldb = Path.Combine(path.FullName, "Tools", "Binn", "SqlLocalDB.exe");
            if (!File.Exists(path_sqlLocaldb))
                path_sqlLocaldb = path_sqlLocaldb.Replace(" (x86)", "");
            Assert.IsTrue(File.Exists(path_sqlLocaldb));

            var procInfo = new ProcessStartInfo();
            procInfo.CreateNoWindow = true;
            procInfo.RedirectStandardError = true;
            procInfo.RedirectStandardOutput = true;
            procInfo.UseShellExecute = false;
            procInfo.FileName = path_sqlLocaldb;
            procInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;


            Process proc = null;

            procInfo.Arguments = "i";
            proc = Process.Start(procInfo);
            proc.WaitForExit();
            var listOutput = proc.StandardOutput.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (!listOutput.Contains(localdbInstance))
            {
                procInfo.Arguments = string.Format("create \"{0}\" {1}", localdbInstance, "12.0");
                proc = Process.Start(procInfo);
                proc.WaitForExit();
                Trace.WriteLine(proc.StandardOutput.ReadToEnd());
            }


            procInfo.Arguments = string.Format("start \"{0}\"", localdbInstance);
            proc = Process.Start(procInfo);
            proc.WaitForExit();
            Trace.WriteLine(proc.StandardOutput.ReadToEnd());


            procInfo.Arguments = "i";
            proc = Process.Start(procInfo);
            proc.WaitForExit();
            listOutput = proc.StandardOutput.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()).ToArray();

            CollectionAssert.Contains(listOutput, localdbInstance.ToLower());
        }
    }
}
