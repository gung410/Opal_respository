using System;
using cxOrganization.Domain.Dtos.DataHub;
using cxOrganization.Domain.Dtos.Users;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cxOrganization.Domain.ApiClient
{
    public interface IDataHubQueryApiClient
    {
        DataHubQueryResponseDto ExecuteDatahubQuery(IRequestContext requestContext, string query);
        List<GenericLogEventMessage> GetMessages(IRequestContext requestContext, string query);

        PaginatedList<GenericLogEventMessage> GetAuditLogs(IRequestContext requestContext, UserDtoBase user,
            int pageIndex, int pageSize);

        PaginatedList<GenericLogEventMessage> GetPaginatedEventLogs(IRequestContext requestContext,
            List<string> routingActions,
            List<string> routingEntityIds,
            DateTime? eventCreatedAfter,
            DateTime? eventCreatedBefore,
            DatahubQuerySortField sortField = DatahubQuerySortField.CREATED,
            DatahubQuerySortOrder sortOrder = DatahubQuerySortOrder.DESC,
            int pageIndex = 1,
            int pageSize = 500);

        int CountEventLogs(IRequestContext requestContext,
            List<string> routingActions,
            List<string> routingEntityIds,
            DateTime? eventCreatedAfter,
            DateTime? eventCreatedBefore);

        Task<DataHubQueryResponseDto>  ExecuteDatahubQueryAsync(IRequestContext requestContext, string query);
        Task<List<GenericLogEventMessage>> GetMessagesAsync(IRequestContext requestContext, string query);

        Task<PaginatedList<GenericLogEventMessage>> GetAuditLogsAsync(IRequestContext requestContext, UserDtoBase user,
            int pageIndex, int pageSize);

        Task<PaginatedList<GenericLogEventMessage>> GetPaginatedEventLogsAsync(IRequestContext requestContext,
            List<string> routingActions,
            List<string> routingEntityIds,
            DateTime? eventCreatedAfter,
            DateTime? eventCreatedBefore,
            DatahubQuerySortField sortField = DatahubQuerySortField.CREATED,
            DatahubQuerySortOrder sortOrder = DatahubQuerySortOrder.DESC,
            int pageIndex = 1,
            int pageSize = 500);

        Task<int> CountEventLogsAsync(IRequestContext requestContext,
            List<string> routingActions,
            List<string> routingEntityIds,
            DateTime? eventCreatedAfter,
            DateTime? eventCreatedBefore);
    }
}
