using System;

namespace Authenticator.Core
{
    public class TimeBasedOneTimePasswordGenerator : IOneTimePasswordGenerator
    {
        private readonly Func<DateTime> _currentTime;
        private readonly DateTime _timeZero;
        private readonly int _stepIntervalSeconds;
        private readonly CounterBasedOneTimePasswordGenerator _inner;

        public TimeBasedOneTimePasswordGenerator(byte[] secret, int passwordLength, Func<DateTime> currentTime, HashMode hashMode = HashMode.HMACSHA1) : this(secret, passwordLength, currentTime, new DateTime(1970,1,1,0,0,0), 30, hashMode)
        {            
        }

        private TimeBasedOneTimePasswordGenerator(byte[] secret, int passwordLength, Func<DateTime> currentTime, DateTime timeZero, int stepIntervalSeconds = 30, HashMode hashMode = HashMode.HMACSHA1)
        {
            _currentTime = currentTime;
            _timeZero = timeZero;
            _stepIntervalSeconds = stepIntervalSeconds;
            _inner = new CounterBasedOneTimePasswordGenerator(secret,passwordLength,CurrentTimeAsCounterValue, hashMode);
        }

        private long CurrentTimeAsCounterValue()
        {
            DateTime instant = _currentTime();
            TimeSpan timeSinceZero = instant - _timeZero;
            long secondsSinceZero = (long) timeSinceZero.TotalSeconds;
            return secondsSinceZero/_stepIntervalSeconds;
        }

        public string NextPassword()
        {
            return _inner.NextPassword();
        }
    }
}