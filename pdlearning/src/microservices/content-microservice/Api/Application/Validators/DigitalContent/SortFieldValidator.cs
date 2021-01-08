using System.Linq;
using FluentValidation;

namespace Microservice.Content.Application.Validators
{
    public class SortFieldValidator : AbstractValidator<string>
    {
        private readonly string[] _sortableField =
        {
            "ChangedDate",
            "AverageRating",
            "ReviewCount"
        };

        public SortFieldValidator()
        {
            RuleFor(p => p).Must(p => _sortableField.Contains(p));
        }
    }
}
