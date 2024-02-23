https://www.codewars.com/kata/550f22f4d758534c1100025a



using System;
using System.Collections.Generic;
using System.Linq;

public class DirReduction
{
    private static readonly Dictionary<string, string> SidesMap;

    static DirReduction()
    {
        SidesMap = new Dictionary<string, string>
        {
            { "NORTH", "SOUTH" },
            { "SOUTH", "NORTH" },
            { "EAST", "WEST" },
            { "WEST", "EAST" }
        };
    }

    public static string[] dirReduc(string[] source)
    {
        var result = source.ToList();
        var index = 0;

        while (index < result.Count - 1)
        {
            if (index >= 0
                && SidesMap.TryGetValue(result[index], out var sideMapped)
                && result[index + 1] == sideMapped)
            {
                result.RemoveRange(
                    index--, 2);

                continue;
            }

            ++index;
        }

        return result.ToArray();
    }
}