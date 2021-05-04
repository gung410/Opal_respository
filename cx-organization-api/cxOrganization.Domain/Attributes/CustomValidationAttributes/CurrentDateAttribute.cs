using System;
using System.ComponentModel.DataAnnotations;

namespace cxOrganization.Domain.Attributes.CustomValidationAttributes
{
    public class CurrentDateAttribute : ValidationAttribute
    {
        public CurrentDateAttribute() { }

        public override bool IsValid(object value)
        {
            if (value is null)
            {
                return true;
            }

            return (DateTime)value >= DateTime.Now;
        }
    }
}
