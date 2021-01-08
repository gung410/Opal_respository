using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace cxOrganization.WebServiceAPI.Validation
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class CollectionRequiredAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                ErrorMessage = "The {0} field is required.";
                return false;
            }
            if (value is ICollection)
            {
                var collection = (ICollection)value;
                if (collection.Count == 0)
                {
                    ErrorMessage = "The {0} field is required to have an element at least.";
                    return false;
                }
            }
            return true;
        }

    }
}
