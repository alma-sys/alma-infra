using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Alma.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Alma.ApiExtensions.Serializadores
{
    public static class SerializadorConstantes
    {
        private class StaticPropertyContractResolver : CamelCasePropertyNamesContractResolver
        {
            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                var baseMembers = base.GetSerializableMembers(objectType);

                var staticMembers =
                    objectType.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                baseMembers.AddRange(staticMembers);

                return baseMembers;
            }
        }


        public static string Serializar(Type tipo, bool formatado)
        {
            var serializer = new JsonSerializer();
            serializer.ContractResolver = new StaticPropertyContractResolver();
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            serializer.Formatting = formatado ? Formatting.Indented : Formatting.None;
            serializer.Serialize(writer, Activator.CreateInstance(tipo, null), tipo);

            return sb.ToString();
        }

        public static string SerializarEnum(Type enumerador, bool formatado, bool orderByCodigo = false)
        {
            var serializer = new JsonSerializer();
            serializer.ContractResolver = new StaticPropertyContractResolver();
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            serializer.Formatting = formatado ? Formatting.Indented : Formatting.None;

            var lista = new List<dynamic>();

            foreach (var item in Enum.GetValues(enumerador))
            {
                lista.Add(((Enum)item).ToCodigoNome());
            }

            if (orderByCodigo)
                lista = lista.OrderBy(t => t.Codigo).ToList();
            else
                lista = lista.OrderBy(t => t.Nome).ToList();

            serializer.Serialize(writer, lista);

            return sb.ToString();
        }

        public static string SerializarEnumChar(Type enumerador, bool formatado, bool orderByCodigo = false)
        {
            var serializer = new JsonSerializer();
            serializer.ContractResolver = new StaticPropertyContractResolver();
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            serializer.Formatting = formatado ? Formatting.Indented : Formatting.None;

            var lista = new List<dynamic>();

            foreach (var item in Enum.GetValues(enumerador))
            {
                lista.Add(((Enum)item).ToCodigoCharNome());
            }

            if (orderByCodigo)
                lista = lista.OrderBy(t => t.Nome).ToList();
            else
                lista = lista.OrderBy(t => t.Codigo).ToList();

            serializer.Serialize(writer, lista);

            return sb.ToString();
        }
    }
}
