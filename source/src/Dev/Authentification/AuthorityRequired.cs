using System;

namespace Testflow.Authentication
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
    public class AuthorityRequired : Attribute
    {
        public AuthorityRequired(Enum test)
        {
        }
    }
}