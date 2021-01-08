
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Communication.DataAccess.Notification
{
    public class UserNotificationSettingRepository : RepositoryBase<UserNotificationSetting>, IUserNotificationSettingRepository
    {
        public UserNotificationSettingRepository(IConfiguration configuration, IMongoDatabase database) : base(database, configuration)
        {

        }
        public Task<UserNotificationSetting> GetSettingbyUserIdAsync(string userId)
        {
            return base._collection.AsQueryable().Where(t=>t.UserId == userId).FirstOrDefaultAsync();
        }
    }
}
