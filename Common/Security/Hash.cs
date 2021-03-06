﻿using System;
using System.Security.Cryptography;

namespace Alma.Common.Security
{
    public static class Hash
    {
        public static string Create(string input)
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
