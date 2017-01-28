using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alma.Testes.DataBuilder
{
    public class DataBuilderBase
    {
        private static object lockGerarTag = new object();
        public static string GerarTag()
        {
            lock (lockGerarTag)
            {
                Thread.Sleep(5);
                var tag = "Z-" + DateTime.Now.ToString("yyMMddHHmmssfff");
                return tag;
            }
        }
    }
}
