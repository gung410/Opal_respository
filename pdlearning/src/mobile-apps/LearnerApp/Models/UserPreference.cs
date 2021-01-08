namespace LearnerApp.Models
{
    public class UserPreference
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Key { get; set; }

        public bool Value { get; set; }

        public string ValueType { get; set; }
    }
}
