https://www.codewars.com/kata/52a89c2ea8ddc5547a000863



public class Kata
{
  public static int getLoopSize(
    LoopDetector.Node startNode)
  {
    var loopSize = 0;
    var (slowPointer, fastPointer) =
        (startNode, startNode);

    while ((slowPointer = slowPointer.next) != null &&
           (fastPointer = fastPointer.next.next)?.next != null)
    {
      if (slowPointer != fastPointer)
        continue;

      do
      {
        ++loopSize;
      } while ((slowPointer = slowPointer.next) != fastPointer);

      break;
    }

    return loopSize;
  }
}