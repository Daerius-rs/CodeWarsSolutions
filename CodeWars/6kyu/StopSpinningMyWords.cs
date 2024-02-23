// https://www.codewars.com/kata/5264d2b162488dc400000001



using System;
using System.Collections.Generic;
using System.Linq;

public class Kata
{
	public static string SpinWords(
		string sentence)
	{
		return string.Join(' ',
			sentence.Split(' ')
			.Select(word => word.Length >= 5
				? string.Concat(word.Reverse())
				: word));
	}
}