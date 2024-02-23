https://www.codewars.com/kata/541c8630095125aba6000c00



using System;
using System.Linq;

public class Number
{
	public int DigitalRoot(
		long n)
	{
		n = n.ToString()
			.Sum(ch => ch - '0');

		return n > 9L
			? DigitalRoot(n)
			: (int)n;
	}
}