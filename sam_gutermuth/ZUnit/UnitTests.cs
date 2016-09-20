using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ZUnit
{
    public static class UnitTests
    {
        private const char
            Pass = '+',
            Fail = '-';

        public static void Run<T>(bool showPassed = false)
        {
            var methods =
                typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(method => !method.Name.Equals("Run"))
                    .ToList();

            var paas = new List<string>();
            var fail = new List<string>();

            foreach (var method in methods)
            {
                var message = (string) method.Invoke(typeof(T), null);
                if (message[0].Equals(Pass)) paas.Add(message);
                else fail.Add(message);
            }

            Console.WriteLine("\r\n" + typeof(T).Name.Replace("_", " ") + ": Passed " + paas.Count + " of " + (paas.Count + fail.Count));
            Console.WriteLine("_______________________________________________________________________");
            fail.ForEach(Console.WriteLine);
            if (!showPassed) return;
            Console.WriteLine("_______________________________________________________________________");
            paas.ForEach(Console.WriteLine);
        }

        public static string Compare(dynamic expected, dynamic actual, [CallerMemberName] string name = "")
        {
            name = name.Replace("_", " ");
            if (expected.Equals(actual)) return Pass + " || " + name;
            return Fail + " || " + name + " expected to get " + expected + " but got " + actual;
        }

        public static string Compare(dynamic[] values, [CallerMemberName] string name = "")
        {
            if (values.Length % 2 != 0) return name + ": Missing Parameters";
            name = name.Replace("_", " ");
            for (var index = 0; index < values.Length - 1; index += 2)
            {
                if (!values[index].Equals(values[index + 1]))
                    return Fail + " || " + name + " expected to get " + values[index] + " but got " + values[index + 1];
            }
            return Pass + " || " + name;
        }
    }
}