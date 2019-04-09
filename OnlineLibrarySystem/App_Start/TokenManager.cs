using System.Collections.Generic;
using System.Linq;

namespace OnlineLibrarySystem
{
    public class TokenManager
    {
        private static volatile TokensHolder tokenDictionary;
        private static readonly object _syncRoot = new object();

        public static TokensHolder TokenDictionaryHolder
        {
            get
            {
                if (tokenDictionary == null)
                {
                    lock (_syncRoot)
                    {
                        if (tokenDictionary == null)
                        {
                            tokenDictionary = new TokensHolder();
                        }
                    }
                }
                return tokenDictionary;
            }
        }
    }

    public class TokensHolder : Dictionary<string, int>
    {
        /// <summary>
        /// get the user id that belongs to this token
        /// </summary>
        /// <param name="key">user id</param>
        /// <returns></returns>
        public new int this[string key]
        {
            get
            {
                if (!TryGetValue(key, out int retValue))
                {
                    retValue = -1;
                }
                return retValue;
            }
        }

        /// <summary>
        /// saves a token to be used used in session data or cookies for authentication
        /// </summary>
        /// <param name="token">generated token</param>
        /// <param name="userId">token for the user having this user id</param>
        /// <returns>true if the token was added successfully and false if the token already exists</returns>
        public string AddToken(string token, int userId)
        {
            KeyValuePair<string, int> retValue = this.FirstOrDefault(t => t.Value == userId);
            if (string.IsNullOrEmpty(retValue.Key))
            {
                Add(token, userId);
                return token;
            }
            return retValue.Key;
        }
    }
}