using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Thunder.Platform.Core.Validation
{
    /// <summary>
    /// Represents a validator, which have validation rule based on a expression for the target to validate.
    /// </summary>
    /// <typeparam name="T">The type of target to validate.</typeparam>
    public class ExpressionValidator<T>
    {
        private readonly List<ValidationError> _ifInvalidErrors;

        public ExpressionValidator(
            Expression<Func<T, bool>> isValidExpression,
            params ValidationError[] ifInvalidErrors)
        {
            IsValidExpression = isValidExpression;
            _ifInvalidErrors = ifInvalidErrors?.ToList();
        }

        public Expression<Func<T, bool>> IsValidExpression { get; private set; }

        public Validation<T> Validate(T target)
        {
            var valid = IsValidExpression.Compile()(target);
            return !valid ? Validation<T>.Invalid(_ifInvalidErrors) : Validation<T>.Valid(target);
        }
    }
}
