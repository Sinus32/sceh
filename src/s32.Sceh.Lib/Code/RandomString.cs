using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Code
{
    public class RandomString
    {
        public static readonly RandomString Instance = new RandomString();
        private const string CHARS = "0123456789abcdefghijklmnopqrstuvwxyz";
        private Random _random;

        public RandomString()
        {
            _random = new Random();
        }

        public static string Generate(int length)
        {
            lock (Instance)
                return Instance.GenerateString(length);
        }

        public string GenerateString(int length)
        {
            var result = new char[length];
            for (int i = 0; i < length; ++i)
                result[i] = CHARS[_random.Next(CHARS.Length)];
            return new String(result);
        }
    }
}