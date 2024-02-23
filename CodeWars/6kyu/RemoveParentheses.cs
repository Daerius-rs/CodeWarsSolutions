// https://www.codewars.com/kata/5f7c38eb54307c002a2b8cc8



namespace Solution
{
    public static class Kata
    {
        public static string RemoveParentheses(
            string source)
        {
            while (true)
            {
                var openingBraceIndex = -1;

                openingBraceIndex = source.IndexOf(
                    '(', openingBraceIndex + 1);

                if (openingBraceIndex == -1)
                    break;

                var closingBraceIndex = -1;
                var parenthesesLevel = 0;

                do
                {
                    closingBraceIndex = source.IndexOfAny(
                        new[] { '(', ')' }, closingBraceIndex + 1);

                    if (closingBraceIndex == -1)
                        break;

                    parenthesesLevel += source[closingBraceIndex] switch
                    {
                        '(' => 1,
                        ')' => -1,
                        _ => 0
                    };
                } while (parenthesesLevel != 0);

                source = source.Remove(openingBraceIndex,
                    closingBraceIndex - openingBraceIndex + 1);
            }

            return source;
        }
    }
}