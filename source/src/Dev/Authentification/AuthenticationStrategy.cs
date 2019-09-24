using System;
using System.Collections.Generic;

namespace TestStation.Authentication
{
    public class AuthenticationStrategy
    {
        private readonly Dictionary<string, Dictionary<string, object>> _authorityData;
        internal AuthenticationStrategy()
        {
            this._authorityData = new Dictionary<string, Dictionary<string, object>>(5);
        }

        public void AddUserLevel(string userLevel)
        {

        }

        public void AddUserAuthority(string userLevel, string authorityName, bool value = true)
        {

        }

        public void AddUserAuthority(string userLevel, string authorityName, object value)
        {

        }

        public TDataType GetAuthorityInfo<TDataType>(string userLevel, string authorityName)
        {
            throw new NotImplementedException();
        }
    }
}