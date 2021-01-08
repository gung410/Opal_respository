using System;
using System.Transactions;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Conexus.Opal.OutboxPattern
{
    internal static class UnitOfWorkExtension
    {
        /// <summary>
        /// To start a new transaction with an Inner Action in SYNC way.
        /// </summary>
        /// <param name="uowManager">The uow manager.</param>
        /// <param name="innerHandler">The inner handler.</param>
        /// <param name="transactionScope">The transaction scope.</param>
        public static void StartNewTransaction(this IUnitOfWorkManager uowManager, Action innerHandler, TransactionScopeOption transactionScope = TransactionScopeOption.Required)
        {
            using (var uow = uowManager.Begin(transactionScope))
            {
                innerHandler.Invoke();
                uow.Complete();
            }
        }
    }
}
