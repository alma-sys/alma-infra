using System;
using System.Threading;

namespace Alma.TestHelper.DataBuilder
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
