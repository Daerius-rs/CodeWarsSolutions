// https://www.codewars.com/kata/52bb6539a4cf1b12d90005b7



using System;
using System.Collections.Generic;
using System.Linq;

namespace Solution
{
  public class BattleshipField
  {
    private class BattleshipSet
    {
      public int Cell1 { get; set; }
      public int Cell2 { get; set; }
      public int Cell3 { get; set; }
      public int Cell4 { get; set; }

      public bool Validate()
      {
        return Cell1 == 4 && Cell2 == 3 && Cell3 == 2 && Cell4 == 1;
      }
    }



    private const int FieldSize = 10;



    private static IEnumerable<(int X, int Y)> GetNeighbors(
      ICollection<(int X, int Y)> shipPoints)
    {
      foreach (var point in shipPoints)
      {
        for (int i = point.X - 1; i <= point.X + 1; i++)
        {
          if (i < 0 || i >= FieldSize)
            continue;

          for (int j = point.Y - 1; j <= point.Y + 1; j++)
          {
            if (j < 0 || j >= FieldSize)
              continue;

            var neighborPoint = (i, j);

            if (!shipPoints.Contains(neighborPoint))
              yield return neighborPoint;
          }
        }
      }
    }

    private static bool VisitPoint(int x, int y, int[,] field,
      BattleshipSet ships, HashSet<(int X, int Y)> visitedPoints)
    {
      var point = (x, y);

      if (field[x, y] == 0)
        return true;
      if (visitedPoints.Contains(point))
        return true;

      var horizontalCount = 1;
      var verticalCount = 1;
      var shipPoints = new HashSet<(int X, int Y)>
      {
        point
      };

      for (int i = x;
        ++i < FieldSize && field[i, y] == 1;)
      {
        shipPoints.Add((i, y));

        ++horizontalCount;
      }
      for (int i = x;
        --i >= 0 && field[i, y] == 1;)
      {
        shipPoints.Add((i, y));

        ++horizontalCount;
      }

      for (int i = y;
        ++i < FieldSize && field[x, i] == 1;)
      {
        shipPoints.Add((x, i));

        ++verticalCount;
      }
      for (int i = y;
        --i >= 0 && field[x, i] == 1;)
      {
        shipPoints.Add((x, i));

        ++verticalCount;
      }

      if (horizontalCount > 1 && verticalCount > 1)
        return false;

      switch (Math.Max(horizontalCount, verticalCount))
      {
        case 1:
          ++ships.Cell1;
          break;
        case 2:
          ++ships.Cell2;
          break;
        case 3:
          ++ships.Cell3;
          break;
        case 4:
          ++ships.Cell4;
          break;
        default:
          return false;
      }

      foreach (var shipPoint in shipPoints)
      {
        visitedPoints.Add(shipPoint);
      }

      return GetNeighbors(shipPoints)
        .All(p => field[p.X, p.Y] == 0);
    }



    public static bool ValidateBattlefield(
      int[,] field)
    {
      var ships = new BattleshipSet();
      var visitedPoints = new HashSet<(int X, int Y)>();

      for (int i = 0; i < FieldSize; ++i)
      {
        for (int j = 0; j < FieldSize; ++j)
        {
          if (!VisitPoint(i, j, field, ships, visitedPoints))
            return false;
        }
      }

      return ships.Validate();
    }
  }
}