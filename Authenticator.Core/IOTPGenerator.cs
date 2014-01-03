namespace Authenticator.Core
{
    public interface IOTPGenerator
    {
        string NextPassword();
    }

    public class CounterBasedOTPGenerator : IOTPGenerator
    {
        public string NextPassword()
        {
            throw new System.NotImplementedException();
        }
    }
}
