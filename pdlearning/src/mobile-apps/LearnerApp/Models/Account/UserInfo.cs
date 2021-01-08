using Xamarin.Forms;

namespace LearnerApp.Models
{
    public class UserInfo
    {
        public string Sub { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public ImageSource Avatar { get; set; }

        public static UserInfo Default()
        {
            return new UserInfo { Sub = string.Empty, Name = string.Empty, Avatar = null };
        }
    }
}
