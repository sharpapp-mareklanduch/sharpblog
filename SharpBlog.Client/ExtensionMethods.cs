using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpBlog.Client
{
    public static class ExtensionMethods
    {
        public static string StripHTML(this string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static string Excerpt(this string input, int wordsNumber) 
        {
            var splitted = input.Split();
            bool addEllipsis = splitted.Length > wordsNumber;

            var result = string.Join(" ", splitted.Take(wordsNumber));
            if(addEllipsis)
            {
                result = $"{result}...";
            }
            return result;
        }
    }
}
