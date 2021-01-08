using System;

namespace LearnerApp.Services.Navigation
{
    /// <summary>
    /// In shell navigation system, only simple data is allowed to be passed, because of our current implementation
    /// we will need to workaround by base on a singleton for passing data, but this need to be changed if later we have push notification
    /// because we are only able to retrieve simple data from push notification.
    /// </summary>
    public static class NavigationParameterManager
    {
        private static string _routeName;
        private static NavigationParameters _currentParameters = null;

        public static void SetTransferParameter(string routeName, NavigationParameters parameters)
        {
            #if DEBUG
            if (_currentParameters != null)
            {
                throw new Exception("Parameter is set before but never used, this should not happen");
            }
            #endif

            _routeName = routeName;
            _currentParameters = parameters;
        }

        public static NavigationParameters RetrieveTransferParameter(string routeName)
        {
            if (_currentParameters == null)
            {
                return null;
            }

            if (_routeName != routeName)
            {
                var indexOf = routeName.IndexOf(_routeName, StringComparison.Ordinal);
                if (indexOf == -1)
                {
                    return null;
                }

                // If it's not a relative path
                if (indexOf + _routeName.Length != routeName.Length)
                {
                    return null;
                }
            }

            var currentParameter = _currentParameters;
            _currentParameters = null;

            return currentParameter;
        }
    }
}
