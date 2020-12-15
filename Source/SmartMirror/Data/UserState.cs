using System;
using System.Collections.Generic;

namespace SmartMirror.Data
{
    public class UserState : Displayable
    {
        private readonly Dictionary<string, Dictionary<string, object>> UserStateDictionary = new Dictionary<string, Dictionary<string, object>>();

        public void InitNewUserState<T>()
        {
            UserStateDictionary.Add(typeof(T).FullName, new Dictionary<string, object>());
        }

        public void SetUserState<T>(string user, object value)
        {
            if (!UserStateDictionary.ContainsKey(typeof(T).FullName))
            {
                throw new InvalidOperationException("Initalize state first");
            }

            Dictionary<string, object> currentStateDictionary = UserStateDictionary[typeof(T).FullName];
            SetDictionaryData(user, currentStateDictionary, value);
        }

        public T GetUserState<T>(string user)
        {
            if (!UserStateDictionary.ContainsKey(typeof(T).FullName))
            {
                throw new InvalidOperationException("Purpose not found");
            }

            Dictionary<string, object> currentStateDictionary = UserStateDictionary[typeof(T).FullName];

            if (!currentStateDictionary.ContainsKey(user))
            {
                return default(T);
            }

            return (T) currentStateDictionary[user];
        }

        private void SetDictionaryData<T>(string user, Dictionary<string, T> dictionary, T newValue)
        {
            if (dictionary.ContainsKey(user))
            {
                dictionary[user] = newValue;
            }
            else
            {
                dictionary.Add(user, newValue);
            }
        }
    }
}
