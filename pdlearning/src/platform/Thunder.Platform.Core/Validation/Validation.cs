using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable SA1402 // File may only contain a single type
namespace Thunder.Platform.Core.Validation
{
    /// <summary>
    /// Represents a validation result. The result is valid if there is no errors.
    /// </summary>
    public class Validation
    {
        public List<ValidationError> Errors { get; protected set; } = new List<ValidationError>();

        public bool IsValid => Errors == null || !Errors.Any();

        public static implicit operator bool(Validation validation) => validation.IsValid;

        public static implicit operator string(Validation validation) => validation.ToString();

        public static implicit operator Validation(string error) => Invalid(error);

        public static implicit operator Validation(List<string> errors) => Invalid(errors.Select(p => (ValidationError)p).ToArray());

        public static implicit operator Validation(List<ValidationError> errors) => Invalid(errors.Select(p => p).ToArray());

        /// <summary>
        /// Return a valid validation result.
        /// </summary>
        /// <returns>A valid validation result.</returns>
        public static Validation Valid()
        {
            return new Validation();
        }

        /// <summary>
        /// Return a invalid validation result.
        /// </summary>
        /// <param name="errors">The validation errors.</param>
        /// <returns>A invalid validation result.</returns>
        public static Validation Invalid(params ValidationError[] errors)
        {
            return new Validation
            {
                Errors = errors != null && errors.Any() ? errors.ToList() : new List<ValidationError> { "Invalid!" }
            };
        }

        /// <summary>
        /// Return a valid validation result if the condition is true, otherwise return a invalid validation with errors.
        /// </summary>
        /// <param name="condition">The valid condition.</param>
        /// <param name="errors">The errors if the valid condition is false.</param>
        /// <returns>A validation result.</returns>
        public static Validation ValidIf(bool condition, params ValidationError[] errors) =>
            condition ? Valid() : Invalid(errors);

        public static Validation FailFast(params Func<Validation>[] validations)
        {
            return validations.Aggregate(Valid(), (acc, validator) => acc.Bind(validator));
        }

        public static Validation FailFast(params Validation[] validations)
        {
            return validations.Aggregate(Valid(), (acc, validator) => acc.Bind(() => validator));
        }

        public static Validation HarvestErrors(params Validation[] validations)
        {
            var errors = validations.SelectMany(p => p.Errors).ToArray();
            return !errors.Any() ? Valid() : Invalid(errors);
        }

        public static Validation HarvestErrors(params Func<Validation>[] validations)
        {
            return HarvestErrors(validations.Select(p => p()).ToArray());
        }

        public static Validation<T> Valid<T>(T value) => Validation<T>.Valid(value);

        public static Validation<T> ValidIfNotNull<T>(T value, params ValidationError[] errors) =>
            value != null ? Validation<T>.Valid(value) : Validation<T>.Invalid(errors);

        public static Validation<T> ValidIf<T>(T target, bool condition, params ValidationError[] errors) =>
            condition ? Validation<T>.Valid(target) : Validation<T>.Invalid(errors);

        public static Validation<T> ValidIf<T>(T target, bool condition, IEnumerable<ValidationError> errors) =>
            condition ? Validation<T>.Valid(target) : Validation<T>.Invalid(errors.ToArray());

        public override string ToString()
        {
            if (Errors == null || !Errors.Any())
            {
                return null;
            }

            var errorsMsg = ErrorsMsg();

            return errorsMsg;
        }

        public string ErrorsMsg()
        {
            return Errors?.Aggregate(
                string.Empty,
                (currentMsg, error) => $"{(currentMsg == string.Empty ? string.Empty : ". ")}{error}.");
        }

        public Validation Match(Func<IEnumerable<ValidationError>, Validation> invalid, Func<Validation> valid)
            => IsValid ? valid() : invalid(this.Errors);

        public Validation And(Validation val)
        {
            return !this.IsValid ? this : val;
        }

        public Validation And(Func<Validation> val)
        {
            return !this.IsValid ? this : val();
        }

        public Validation Or(Validation val)
        {
            return this.IsValid ? this : val;
        }

        public Validation Or(Func<Validation> val)
        {
            return this.IsValid ? this : val();
        }

        public async Task<Validation> And(Task<Validation> val)
        {
            return !this.IsValid ? this : await val;
        }

        public async Task<Validation> And(Func<Task<Validation>> val)
        {
            return !this.IsValid ? this : await val();
        }

        public async Task<Validation> Or(Task<Validation> val)
        {
            return this.IsValid ? this : await val;
        }

        public async Task<Validation> Or(Func<Task<Validation>> val)
        {
            return this.IsValid ? this : await val();
        }
    }

    /// <summary>
    /// Represents a validation result, including the target to validate. The result is valid if there is no errors.
    /// </summary>
    /// <typeparam name="T">The type of target to validate.</typeparam>
    public class Validation<T> : Validation
    {
        public T Target { get; set; }

        public static implicit operator T(Validation<T> validation) => validation.IsValid ? validation.Target : throw new Exception(validation);

        public static implicit operator Validation<T>(List<ValidationError> errors) => Invalid(errors.Select(p => p).ToArray());

        public static implicit operator Validation<T>(T validTarget) => Valid(validTarget);

        public static Validation<T> Valid(T target)
        {
            return new Validation<T>
            {
                Target = target
            };
        }

        public static new Validation<T> Invalid(params ValidationError[] errors)
        {
            return new Validation<T>
            {
                Errors = errors?.ToList()
            };
        }

        public static Validation<T> Invalid(IEnumerable<ValidationError> errors)
        {
            return Invalid(errors.ToArray());
        }

        public static Validation<T> Invalid(ValidationError validationError)
        {
            return new Validation<T>
            {
                Errors = new List<ValidationError> { validationError }
            };
        }

        public TR Match<TR>(Func<IEnumerable<ValidationError>, TR> invalid, Func<T, TR> valid)
            => IsValid ? valid(this.Target) : invalid(this.Errors);
    }

    internal static class ValidationExts
    {
        public static Validation Bind(this Validation val, Func<Validation> f)
            => val.Match(
                invalid: err => Validation.Invalid(err.ToArray()),
                valid: () => f());

        public static Validation<TR> Map<T, TR>(this Validation<T> @this, Func<T, TR> f)
            => @this.IsValid
                ? Validation<TR>.Valid(f(@this.Target))
                : Validation<TR>.Invalid(@this.Errors.ToArray());
    }
}
#pragma warning restore SA1402 // File may only contain a single type
