https://www.codewars.com/kata/55b42574ff091733d900002f



using System;
using System.Collections.Generic;
using System.Linq;

public static class Kata
{
	public static IEnumerable<string> FriendOrFoe(
		string[] names)
	{
		return names.Where(
			name => name.Length == 4);
	}
}