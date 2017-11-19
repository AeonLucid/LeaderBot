using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace LeaderBot.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex PascalCaseRegex = new Regex("(?<=.)([A-Z])", RegexOptions.Compiled);

        public static string ToPascalCase(this string input)
        {
            return PascalCaseRegex.Replace(input, "_$0").ToLower();
        }

        public static string GetChecksum(this string input)
        {
            using (var hash = new SHA1CryptoServiceProvider())
            {
                var result = hash.ComputeHash(Encoding.ASCII.GetBytes(input));

                return BitConverter.ToString(result).Replace("-", "").ToLower();
            }
        }
    }
}