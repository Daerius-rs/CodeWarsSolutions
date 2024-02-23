// https://www.codewars.com/kata/52685f7382004e774f0001f7



public static class TimeFormat
{
  public static string GetReadableTime(
    int seconds)
  {
    return $"{seconds / 3600:D2}:{seconds / 60 % 60:D2}:{seconds % 60:D2}";
  }
}