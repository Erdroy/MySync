// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.Text.RegularExpressions;

namespace MySync.Client.Utilities
{
    public static class StringUtils
    {
        public static int ExtractNumber(string text)
        {
            var match = Regex.Match(text, @"(\d+)");
            int value;
            return !int.TryParse(match.Value, out value) ? 0 : value;
        }
    }
}
