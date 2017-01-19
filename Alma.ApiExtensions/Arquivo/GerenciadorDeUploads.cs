using System;
using System.IO;
using System.Threading;
using System.Web;

namespace Alma.ApiExtensions.Arquivo
{
    public class GerenciadorDeUploads
    {
        static GerenciadorDeUploads()
        {
            try
            {
                PathUploads = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "alma-infra-uploads");
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

                lockLimpeza = new object();
                Limpeza();
            }
            catch (Exception e)
            {
                throw new ApplicationException("Erro ao inicializar o gerenciador de uploads, verifique permissões de pasta", e);
            }
        }



        public static String PathUploads { get; protected set; }
        private static String PathSession
        {
            get
            {
                if (HttpContext.Current.Session == null)
                {
                    return PathUploads;
                }

                var pathSession = Path.Combine(PathUploads, HttpContext.Current.Session.SessionID);
                if (!Directory.Exists(pathSession))
                {
                    Directory.CreateDirectory(pathSession);
                }
                return pathSession;
            }
        }
        private static Object lockLimpeza;
        private static DateTime ultimaLimpeza;

        static void Limpeza()
        {
            if (Monitor.TryEnter(lockLimpeza, 10))
            {
                if (ultimaLimpeza < DateTime.Now.AddMinutes(-10))
                {
                    var pastas = Directory.GetDirectories(PathUploads);
                    foreach (var pasta in pastas)
                    {
                        var arquivos = Directory.GetFiles(pasta);
                        foreach (var arquivo in arquivos)
                        {
                            var fi = new FileInfo(arquivo);
                            if (fi.CreationTime < DateTime.Now.AddMinutes(-30))
                            {
                                File.Delete(arquivo);
                            }
                        }
                        if (Directory.GetFiles(pasta).Length == 0)
                        {
                            Directory.Delete(pasta);
                        }
                    }
                    ultimaLimpeza = DateTime.Now;
                }
                Monitor.Exit(lockLimpeza);
            }
        }

        public static String Salvar(String nome, Byte[] arquivo)
        {
            return SalvarInterno(nome, arquivo);
        }

        public static FileInfo Arquivo(string key)
        {
            if (!String.IsNullOrWhiteSpace(key))
            {
                String path = Path.Combine(PathSession, key);
                if (File.Exists(path))
                {
                    return new FileInfo(path);
                }
            }

            // Não achou nada
            return null;
        }

        public static String Renomear(String arquivo, string nome = null)
        {
            var fi = new FileInfo(arquivo);

            nome = String.IsNullOrWhiteSpace(nome) ? fi.Name : nome;

            string key = GerarKey(nome);

            File.Move(arquivo, Path.Combine(PathUploads, key));

            return key;
        }

        private static String SalvarInterno(String nome, Byte[] arquivo)
        {
            string key = GerarKey(nome);

            var path = Path.Combine(PathSession, key);
            File.WriteAllBytes(path, arquivo);

            // Talvez fazer um timer
            Limpeza();

            return key;
        }

        private static string GerarKey(string nome)
        {
            return String.Format("{0}_@_{1}", Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15).ToUpper(), nome);
        }

        public static Byte[] Recuperar(String key)
        {
            if (!String.IsNullOrWhiteSpace(key))
            {
                String path = Path.Combine(PathSession, key);
                if (File.Exists(path))
                {
                    return File.ReadAllBytes(path);
                }
            }

            // Não achou nada
            return null;
            //throw new FileNotFoundException("Arquivo temporário não encontrado");
        }
    }
}
