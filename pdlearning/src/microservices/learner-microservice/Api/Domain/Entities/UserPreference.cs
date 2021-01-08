using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Learner.Domain.Entities
{
    public class UserPreference : AuditedEntity
    {
        public Guid UserId { get; set; }

        public string Key { get; set; }

        public string ValueString { get; set; }

        public UserPreferenceValueType ValueType { get; set; }

#pragma warning disable SA1201 // Elements should appear in the correct order
        private object _value;
#pragma warning restore SA1201 // Elements should appear in the correct order

        public object Value
        {
            get
            {
                return _value ??= GetValue();
            }
        }

        private object GetValue()
        {
            switch (ValueType)
            {
                case UserPreferenceValueType.Boolean:
                    return bool.Parse(ValueString);
                case UserPreferenceValueType.Number:
                    return double.Parse(ValueString);
                case UserPreferenceValueType.String:
                    return ValueString;
                default:
                    return null;
            }
        }
    }
}
