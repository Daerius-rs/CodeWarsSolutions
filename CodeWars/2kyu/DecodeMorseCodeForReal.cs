// https://www.codewars.com/kata/54acd76f7207c6a2880012bb



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class MorseCodeDecoder
{
  private class Cluster
  {
    public double Center { get; set; }
    public int Weight { get; set; }
    public double N { get; set; }
    public double X { get; set; }



    public Cluster(double center,
      int weight, double n, double x)
    {
      Center = center;
      Weight = weight;
      N = n;
      X = x;
    }
  }



  public static string decodeBitsAdvanced(
    string bits)
  {
    bits = bits.Trim('0');

    if (string.IsNullOrEmpty(bits))
      return string.Empty;

    var tokens = (from match in Regex.Matches(bits, @"1+|0+")
      select match.Value).ToArray();
    var lengths = (from token in tokens
      orderby token.Length select token.Length).ToList();
    var minLength = lengths[0];
    var maxLength = lengths[^1];
    var lengthsDifference = maxLength - minLength;
    var clusters = (from x in new[] { 1, 3, 7 }
      select new Cluster(minLength + lengthsDifference * x / 8.0,
        0, maxLength, minLength)).ToList();

    foreach (var length in lengths)
    {
      var minCluster = (from cluster in clusters
        orderby Math.Abs(length - cluster.Center)
        select cluster).First();

      minCluster.Center =
        (minCluster.Center * minCluster.Weight + length) / (minCluster.Weight + 1);
      minCluster.Weight += 1;

      if (length < minCluster.N)
        minCluster.N = length;
      if (length > minCluster.X)
        minCluster.X = length;
    }

    clusters = (from cluster in clusters
          where cluster.Weight != 0
          select cluster).ToList();

    if (clusters.Count == 2)
    {
      if (clusters[1].N / clusters[0].X >= 5)
      {
        clusters.Insert(1,
          new Cluster((clusters[0].X + clusters[1].N) / 2.0,
            0, clusters[0].X + 1, clusters[1].N - 1));
      }
    }
    if (clusters.Count < 3)
    {
      var limit = clusters[^1].X + 1;

      clusters.AddRange(from i in Enumerable.Range(0, 3 - clusters.Count)
        select new Cluster(limit,
          0, limit, limit));
    }

    var maxDotLength =
      (clusters[0].X + clusters[1].N) / 2.0;
    var maxDashLength =
      (clusters[1].X + clusters[2].N) / 2.0;
    var result = new List<string>();
    
    foreach (var token in tokens)
    {
      if (token[0] == '1')
      {
        result.Add(token.Length <= maxDotLength
          ? "."
          : "-");
      }
      else if (token.Length > maxDotLength)
      {
        result.Add(token.Length <= maxDashLength
          ? " "
          : "   ");
      }
    }

    return string.Concat(result);
  }

  public static string decodeMorse(
    string morseCode)
  {
    morseCode = morseCode.Trim();
    
    if (string.IsNullOrEmpty(morseCode))
      return string.Empty;
    
    return string.Join(" ", from word in morseCode.Split("   ")
      select string.Concat(from letter in word.Split(" ")
        select Preloaded.MORSE_CODE[letter]));
  }
}