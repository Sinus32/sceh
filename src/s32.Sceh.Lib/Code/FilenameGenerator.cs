using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Code
{
    public static class FilenameGenerator
    {
        private const string CHARS = "0123456789abcdefghijklmnopqrstuvwxyz";

        public static string Generate(string input, int length)
        {
            var vals = new int[length];
            for (int i = 0; i < input.Length; ++i)
                vals[i % length] += (char)input[i];

            var result = new char[length];
            for (int i = 0; i < length; ++i)
                result[i] = CHARS[vals[i] % CHARS.Length];

            return new String(result);
        }
    }
}
