using System;
using System.Collections.Generic;
using System.Text;

namespace AspnetCoreExtensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendIf(this StringBuilder sb, bool condition, string value)
        {
            if (condition)
                sb.Append(value);

            return sb;
        }

        public static StringBuilder InsertIf(this StringBuilder sb, bool condition, int index, string value)
        {
            if (condition)
                sb.Insert(index, value);

            return sb;
        }
    }
}
