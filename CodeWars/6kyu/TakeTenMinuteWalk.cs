// https://www.codewars.com/kata/54da539698b8a2ad76000228



public class Kata
{
  public ref struct Point
  {
    public int X;
    public int Y;
    
    public Point(
      int x, int y)
    {
      X = x;
      Y = y;
    }
    
    public bool IsOrigin()
    {
      return X == 0 &&
             Y == 0;
    }
  }
  
  public static bool IsValidWalk(
    string[] route)
  {
    if (route.Length != 10)
      return false;
    if (route.Length % 2 != 0)
      return false;

    var endPoint = new Point();

    foreach (var direction in route)
    {
      endPoint.X += direction switch
      {
        "n" => 1,
        "s" => -1,
         _  => 0
      };
      endPoint.Y += direction switch
      {
        "w" => 1,
        "e" => -1,
         _  => 0
      };
    }

    return endPoint.IsOrigin();
  }
}