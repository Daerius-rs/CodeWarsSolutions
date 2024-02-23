https://www.codewars.com/kata/5412509bd436bd33920011bc



public static class Kata
{
	public static string Maskify(string value)
	{
		return value.Length > 4
			? value
				.Substring(value.Length - 4, 4)
				.PadLeft(value.Length, '#')
			: value;
	}
}