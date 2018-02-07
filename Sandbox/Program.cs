using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sandbox
{
    class Program
    {
        static Dictionary<string, Action<string[]>> _commands = new Dictionary<string, Action<string[]>>();
        static bool exit;
        static Dictionary<string, Type> _showcases;

        static Program()
        {
            _commands.Add("exit", (args) => exit = true);
            _commands.Add("list", ListShowcases);
            _commands.Add("showcase", ShowcaseStart);

            _showcases = Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(t => typeof(IShowcase).IsAssignableFrom(t) && !t.IsInterface)
                .Aggregate(new Dictionary<string, Type>(), 
                    (d, itm) => { d.Add(itm.Name, itm); return d; });
        }

        static void Main(string[] args)
        {
            do
            {
                var input = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (!input.Any())
                    continue;
                var command = input.First();
                var comandArgs = input.Skip(1).ToArray();
                if (!_commands.TryGetValue(command, out Action<string[]> call))
                {
                    Console.WriteLine("Unknown command");
                    continue;
                }
                call(comandArgs);
                
            } while (!exit);
        }

        static void ListShowcases(params string[] args)
        {
            foreach (var sc in _showcases.Keys)
            {
                Console.WriteLine(sc);
            }
        }

        static void ShowcaseStart(params string[] args)
        {
            foreach (var sc in args)
            {
                if (!_showcases.TryGetValue(sc, out Type t))
                    Console.WriteLine($"Unknown showcase {sc}");

                var s = (IShowcase)Activator.CreateInstance(t);
                s.Start();
            }
        }
    }
}
