using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class UserPreferenceModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Key { get; set; }

        public object Value { get; set; }

        public UserPreferenceValueType ValueType { get; set; }
    }
}
