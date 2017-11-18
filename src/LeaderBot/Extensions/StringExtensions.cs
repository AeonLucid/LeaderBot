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
    }
}