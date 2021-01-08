using System;

namespace LearnerApp.Services.ExceptionHandler
{
    public interface IExceptionHandler
    {
        void HandleException(Exception exception);
    }
}
