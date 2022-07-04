using System;
using System.Linq;

namespace backend.util
{
    public class common
    {
        public static string yieldDatabaseId(string prefix, string current)
        {
            int length = current.Length;
            string id = Convert.ToString(Int32.Parse(current) + 1);
            id = id.PadLeft(length, '0');
            return $"{prefix}{id}";
        }

        public static string DateFormat_full(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string DateFormat_simple(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string DateFormat_report(DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
         public static string RandomStringOnlyNumber(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}