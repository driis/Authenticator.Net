using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Text;
using System.Threading;
using Authenticator.Core;

namespace Authenticator.Net.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayInfo();
            if (args.Length == 0)
            {
                DisplayUsage();
                return;
            }
            string secretArgument = args[0];
            byte[] secretBytes = Base32.DecodeFromString(secretArgument);
            secretBytes = secretBytes.Concat(secretBytes).ToArray();
            var generator = new TimeBasedOneTimePasswordGenerator(secretBytes, 6, () => DateTime.Now);

            using (new ConsolePasswordDisplay(generator).Run())
            {
                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Escape)
                            break;
                    }
                    Thread.Sleep(500);
                }    
            }
        }

        private static void DisplayUsage()
        {
            Console.WriteLine("This program displays one-time passwords generated using the time-based\n" +
                              "algorithm outlined in RFC 6238.\n\n");
            Console.WriteLine("Usage:\n\n\tAuthenticator.Net.ConsoleClient.exe <base32secret>\n\n");
        }

        private static void DisplayInfo()
        {
            Console.WriteLine("Authenticator .NET (c) Dennis Riis 2014\n");
        }
    }
}
