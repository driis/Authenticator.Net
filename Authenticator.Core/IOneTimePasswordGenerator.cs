namespace Authenticator.Core
{
    public interface IOneTimePasswordGenerator
    {
        string NextPassword();
    }
}
