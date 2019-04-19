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
                        if (tokenDictionary == null) tokenDictionary = new TokensHolder();
                    }
                }
                return tokenDictionary;
            }
        }
    }

    public class TokensHolder : Dictionary<string, int>
    {

        public new int this[string key]
        {
            get
            {
                if (!TryGetValue(key, out int retValue)) retValue = -1;
                return retValue;
            }
        }

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