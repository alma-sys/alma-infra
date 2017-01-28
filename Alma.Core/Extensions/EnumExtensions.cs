﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Alma.Core.Dto;

namespace Alma.Core
{
    public static class EnumExtensions
    {
        public static IEnumerable<CodigoNome> ToCodigoNomeFlags(this Enum enumeration)
        {
            var valores = enumeration.GetFlags();
            foreach (var item in valores)
            {
                yield return item.ToCodigoNome();
            }
        }

        [Obsolete("Usar ToIdNome")]
        public static CodigoNome ToCodigoNome(this Enum enumeration)
        {
            return new CodigoNome(

                codigo: enumeration.ToInt(),
                nome: enumeration.ToDescription()
            );
        }

        public static IIdNome ToIdNome(this Enum enumeration)
        {
            return new IdNome(
                id: enumeration.ToInt(),
                nome: enumeration.ToDescription()
            );
        }

        public static CodigoDescricao ToCodigoDescricao(this Enum enumeration)
        {
            return new CodigoDescricao(enumeration.ToString(), enumeration.ToDescription());
        }

        [Obsolete("Usar ToCodigoDescricao")]
        public static dynamic ToCodigoCharNome(this Enum enumeration)
        {
            return new
            {
                Codigo = enumeration.ToString(),
                Nome = enumeration.ToDescription()
            };
        }
        public static string ToDescription(this Enum enumeration, string valueWhenEmpty)
        {
            if (enumeration == null)
            {
                return valueWhenEmpty;
            }

            var attributes = (DescriptionAttribute[])enumeration
                .GetType()
                .GetField(enumeration.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            return (attributes.Length > 0)
                ? attributes[0].Description
                : enumeration.ToString();
        }

        public static string ToDescription(this Enum val)
        {
            return val.ToDescription(string.Empty);
        }

        public static int ToInt(this Enum val)
        {
            return Convert.ToInt32(val);
        }

        public static char ToChar(this Enum val)
        {
            return Convert.ToChar(val);
        }

        public static int[] ToArray(this Enum val)
        {
            var lista = new List<int>();
            var flags = val.GetFlags();

            foreach (var item in flags)
            {
                lista.Add(item.ToInt());
            }

            return lista.ToArray();
        }

        public static string AttributeToString<T>(this Enum enumeration)
        {
            Type type = enumeration.GetType();
            MemberInfo[] memInfo = type.GetMember(enumeration.ToString());

            if (null != memInfo && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(T), false);

                if (null != attrs && attrs.Length > 0) return attrs.ToString();
            }

            return string.Empty;
        }

        public static IList<Enum> GetValues(Type enumType)
        {
            return GetValues(enumType, null);
        }

        public static IList<Enum> GetValues(Type enumType, Func<Enum, bool> expression)
        {
            var enumValues = Enum.GetValues(enumType).Cast<Enum>();

            if (expression != null)
            {

                enumValues = enumValues.Where(expression);
            }

            return enumValues.ToList();
        }

        private const string ArgumentEnumException = "T isn't an enumerable type";
        public static IList<T> EnumToList<T>()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(ArgumentEnumException);

            IList<T> list = new List<T>();
            Type type = typeof(T);

            if (type != null)
            {
                Array enumValues = Enum.GetValues(type);
                foreach (T value in enumValues)
                {
                    list.Add(value);
                }
            }

            return list;
        }

        public static IDictionary<int, string> GetDictionaryItens(Type enumType)
        {
            var lista = (from Enum e in Enum.GetValues(enumType)
                         select new { Id = Convert.ToInt32(e), Name = e.ToDescription() }).ToDictionary(k => k.Id, v => v.Name);

            return lista;
        }

        public static T GetEnumByDescription<T>(String description)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(ArgumentEnumException);

            IList<T> list = EnumToList<T>();

            foreach (T item in list)
            {
                if (((Enum)Enum.Parse(typeof(T),
                       item.ToString())).ToDescription() == description)
                    return item;
            }

            throw new Exception("The description is invalid");
        }

        // Flags
        public static IEnumerable<Enum> GetFlags(this Enum value)
        {
            return GetFlags(value, Enum.GetValues(value.GetType()).Cast<Enum>().ToArray());
        }

        public static IEnumerable<Enum> GetIndividualFlags(this Enum value)
        {
            return GetFlags(value, GetFlagValues(value.GetType()).ToArray());
        }

        private static IEnumerable<Enum> GetFlags(Enum value, Enum[] values)
        {
            ulong bits = Convert.ToUInt64(value);
            List<Enum> results = new List<Enum>();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                ulong mask = Convert.ToUInt64(values[i]);
                if (i == 0 && mask == 0L)
                    break;
                if ((bits & mask) == mask)
                {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }
            if (bits != 0L)
                return Enumerable.Empty<Enum>();
            if (Convert.ToUInt64(value) != 0L)
                return results.Reverse<Enum>();
            if (bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L)
                return values.Take(1);
            return Enumerable.Empty<Enum>();
        }

        private static IEnumerable<Enum> GetFlagValues(Type enumType)
        {
            ulong flag = 0x1;
            foreach (var value in Enum.GetValues(enumType).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                if (bits == 0L)
                    //yield return value;
                    continue; // skip the zero value
                while (flag < bits) flag <<= 1;
                if (flag == bits)
                    yield return value;
            }
        }

        public static bool Has<T>(this System.Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }

        public static bool Is<T>(this System.Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }


        public static T Add<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), nameof(type), ex);
            }
        }


        public static T Remove<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), nameof(type), ex);
            }
        }
    }
}
