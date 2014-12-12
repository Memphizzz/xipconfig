using System;
using System.Linq;

namespace X_ToolZ.Helpers
{
    static class OutputHelper
    {
        public static int LeftLen = 25;
        public static string Seperator = " : ";

        public static string GetString(string left, string right)
        {
            string retval = "   " + left;
            int leftCount = left.ToCharArray().Count();
            if (leftCount < LeftLen)
            {
                int leftOver = LeftLen - leftCount;
                retval += GetSpaces(leftOver, true);
            }
            else
                retval = left;
            
            if (!string.IsNullOrWhiteSpace(right) )
            {
                right = right.TrimEnd('\r', '\n');
                if (right.Contains(Environment.NewLine))
                {
                    int i = retval.ToCharArray().Count() + Seperator.ToCharArray().Count();
                    int index = right.IndexOf(Environment.NewLine);
                    right = right.Insert(index + Environment.NewLine.Length, GetSpaces(i - 1, false));
                }
            }

            return retval + Seperator + right;
        }

        private static string GetSpaces(int count, bool doted)
        {
            string retval = string.Empty;
            for (int i = count; i >= 0; i--)
            {
                if (doted)
                {
                    if (IsEven(i))
                        retval += ".";
                    else
                        retval += " ";
                }
                else
                    retval += " ";
            }

            return retval;
        }

        private static bool IsEven(int i)
        {
            return i % 2 == 0;
        }
    }
}