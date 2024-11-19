using System;

namespace Connexien.Lib.Extensions
{
    public static class Guard
    {
        public static void AgainstNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}
