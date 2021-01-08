using System;

namespace Thunder.Platform.Core.Exceptions
{
    public static class ThunderExceptionExtensions
    {
        public static ErrorInfo ToDisplayError(this BusinessLogicException exception)
        {
            return new ErrorInfo
            {
                Code = nameof(BusinessLogicException),
                Message = exception.Message
            };
        }

        public static ErrorInfo ToDisplayError(this Exception exception)
        {
            return new ErrorInfo
            {
                Code = exception.GetType().Name,
                Message = exception.Message
            };
        }
    }
}
