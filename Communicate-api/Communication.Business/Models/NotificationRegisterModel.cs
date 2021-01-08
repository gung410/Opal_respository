namespace Communication.Business.Models
{
    public class NotificationRegisterModel
    {
        public string UserId { get; set; }
        public string InstanceIdToken { get; set; }
        public string DeviceId { get; set; }
        public string Platform { get; set; }
        public string ClientId { get; set; }
    }
}