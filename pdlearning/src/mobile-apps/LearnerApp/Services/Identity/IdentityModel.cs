using System;
using LearnerApp.Models;

namespace LearnerApp.Services.Identity
{
    public class IdentityModel
    {
        public IdentityModel(string accessToken, string idToken, UserInfo user, DateTime tokenExpiryTime)
        {
            AccessToken = accessToken;
            IdToken = idToken;
            User = user;
            TokenExpiryTime = tokenExpiryTime;
        }

        public bool IsAuthenticated
        {
            get
            {
                return !string.IsNullOrEmpty(AccessToken);
            }
        }

        public string AccessToken { get; }

        public string IdToken { get; }

        public UserInfo User { get; }

        public DateTime TokenExpiryTime { get; }

        public string OnBoarded { get; set; }

        public static IdentityModel Default()
        {
            return new IdentityModel(string.Empty, string.Empty, UserInfo.Default(), DateTime.MinValue);
        }

        public void SetOnBoardedStatus(string state)
        {
            OnBoarded = state;
        }
    }
}
