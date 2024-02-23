// https://www.codewars.com/kata/52fba66badcd10859f00097e



using System;
using System.Text.RegularExpressions;

public static class Kata
{
	public static string Disemvowel(
		string value)
	{
		return Regex.Replace(
			value, "[aeiou]", "",
			RegexOptions.IgnoreCase);
	}
}