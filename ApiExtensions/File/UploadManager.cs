using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading;

namespace Alma.ApiExtensions.File
{
    public class UploadManager
    {
        static UploadManager()
        {
            PathUploads = Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid().ToString().Substring(0, 5)}-Uploads");
            try
            {
                if (!Directory.Exists(PathUploads))
                {
                    Directory.CreateDirectory(PathUploads);
                }

                var machine = Environment.MachineName;
                PathUploads = Path.Combine(PathUploads, machine);
                if (!Directory.Exists(PathUploads))
                {
                    Directory.CreateDirectory(PathUploads);
                }

                lockCleaning = new object();
                DoClean();
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Error during upload manager initialization, please check permissions for '{PathUploads}'.", e);
            }
        }



        public static string PathUploads { get; protected set; }
        private static string PathSession
        {
            get
            {
                var acessor = new HttpContextAccessor(); //untested
                if (acessor.HttpContext.Session == null)
                {
                    return PathUploads;
                }

                var pathSession = Path.Combine(PathUploads, acessor.HttpContext.Session.Id);
                if (!Directory.Exists(pathSession))
                {
                    Directory.CreateDirectory(pathSession);
                }
                return pathSession;
            }
        }
        private static object lockCleaning;
        private static DateTime lastCleaning;

        static void DoClean()
        {
            if (Monitor.TryEnter(lockCleaning, 10))
            {
                if (lastCleaning < DateTime.Now.AddMinutes(-10))
                {
                    var folders = Directory.GetDirectories(PathUploads);
                    foreach (var folder in folders)
                    {
                        var files = Directory.GetFiles(folder);
                        foreach (var file in files)
                        {
                            var fi = new FileInfo(file);
                            if (fi.CreationTime < DateTime.Now.AddMinutes(-30))
                            {
                                System.IO.File.Delete(file);
                            }
                        }
                        if (Directory.GetFiles(folder).Length == 0)
                        {
                            Directory.Delete(folder);
                        }
                    }
                    lastCleaning = DateTime.Now;
                }
                Monitor.Exit(lockCleaning);
            }
        }

        public static string Save(String nome, Byte[] arquivo)
        {
            return SaveImpl(nome, arquivo);
        }

        public static FileInfo File(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                string path = Path.Combine(PathSession, key);
                if (System.IO.File.Exists(path))
                {
                    return new FileInfo(path);
                }
            }

            // Didn't find any
            return null;
        }

        public static string Rename(String arquivo, string nome = null)
        {
            var fi = new FileInfo(arquivo);

            nome = String.IsNullOrWhiteSpace(nome) ? fi.Name : nome;

            string key = GenerateKey(nome);

            System.IO.File.Move(arquivo, Path.Combine(PathUploads, key));

            return key;
        }

        private static String SaveImpl(String nome, Byte[] arquivo)
        {
            string key = GenerateKey(nome);

            var path = Path.Combine(PathSession, key);
            System.IO.File.WriteAllBytes(path, arquivo);

            // TODO: Check if a timer is needed
            DoClean();

            return key;
        }

        private static string GenerateKey(string nome)
        {
            return String.Format("{0}_@_{1}", Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15).ToUpper(), nome);
        }

        public static Byte[] Recuperar(String key)
        {
            if (!String.IsNullOrWhiteSpace(key))
            {
                String path = Path.Combine(PathSession, key);
                if (System.IO.File.Exists(path))
                {
                    return System.IO.File.ReadAllBytes(path);
                }
            }

            // Didn't find any
            return null;
            //throw new FileNotFoundException("Cannot find temporary file.");
        }
    }
}
