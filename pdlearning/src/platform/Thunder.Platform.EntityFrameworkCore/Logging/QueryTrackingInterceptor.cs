using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Thunder.Platform.EntityFrameworkCore.Logging
{
    public class QueryTrackingInterceptor : DbCommandInterceptor
    {
        private readonly IQueryTrackingSource _trackingSource;

        public QueryTrackingInterceptor(IQueryTrackingSource trackingSource)
        {
            _trackingSource = trackingSource;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken)
        {
            command.CommandText = AddTrackingInfoToCommandText(command.CommandText);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken)
        {
            command.CommandText = AddTrackingInfoToCommandText(command.CommandText);
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken)
        {
            command.CommandText = AddTrackingInfoToCommandText(command.CommandText);
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }

        private string AddTrackingInfoToCommandText(string commandText)
        {
            var trackingInformation = _trackingSource.GetAllTrackingInformation();

            return !string.IsNullOrWhiteSpace(trackingInformation) ? $"{trackingInformation} {commandText}" : commandText;
        }
    }
}
