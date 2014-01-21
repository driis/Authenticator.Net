using System;
using System.Threading;
using Authenticator.Core;

namespace Authenticator.Net.ConsoleClient
{
    internal class ConsolePasswordDisplay : IDisposable
    {
        private static readonly DateTime TimeZero = new DateTime(1970, 1, 1, 0, 0, 0);
        private readonly TimeBasedOneTimePasswordGenerator _generator;
        private Timer _timer;

        public ConsolePasswordDisplay(TimeBasedOneTimePasswordGenerator generator)
        {
            _generator = generator;
        }

        public IDisposable Run()
        {
            TimeSpan timeSinceZero = DateTime.Now - TimeZero;
            long initialDelay = 30 - ((long) timeSinceZero.TotalSeconds%30);
            if (initialDelay == 0)
                initialDelay = 30;
            DisplayPassword(null);
            _timer = new Timer(DisplayPassword, null, TimeSpan.FromSeconds(initialDelay),TimeSpan.FromSeconds(30));
            return this;
        }

        private void DisplayPassword(object state)
        {
            Console.WriteLine("{0} :\t{1}", DateTime.Now.ToString("HH:mm:ss"), _generator.NextPassword());
        }

        public void Dispose()
        {            
            _timer.Dispose();
        }
    }
}