https://www.codewars.com/kata/5266876b8f4bf2da9b000362



public static class Kata
{
  public static string Likes(
    string[] names)
  {
    return $"{NamesToString(names)} like{(names.Length < 2 ? "s" : string.Empty)} this";
  }

  public static string NamesToString(
    string[] names)
  {
    return names.Length switch
    {
      0 => "no one",
      1 => names[0],
      2 => $"{names[0]} and {names[1]}",
      _ => $"{names[0]}, {names[1]} and {(names.Length == 3 ? names[2] : $"{names.Length - 2} others")}"
    };
  }
}