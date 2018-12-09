using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Alma.DataAccess.Api
{
    public static class DalApiClientExtensions
    {


        public static Dictionary<string, object> ObterColecao(this Dictionary<string, object> colecao, string key)
        {
            if (colecao.ContainsKey(key))
                return (Dictionary<string, object>)colecao[key];
            else
                return new Dictionary<string, object>();
        }


        public static T ObterValor<T>(this Dictionary<string, object> colecao, string key)
        {
            if (colecao.ContainsKey(key))
            {
                var obj = colecao[key];
                if (obj == null)
                    return default(T);
                if (typeof(T) == typeof(int))
                    if (obj.GetType() == typeof(string))
                    {
                        int num = 0;
                        int.TryParse((string)obj, out num);
                        obj = num;
                    }
                    else
                        obj = Convert.ToInt32(obj);
                if (typeof(T) == typeof(float))
                    obj = Convert.ToSingle(obj);
                if (obj.GetType() == typeof(ArrayList) && typeof(T).IsArray && typeof(T).GetElementType() == typeof(int))
                    obj = ((ArrayList)obj).Cast<int>().ToArray();
                if (obj.GetType() == typeof(ArrayList) && typeof(T).IsArray && typeof(T).GetElementType() == typeof(string))
                    obj = ((ArrayList)obj).Cast<string>().ToArray();
                if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
                    obj = Convert.ToDateTime(obj);
                if (typeof(T) == typeof(TimeSpan) || typeof(T) == typeof(TimeSpan?))
                {
                    if (string.IsNullOrEmpty((string)obj))
                    {
                        if (typeof(T) == typeof(TimeSpan?))
                            obj = (TimeSpan?)null;
                        else
                            obj = TimeSpan.Zero;
                    }
                    else
                    {
                        var parts = obj.ToString().Split(':');
                        int[] weights = { /*60 * 60 * 1000,*/ 60 * 1000, 1000, 1 };
                        long ms = 0;
                        for (int i = 0; i < parts.Length && i < weights.Length; i++)
                            ms += Convert.ToInt64(parts[i]) * weights[i];
                        obj = TimeSpan.FromMilliseconds(ms);
                    }
                }
                if ((typeof(T) == typeof(bool) || typeof(T) == typeof(bool?)) && obj.GetType() == typeof(string))
                {
                    if (string.IsNullOrEmpty((string)obj))
                        obj = (bool?)null;
                    else
                        obj = Boolean.Parse((string)obj);
                }

                return (T)obj;
            }
            else
                return default(T);
        }

        public static Dictionary<string, object>[] ObterArray(this Dictionary<string, object> colecao, string key)
        {
            if (colecao.ContainsKey(key))
            {
                return ((ArrayList)colecao[key]).Cast<Dictionary<string, object>>().ToArray();
            }
            else
                return new Dictionary<string, object>[] { };
        }
        public static string ObterNoXml(this XmlNode root, string tag, XmlNamespaceManager nsmgr)
        {
            return ObterNoXml<string>(root, tag, nsmgr);
        }
        public static T ObterNoXml<T>(this XmlNode root, string tag, XmlNamespaceManager nsmgr)
        {
            var node = root.SelectSingleNode($".//{tag}", nsmgr);
            if (node != null)
            {
                var dado = node.InnerText;
                if (string.IsNullOrWhiteSpace(dado))
                    return default(T);

                var tipo = typeof(T).IsGenericType ? typeof(T).GetGenericArguments()[0] : typeof(T);

                if (tipo == typeof(string))
                    return (T)(object)dado;
                else if (tipo == typeof(int))
                    return (T)(object)Convert.ToInt32(dado);
                else if (tipo == typeof(long))
                    return (T)(object)Convert.ToInt64(dado);
                else if (tipo == typeof(decimal))
                    return (T)(object)Convert.ToDecimal(dado);
                else if (tipo == typeof(float))
                    return (T)(object)Convert.ToSingle(dado);
                else if (tipo == typeof(double))
                    return (T)(object)Convert.ToDouble(dado);
                else if (tipo == typeof(DateTime))
                    return (T)(object)DateTime.Parse(dado);

                throw new NotSupportedException("Tipo não suportado");
            }
            else
                return default(T);
        }

        /// <summary>
        /// Returns null if string is empty. 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string NullIfEmpty(this string text)
        {
            return string.IsNullOrWhiteSpace(text) ? null : text;
        }

    }
}
