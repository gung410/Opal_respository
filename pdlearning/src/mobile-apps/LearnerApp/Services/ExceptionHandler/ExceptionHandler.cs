using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Crashes;
using Refit;

namespace LearnerApp.Services.ExceptionHandler
{
    public class ExceptionHandler : IExceptionHandler
    {
        public ExceptionHandler()
        {
        }

        public void HandleException(Exception exception)
        {
            if (exception is AggregateException aggregateException)
            {
                aggregateException.Handle(inner =>
                {
                    CaptureException(inner);
                    return true;
                });
            }
            else
            {
                CaptureException(exception);
            }
        }

        private void CaptureException(Exception exception)
        {
            var properties = new Dictionary<string, string>();
            if (exception is ApiException apiException)
            {
                properties.Add("RequestUri", apiException.Uri.ToString());
            }

            Crashes.TrackError(exception, properties);
        }
    }
}
