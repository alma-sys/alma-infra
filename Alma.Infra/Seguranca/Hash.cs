﻿using System;
using System.Security.Cryptography;

namespace Alma.Infra.Seguranca
{
    public class Hash
    {
        public static String Gerar(String input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = SHA512.Create())
            {
                var result = hash.ComputeHash(inputBytes);
                return BitConverter.ToString(result).Replace("-", "");
            }
        }
    }
}
