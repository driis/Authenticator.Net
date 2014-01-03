using System;

namespace Authenticator.Core
{
    public static class HashExtensions
    {
        public static T[] Copy<T>(this T[] original)
        {
            if (original == null)
                throw new ArgumentNullException("original");
            
            var copy = new T[original.Length];
            original.CopyTo(copy,0);
            return copy;
        }   
    }
}
