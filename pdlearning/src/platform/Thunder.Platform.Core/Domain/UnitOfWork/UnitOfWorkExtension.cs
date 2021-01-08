using System;
using System.Threading.Tasks;
using System.Transactions;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Thunder.Platform.Core.Domain.UnitOfWork
{
    public static class UnitOfWorkExtension
    {
        public static async Task StartNewTransactionAsync(this IUnitOfWorkManager uowManager, Func<Task> innerHandler, TransactionScopeOption transactionScope = TransactionScopeOption.Required)
        {
            using (var uow = uowManager.Begin(transactionScope))
            {
                await innerHandler.Invoke();
                await uow.CompleteAsync();
            }
        }
    }
}
