// https://www.codewars.com/kata/563fbac924106b8bf7000046



using System;
using System.Linq;

public class Kata
{
    private static readonly string[] ExcludedWords =
    {
        "THE",
        "OF",
        "IN",
        "FROM",
        "BY",
        "WITH",
        "AND",
        "OR",
        "FOR",
        "TO",
        "AT",
        "A"
    };

    public static string GenerateBC(
        string url, string separator)
    {
        var urlParts = new Uri(
                new UriBuilder(url).ToString())
            .Segments
            .Skip(1)
            .Where(x => !x.Contains("index."))
            .ToArray();

        return string.Join(
            separator,
            urlParts
                .Select(part => part
                    .Split('.')[0]
                    .TrimEnd('/')
                    .ToUpper())
                .Prepend("HOME")
                .Select((part, index) => new
                {
                    href = string.Concat(urlParts.Take(index)),
                    text = part.Length > 30
                        ? string.Concat(part
                            .Split('-')
                            .Where(word => !ExcludedWords.Contains(word))
                            .Select(word => word[0]))
                        : part.Replace('-', ' ')
                }).Select((x, i) =>
                    i < urlParts.Length
                        ? $"<a href=\"/{x.href}\">{x.text}</a>"
                        : $"<span class=\"active\">{x.text}</span>"));
    }
}