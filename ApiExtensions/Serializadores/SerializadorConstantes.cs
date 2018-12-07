﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Alma.Common;
using Alma.Common.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Alma.ApiExtensions.Serializers
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
        public static string SerializarEnum<T>(bool formatado, bool orderById = false) where T : struct
        {
            return SerializarEnum(typeof(T), formatado, orderById);
        }
        public static string SerializarEnum(Type enumerador, bool formatado, bool orderById = false)
        {
            var serializer = new JsonSerializer();
            serializer.ContractResolver = new StaticPropertyContractResolver();
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            serializer.Formatting = formatado ? Formatting.Indented : Formatting.None;

            var lista = new List<IIdName>();

            foreach (var item in Enum.GetValues(enumerador))
            {
                lista.Add(((Enum)item).ToIdName());
            }

            if (orderById)
                lista = lista.OrderBy(t => t.Id).ToList();
            else
                lista = lista.OrderBy(t => t.Name).ToList();

            serializer.Serialize(writer, lista);

            return sb.ToString();
        }
        public static string SerializarEnumChar<T>(bool formatado, bool orderByCodigo = false) where T : struct
        {
            return SerializarEnumChar(typeof(T), formatado, orderByCodigo);
        }


        public static string SerializarEnumChar(Type enumerador, bool formatado, bool orderByCodigo = false)
        {
            var serializer = new JsonSerializer();
            serializer.ContractResolver = new StaticPropertyContractResolver();
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            serializer.Formatting = formatado ? Formatting.Indented : Formatting.None;

            var lista = new List<CodeDescription>();

            foreach (var item in Enum.GetValues(enumerador))
            {
                lista.Add(((Enum)item).ToCodeDescription());
            }

            if (orderByCodigo)
                lista = lista.OrderBy(t => t.Code).ToList();
            else
                lista = lista.OrderBy(t => t.Description).ToList();

            serializer.Serialize(writer, lista);

            return sb.ToString();
        }
    }
}
