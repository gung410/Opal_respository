using System;
using System.Net;
using System.Security.Authentication;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Thunder.Platform.AspNetCore.Extensions;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Helpers;

namespace Thunder.Platform.AspNetCore.ExceptionHandling
{
    public partial class ThunderExceptionFilter : IExceptionFilter
    {
        private const string DefaultServerErrorMessage =
            "There is an exception during the processing of the request. Please try again!";

        private readonly ILogger _logger;
        private readonly bool _developerExceptionEnabled;

        public ThunderExceptionFilter(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<ThunderExceptionFilter>();
            _developerExceptionEnabled = configuration.GetValue<bool>("DeveloperExceptionEnabled");
        }

        public void OnException([NotNull] ExceptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.ActionDescriptor.IsControllerAction())
            {
                return;
            }

            var exception = context.Exception;
            if (!HandleAuthenticationException(exception, out ErrorResponse errorResponse) &&
                !HandleEntityNotFoundException(exception, out errorResponse) &&
                !HandleBusinessLogicException(exception, out errorResponse))
            {
                Log.GeneralRequestError(_logger, exception);
                errorResponse = new ErrorResponse(
                    new ErrorInfo
                    {
                        Code = "InternalServerException",
                        Message = _developerExceptionEnabled ? exception.ToString() : DefaultServerErrorMessage,
                    },
                    HttpStatusCode.BadRequest);
            }

            context.Result = new JsonResult(errorResponse);
            context.HttpContext.Response.StatusCode = errorResponse.StatusCode;
            context.ExceptionHandled = true;
        }

        private bool HandleBusinessLogicException(Exception exception, out ErrorResponse errorResponse)
        {
            var businessLogicException = ExceptionHelper.GetFirstExceptionOfType<BusinessLogicException>(exception);
            if (businessLogicException != null)
            {
                if (businessLogicException is DataValidationException dataValidationError)
                {
                    Log.KnownRequestError(_logger, nameof(DataValidationException), exception);
                    errorResponse = new ErrorResponse(dataValidationError.ValidationError, HttpStatusCode.BadRequest);
                }
                else
                {
                    Log.KnownRequestError(_logger, nameof(BusinessLogicException), exception);
                    errorResponse = new ErrorResponse(businessLogicException.ToDisplayError(), HttpStatusCode.BadRequest);
                }

                return true;
            }

            errorResponse = null;
            return false;
        }

        private bool HandleEntityNotFoundException(Exception exception, out ErrorResponse errorResponse)
        {
            // From Toan Nguyen: This is a we really don't want to. We need to check and return the EF Not Found exception.
            // There is a case that another processor/API need to call our API directly instead of another methods.
            // In this case, there is no user id in the user context.
            // Remember, we don't want to support this error for Client Web because it may lead to brute force attack by sending incorrect ID.
            var userId = UserContext.Current.GetValue<string>(CommonUserContextKeys.UserId);
            if (string.IsNullOrEmpty(userId))
            {
                var notFoundException = ExceptionHelper.GetFirstExceptionOfType<EntityNotFoundException>(exception);
                if (notFoundException != null)
                {
                    errorResponse = new ErrorResponse(
                        error: new ErrorInfo
                        {
                            Code = nameof(EntityNotFoundException),
                            Message = "Unable to find the entity."
                        },
                        statusCode: HttpStatusCode.NotFound);

                    return true;
                }
            }

            errorResponse = null;
            return false;
        }

        private bool HandleAuthenticationException(Exception exception, out ErrorResponse errorResponse)
        {
            AuthenticationException authenticationException = ExceptionHelper.GetFirstExceptionOfType<AuthenticationException>(exception);
            if (authenticationException != null)
            {
                Log.KnownRequestError(_logger, nameof(AuthenticationException), exception);

                errorResponse = new ErrorResponse(
                    new ErrorInfo
                    {
                        Code = nameof(AuthenticationException),
                        Message = "Unauthorized request.",
                    },
                    HttpStatusCode.Unauthorized);

                return true;
            }

            errorResponse = null;

            return false;
        }
    }
}
