﻿using Alma.Common;
using Alma.Common.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Alma.ApiExtensions.Serializers
{
    public static class ConstantSerializer
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


        public static string Serialize(Type tipo, bool formatted)
        {
            var serializer = new JsonSerializer();
            serializer.ContractResolver = new StaticPropertyContractResolver();
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            serializer.Formatting = formatted ? Formatting.Indented : Formatting.None;
            serializer.Serialize(writer, Activator.CreateInstance(tipo, null), tipo);

            return sb.ToString();
        }

        public static string SerializeEnum<T>(bool formatted, bool orderById = false) where T : struct
        {
            return SerializeEnum(typeof(T), formatted, orderById);
        }

        public static string SerializeEnum(Type enumerador, bool formatted, bool orderById = false)
        {
            var serializer = new JsonSerializer();
            serializer.ContractResolver = new StaticPropertyContractResolver();
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            serializer.Formatting = formatted ? Formatting.Indented : Formatting.None;

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

        public static string SerializeEnumChar<T>(bool formatted, bool orderByCode = false) where T : struct
        {
            return SerializeEnumChar(typeof(T), formatted, orderByCode);
        }

        public static string SerializeEnumChar(Type enumerador, bool formatted, bool orderByCode = false)
        {
            var serializer = new JsonSerializer();
            serializer.ContractResolver = new StaticPropertyContractResolver();
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            serializer.Formatting = formatted ? Formatting.Indented : Formatting.None;

            var list = new List<CodeDescription>();

            foreach (var item in Enum.GetValues(enumerador).OfType<Enum>().OrderBy(x => x.ToString()))
            {
                list.Add(((Enum)item).ToCodeDescription());
            }

            if (orderByCode)
                list = list.OrderBy(t => t.Code).ToList();
            else
                list = list.OrderBy(t => t.Description).ToList();

            serializer.Serialize(writer, list);

            return sb.ToString();
        }
    }
}
