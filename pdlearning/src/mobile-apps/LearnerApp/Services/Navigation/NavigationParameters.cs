using System.Collections.Generic;

namespace LearnerApp.Services.Navigation
{
    public class NavigationParameters
    {
        private readonly Dictionary<string, object> _navigationParameters;

        public NavigationParameters()
        {
            _navigationParameters = new Dictionary<string, object>();
        }

        public TValueType GetParameter<TValueType>(string key)
        {
            if (_navigationParameters.ContainsKey(key))
            {
                return (TValueType)_navigationParameters[key];
            }

            return default;
        }

        public void SetParameter(string key, object value)
        {
            _navigationParameters.Add(key, value);
        }
    }
}
