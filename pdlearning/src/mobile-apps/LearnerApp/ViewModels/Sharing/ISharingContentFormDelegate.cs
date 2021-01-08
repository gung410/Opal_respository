using System.Threading.Tasks;
using LearnerApp.Models.UserOnBoarding;

namespace LearnerApp.ViewModels.Sharing
{
    public interface ISharingContentFormDelegate
    {
        Task<bool> AddShareUser(UserInformation userInformation);
    }
}
