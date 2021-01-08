using System.Collections.Generic;
using System.Linq;

namespace Thunder.Platform.Core.Validation
{
    /// <summary>
    /// Represents a validation error information, including error message, params for the message (optional) and the related property name of the validation target (optional).
    /// </summary>
    public class ValidationError
    {
        public string Message { get; set; }

        public Dictionary<string, string> Params { get; set; }

        public string RelatedToProp { get; set; }

        public static implicit operator string(ValidationError validationError) => validationError.ToString();

        public static implicit operator ValidationError(string msg) => Create(msg);

        public static ValidationError Create(string message, Dictionary<string, string> messageParams = null)
        {
            return Create(message, (string)null, messageParams);
        }

        public static ValidationError Create(string message, string relatedToProp, Dictionary<string, string> messageParams = null)
        {
            return new ValidationError
            {
                Message = message,
                Params = messageParams,
                RelatedToProp = relatedToProp
            };
        }

        public override string ToString()
        {
            if (Params == null)
            {
                return Message;
            }

            return Params.Aggregate(
                Message,
                (currentMsg, param) => currentMsg?.Replace($"{{{{{param.Key}}}}}", param.Value));
        }
    }
}
