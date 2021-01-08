using System;

// ReSharper disable once CheckNamespace
namespace Microservice.NewsFeed.Application.Consumers.Messages.UserModification
{
    public class UserModificationMessage
    {
        public UserData UserData { get; set; }
    }

    public class UserData
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public DynamicAttributes JsonDynamicAttributes { get; set; }

        public UserIdentity Identity { get; set; }
    }

    public class DynamicAttributes
    {
        public string AvatarUrl { get; set; }
    }

    public class UserIdentity
    {
        public int Id { get; set; }

        public Guid ExtId { get; set; }
    }
}
