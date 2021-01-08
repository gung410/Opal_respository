using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Exceptions;

namespace Thunder.Platform.Cqrs
{
    public class PagedResultAwareValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IValidator<PagedResultRequestDto> _pagedResultRequestDtoValidator;

        public PagedResultAwareValidationBehavior(IValidator<PagedResultRequestDto> pagedResultRequestDtoValidator)
        {
            _pagedResultRequestDtoValidator = pagedResultRequestDtoValidator;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is IPagedResultAware pagedResultAwareRequest)
            {
                var validationErrors = _pagedResultRequestDtoValidator.Validate(pagedResultAwareRequest.PageInfo).Errors;

                if (validationErrors.Count != 0)
                {
                    throw new DataValidationException(new ErrorInfo()
                    {
                        Details = validationErrors.Select(p => new ErrorInfo()
                        {
                            Message = p.ErrorMessage
                        }).ToList()
                    });
                }
            }

            return next();
        }
    }
}
