using System;
using System.Collections.Generic;

namespace SmartMirror.Data
{
    public class UserState : Displayable
    {
        private readonly Dictionary<string, Dictionary<string, object>> _userStateDictionary = new();

        public void InitNewUserState<T>()
        {
            _userStateDictionary.Add(typeof(T).FullName, new Dictionary<string, object>());
        }

        public void SetUserState<T>(string user, object value)
        {
            if (!_userStateDictionary.ContainsKey(typeof(T).FullName))
            {
                throw new InvalidOperationException("Initalize state first");
            }

            Dictionary<string, object> currentStateDictionary = _userStateDictionary[typeof(T).FullName];
            SetDictionaryData(user, currentStateDictionary, value);
        }

        public T GetUserState<T>(string user)
        {
            if (!_userStateDictionary.ContainsKey(typeof(T).FullName))
            {
                throw new InvalidOperationException("Purpose not found");
            }

            Dictionary<string, object> currentStateDictionary = _userStateDictionary[typeof(T).FullName];

            if (!currentStateDictionary.ContainsKey(user))
            {
                return default;
            }

            return (T) currentStateDictionary[user];
        }

        private static void SetDictionaryData<T>(string user, Dictionary<string, T> dictionary, T newValue)
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
