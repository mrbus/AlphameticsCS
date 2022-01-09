using System;
using System.Collections.Generic;
using System.Linq;
using Combinatorics.Collections;

namespace AlphameticsCS
{
    class Program
    {
        static void Main()
        {
            try
            {
                do_work();
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void do_work()
        {
            Console.Write("Input the formula: ");
            var formula = Console.ReadLine().ToUpper().Replace(" ", "");
            var start_time = DateTime.Now;
            var left_right = formula.Split('=');
            check_error(left_right.Length == 2, "Error: formula must contain exactly one '=' sign");
            var left = left_right[0];
            var args = left.Split('+');
            check_error(args.Length <= 2, "Error: formula contains more than one '+' sign");
            char sign;
            if (args.Length == 1)
            {
                args = left.Split('-');
                check_error(args.Length <= 2, "Error: formula contains more than one '-' sign");
                check_error(args.Length == 2, "Error: formula contains neither '+' nor '-' sign");
                sign = '-';
            }
            else
                sign = '+';
            var result = left_right[1];
            check_error(args[0].All(c => char.IsLetter(c))
                && args[1].All(char.IsLetter)
                && result.All(char.IsLetter),
                "Error: arguments and result must contain letters only");
            var all_letters = 
                (from c in formula
                where char.IsLetter(c)
                select c).Distinct().ToArray();
            // Другая версия:
            // var all_letters = formula.Where(char.IsLetter).Distinct().ToArray();
            check_error(all_letters.Length <= 10, "Error: formula must contain less than or equal to 10 different letters");
            var arg0n = args[0].Select(c => Array.IndexOf(all_letters, c)).ToArray();
            var arg1n = args[1].Select(c => Array.IndexOf(all_letters, c)).ToArray();
            var resultn = result.Select(c => Array.IndexOf(all_letters, c)).ToArray();
            var digits = Enumerable.Range(0, 10);
            //var perm = new Permutations<int>(digits);
            var perms = new Variations<int>(digits, all_letters.Length);
            foreach (var perm in perms)
            {
                if (perm[arg0n[0]] == 0 || perm[arg1n[0]] == 0 || perm[resultn[0]] == 0)
                    continue;
                var arg0 = get_number(arg0n, perm);
                var arg1 = get_number(arg1n, perm);
                var res = get_number(resultn, perm);
                bool is_correct =
                    sign == '+' ? arg0 + arg1 == res : arg0 - arg1 == res;
                if (is_correct)
                    Console.WriteLine($"{arg0}{sign}{arg1}={res}");
            }
            Console.WriteLine($"Elapsed time: {DateTime.Now - start_time}");
            //Console.ReadKey();
        }

        static void check_error(bool v, string msg)
        {
            if (!v)
                throw new ApplicationException(msg);
        }

        static int get_number(int[] arg, IReadOnlyList<int> perm)
        {
            int result = 0;
            int ten_power = 1;
            foreach(var v in arg.Reverse())
            {
                result += perm[v] * ten_power;
                ten_power *= 10;
            }
            return result;
        }
    }
}
