using System;

namespace Authenticator.Core
{
    public class TimeBasedOneTimePasswordGenerator : IOneTimePasswordGenerator
    {
        private readonly Func<DateTime> _currentTime;
        private readonly DateTime _timeZero;
        private readonly int _stepIntervalSeconds;
        private readonly CounterBasedOneTimePasswordGenerator _inner;
        public TimeBasedOneTimePasswordGenerator(byte[] secret, int passwordLength, Func<DateTime> currentTime) : this(secret, passwordLength, currentTime, new DateTime(1970,1,1,0,0,0), 30)
        {            
        }

        private TimeBasedOneTimePasswordGenerator(byte[] secret, int passwordLength, Func<DateTime> currentTime, DateTime timeZero, int stepIntervalSeconds)
        {
            _currentTime = currentTime;
            _timeZero = timeZero;
            _stepIntervalSeconds = stepIntervalSeconds;
            _inner = new CounterBasedOneTimePasswordGenerator(secret,passwordLength,CurrentTimeAsCounterValue);
        }

        private long CurrentTimeAsCounterValue()
        {
            DateTime instant = _currentTime();
            var timeSinceZero = instant - _timeZero;
            var secondsSinceZero = (long) timeSinceZero.TotalSeconds;
            return secondsSinceZero/_stepIntervalSeconds;
        }

        public string NextPassword()
        {
            return _inner.NextPassword();
        }
    }
}