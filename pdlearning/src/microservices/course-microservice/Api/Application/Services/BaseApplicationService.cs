using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Services
{
    public class BaseApplicationService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public BaseApplicationService(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWorkManager)
        {
            ThunderCqrs = thunderCqrs;
            _unitOfWorkManager = unitOfWorkManager;
        }

        protected IThunderCqrs ThunderCqrs { get; }

        protected async Task<T> ExecuteInUoW<T>(Func<Task<T>> serviceFn)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Required))
            {
                var result = await serviceFn();
                await uow.CompleteAsync();
                return result;
            }
        }

        protected async Task ExecuteInUoW(Func<Task> serviceFn)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Required))
            {
                await serviceFn();
                await uow.CompleteAsync();
            }
        }
    }
}
