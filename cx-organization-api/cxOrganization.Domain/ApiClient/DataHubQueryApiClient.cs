using Backend.CrossCutting.HttpClientHelper;
using cxOrganization.Domain.Dtos.DataHub;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using cxOrganization.Domain.Extensions;
using cxPlatform.Core.Extentions;
using GenericLogEventMessage = cxOrganization.Domain.Dtos.DataHub.GenericLogEventMessage;

namespace cxOrganization.Domain.ApiClient
{
    public class DataHubQueryApiClient : IDataHubQueryApiClient
    {
        private readonly ILogger<DataHubQueryApiClient> _logger;
        private readonly DataHubQueryAPISettings _dataHubQueryAPISettings;
        private readonly IHttpRequestSender _httpRequestSender;

        public DataHubQueryApiClient(
            ILogger<DataHubQueryApiClient> logger,
            IOptions<DataHubQueryAPISettings> dataHubQueryAPISettings,
            IHttpRequestSender httpRequestSender)
        {
            _logger = logger;
            _dataHubQueryAPISettings = dataHubQueryAPISettings.Value;
            _httpRequestSender = httpRequestSender;
        }

        public PaginatedList<GenericLogEventMessage> GetAuditLogs(IRequestContext requestContext, UserDtoBase user, int pageIndex, int pageSize)
        {
            var query = BuildQuery(user, pageIndex, pageSize);

            var messages = this.GetMessages(requestContext, query);
            var hasMoreData = false;
            if (pageSize > 0)
            {
                hasMoreData = messages.Count > pageSize;
                if (hasMoreData)
                {
                    messages.RemoveAt(messages.Count - 1);
                }
            }

            return new PaginatedList<GenericLogEventMessage>(messages, pageIndex, pageSize, hasMoreData);
        }

        public async Task<PaginatedList<GenericLogEventMessage>> GetAuditLogsAsync(IRequestContext requestContext, UserDtoBase user, int pageIndex, int pageSize)
        {
            var query = BuildQuery(user, pageIndex, pageSize);

            var messages = await this.GetMessagesAsync(requestContext, query);
            var hasMoreData = false;
            if (pageSize > 0)
            {
                hasMoreData = messages.Count > pageSize;
                if (hasMoreData)
                {
                    messages.RemoveAt(messages.Count - 1);
                }
            }

            return new PaginatedList<GenericLogEventMessage>(messages, pageIndex, pageSize, hasMoreData);
        }
        private static string BuildQuery(UserDtoBase user, int pageIndex, int pageSize)
        {
            var skip = pageIndex * pageSize;
            var query = @"
                {
	                eventMany(filter: {OR:[
		                {routing: {action: 'cxid.system_warn.locked.user', entityId: '{userExtId}'}},
		                {routing: {action: 'cx-organization-api.crud.updated.employee', entityId: '{userId}'}},
		                {routing: {action: 'cx-organization-api.crud.entitystatus_changed.employee', entityId: '{userId}'}},
		                {routing: {action: 'cx-organization-api.crud.created.employee'},
			                payload: {
				                body: {
					                userData: {
						                identity: {
							                id: {userId}
						                }
					                }
				                }
			                }
		                },
		                {routing: {action: 'cx-organization-api.crud.user_membership_created.approvalgroup', entityId: '{userId}'}},
		                {routing: {action: 'cx-organization-api.crud.user_membership_deleted.approvalgroup', entityId: '{userId}'}}
	                  ]},sort: CREATED_ASC, skip: {skip}, limit: {pageSize}) {
	                  _id
	                  type
	                  version
	                  id
	                  created
	                  routing {
		                action
		                actionVersion
		                entity
		                entityId
	                  }
	                  payload {
		                identity {
		                  clientId
		                  customerId
		                  sourceIp
		                  userId
		                  onBehalfOfUser
		                }
		                references {
		                  externalId
		                  correlationId
		                  commandId
		                  eventId
		                }
		                body
	                  }
	                }
                }
            ".Replace("{userExtId}", user.Identity.ExtId)
                .Replace("{userId}", user.Identity.Id.ToString())
                .Replace("{skip}", skip.ToString())
                .Replace("{pageSize}", (pageSize > 0 ? pageSize + 1 : 0).ToString())
                .Replace("'", "\"");
            return query;
        }

        public PaginatedList<GenericLogEventMessage> GetPaginatedEventLogs(IRequestContext requestContext,
            List<string> routingActions,
            List<string> routingEntityIds,
            DateTime? eventCreatedAfter,
            DateTime? eventCreatedBefore,
            DatahubQuerySortField sortField = DatahubQuerySortField.CREATED,
            DatahubQuerySortOrder sortOrder = DatahubQuerySortOrder.DESC,
            int pageIndex = 1,
            int pageSize = 500)
        {
            var query = BuildPaginatedQuery(routingActions, routingEntityIds, eventCreatedAfter, eventCreatedBefore, sortField, sortOrder, pageIndex, pageSize);

            return GetPaginationMessages(requestContext, query);
        }

        public int CountEventLogs(IRequestContext requestContext,
            List<string> routingActions,
            List<string> routingEntityIds,
            DateTime? eventCreatedAfter,
            DateTime? eventCreatedBefore)
        {
            var filter = BuildEventQueryFilter(routingActions, routingEntityIds, eventCreatedAfter, eventCreatedBefore);
            var argument = BuildPaginatedEventQueryArgument(filter);
            var query = $"{{eventTotal({argument})}}";

            var countQueryResponse = PostDataAsObject<DataHubCountQueryResponseDto>(requestContext, "event", query);

            return countQueryResponse?.Data?.EventTotal ?? 0;

        }
        public async Task<PaginatedList<GenericLogEventMessage>> GetPaginatedEventLogsAsync(IRequestContext requestContext, List<string> routingActions, List<string> routingEntityIds, DateTime? eventCreatedAfter, DateTime? eventCreatedBefore, DatahubQuerySortField sortField = DatahubQuerySortField.CREATED, DatahubQuerySortOrder sortOrder = DatahubQuerySortOrder.DESC, int pageIndex = 1, int pageSize = 500)
        {
            var query = BuildPaginatedQuery(routingActions, routingEntityIds, eventCreatedAfter, eventCreatedBefore, sortField, sortOrder, pageIndex, pageSize);

            return await GetPaginationMessagesAsync(requestContext, query);
        }

        public async  Task<int> CountEventLogsAsync(IRequestContext requestContext, List<string> routingActions, List<string> routingEntityIds, DateTime? eventCreatedAfter, DateTime? eventCreatedBefore)
        {
            var filter = BuildEventQueryFilter(routingActions, routingEntityIds, eventCreatedAfter, eventCreatedBefore);
            var argument = BuildPaginatedEventQueryArgument(filter);
            var query = $"{{eventTotal({argument})}}";

            var countQueryResponse = await PostDataAsObjectAsync<DataHubCountQueryResponseDto>(requestContext, "event", query);

            return countQueryResponse?.Data?.EventTotal ?? 0;
        }

        private string BuildPaginatedQuery(List<string> routingActions, List<string> routingEntityIds, DateTime? eventCreatedAfter,
            DateTime? eventCreatedBefore, DatahubQuerySortField sortField = DatahubQuerySortField.CREATED, DatahubQuerySortOrder sortOrder = DatahubQuerySortOrder.DESC, int pageIndex = 1,
            int pageSize = 500)
        {

            var filter = BuildEventQueryFilter(routingActions, routingEntityIds, eventCreatedAfter, eventCreatedBefore);
            var argument = BuildPaginatedEventQueryArgument(filter, sortField, sortOrder, pageIndex, pageSize);

            return $@"{{eventPagination({argument}) {{
	                  count
                      pageInfo{{
                          currentPage
                          perPage
                          pageCount
                          itemCount
                          hasNextPage
                          hasPreviousPage
                      }}
                      items{{
                        _id
	                      type
	                      version
	                      id
	                      created
	                      routing {{
		                    action
		                    actionVersion
		                    entity
		                    entityId
	                      }}
	                      payload {{
		                    identity {{
		                      clientId
		                      customerId
		                      sourceIp
		                      userId
		                      onBehalfOfUser
		                    }}
		                    references {{
		                      externalId
		                      correlationId
		                      commandId
		                      eventId
		                    }}
		                    body
	                      }}
	                   }}
                    }}
                }}";
        }

        private string BuildEventQueryFilter(List<string> routingActions, List<string> routingEntityIds, DateTime? eventCreatedAfter, DateTime? eventCreatedBefore)
        {
            var datetimeFilter = BuildDatetimeFilter(eventCreatedAfter, eventCreatedBefore);
            var routingActionFilter = BuildRoutingActionFilter(routingActions);
            var routingEntityIdFilter = BuildRoutingEntityIdFilter(routingEntityIds);
           
            var otherFilter = $"{routingActionFilter},{routingEntityIdFilter}".Trim(',');

            if (!string.IsNullOrEmpty(otherFilter))
            {
                otherFilter = $"AND: [{otherFilter}]";
            }

            var filter = $"{datetimeFilter},{otherFilter}".Trim(',');
            if (filter != string.Empty)
            {
                return $"{{{filter}}}";
            }
            return "";
        }

        private static string BuildPaginatedEventQueryArgument(string filter, DatahubQuerySortField sortField , DatahubQuerySortOrder sortOrder, int pageIndex, int pageSize)
        {
            var inputBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(filter))
            {
                inputBuilder.Append($"filter:{filter},");
            }

            var orderBy = $"{sortField}_{sortOrder}".ToUpper();
            inputBuilder.Append($"sort:{orderBy},");
            inputBuilder.Append($"page:{pageIndex},");
            inputBuilder.Append($"perPage:{pageSize}");

            var argument = inputBuilder.ToString().Trim(',');
            return argument;
        }
        private static string BuildPaginatedEventQueryArgument(string filter)
        {
            var inputBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(filter))
            {
                inputBuilder.Append($"filter:{filter},");
            }
            var argument = inputBuilder.ToString().Trim(',');
            return argument;
        }
        private string BuildRoutingActionFilter(List<string> routingActions)
        {
            var routingActionFilter = "";
            if (!routingActions.IsNullOrEmpty())
            {
                var routingActionFilterBuilder = new StringBuilder();
                foreach (var routingAction in routingActions)
                {
                    routingActionFilterBuilder.Append($"{{routing: {{action: \"{routingAction}\"}}}},");
                }
                routingActionFilter = $"{{OR: [{routingActionFilterBuilder.ToString().Trim(',')}]}}";
            }

            return routingActionFilter;
        }
        private string BuildRoutingEntityIdFilter(List<string> routingEntityIds)
        {
            var buildRoutingEntityIdFilter = "";
            if (!routingEntityIds.IsNullOrEmpty())
            {
                var routingActionFilterBuilder = new StringBuilder();
                foreach (var routingEntityId in routingEntityIds)
                {
                    routingActionFilterBuilder.Append($"{{routing: {{entityId: \"{routingEntityId}\"}}}},");
                }
                buildRoutingEntityIdFilter = $"{{OR: [{routingActionFilterBuilder.ToString().Trim(',')}]}}";
            }

            return buildRoutingEntityIdFilter;
        }
        private static string BuildDatetimeFilter(DateTime? eventCreatedAfter, DateTime? eventCreatedBefore)
        {
            var datetimeFilter = "";

            if (eventCreatedAfter.HasValue || eventCreatedBefore.HasValue)
            {
                var datetimeFilterBuilder = new StringBuilder();
                if (eventCreatedAfter.HasValue)
                {
                    datetimeFilterBuilder.Append($"gte:\"{eventCreatedAfter.Value.ConvertToISO8601()}\",");
                }

                if (eventCreatedBefore.HasValue)
                {
                    datetimeFilterBuilder.Append($"lte:\"{eventCreatedBefore.Value.ConvertToISO8601()}\"");
                }

                datetimeFilter = $"_operators: {{ created: {{{datetimeFilterBuilder.ToString().Trim(',')} }} }}";
            }

            return datetimeFilter;
        }
        public PaginatedList<GenericLogEventMessage> GetPaginationMessages(IRequestContext requestContext, string query)
        {
            var datahubResponse = ExecuteDatahubQueryPagination(requestContext, query);
            var eventPagination = datahubResponse?.Data?.EventPagination;
            if (eventPagination != null)
            {
                var pageInfo = eventPagination.PageInfo ?? new DataHubQueryPaginationInfo();
                return new PaginatedList<GenericLogEventMessage>(eventPagination.Items, pageInfo.CurrentPage, pageInfo.PerPage, hasMoreData: pageInfo.HasNextPage)
                {
                    TotalItems = eventPagination.Count
                };
            }
            return new PaginatedList<GenericLogEventMessage>();
        }
        public async Task<PaginatedList<GenericLogEventMessage>> GetPaginationMessagesAsync(IRequestContext requestContext, string query)
        {
            var datahubResponse = await ExecuteDatahubQueryPaginationAsync(requestContext, query);
            var eventPagination = datahubResponse?.Data?.EventPagination;
            if (eventPagination != null)
            {
                var pageInfo = eventPagination.PageInfo ?? new DataHubQueryPaginationInfo();
                return new PaginatedList<GenericLogEventMessage>(eventPagination.Items, pageInfo.CurrentPage, pageInfo.PerPage, hasMoreData: pageInfo.HasNextPage)
                {
                    TotalItems = eventPagination.Count
                };
            }
            return new PaginatedList<GenericLogEventMessage>();
        }
        public DataHubQueryPaginationResponseDto ExecuteDatahubQueryPagination(IRequestContext requestContext, string query)
        {
            var resource = "event";

            var data = new
            {
                query = query
            };

            return PostDataAsObject<DataHubQueryPaginationResponseDto>(requestContext, resource, data);
        }
        public async Task<DataHubQueryPaginationResponseDto> ExecuteDatahubQueryPaginationAsync(IRequestContext requestContext, string query)
        {
            var resource = "event";

            var data = new
            {
                query = query
            };

            return await PostDataAsObjectAsync<DataHubQueryPaginationResponseDto>(requestContext, resource, data);
        }
        public List<GenericLogEventMessage> GetMessages(IRequestContext requestContext, string query)
        {
            var datahubResponse = ExecuteDatahubQuery(requestContext, query);
            if (datahubResponse != null 
                && datahubResponse.Data != null
                && datahubResponse.Data.EventMany != null)
            {
                return datahubResponse.Data.EventMany;
            }
            return new List<GenericLogEventMessage>();
        }
        public  async  Task<List<GenericLogEventMessage>> GetMessagesAsync(IRequestContext requestContext, string query)
        {
            var datahubResponse = await ExecuteDatahubQueryAsync(requestContext, query);
            if (datahubResponse != null
                && datahubResponse.Data != null
                && datahubResponse.Data.EventMany != null)
            {
                return datahubResponse.Data.EventMany;
            }
            return new List<GenericLogEventMessage>();
        }
        public DataHubQueryResponseDto ExecuteDatahubQuery(IRequestContext requestContext, string query)
        {
            var resource = "event";

            var data = new
            {
                query = query
            };

            return PostDataAsObject<DataHubQueryResponseDto>(requestContext, resource, data);
        }
        public  async Task<DataHubQueryResponseDto> ExecuteDatahubQueryAsync(IRequestContext requestContext, string query)
        {
            var resource = "event";

            var data = new
            {
                query = query
            };

            return await PostDataAsObjectAsync<DataHubQueryResponseDto>(requestContext, resource, data);
        }

        private T PostDataAsObject<T>(IRequestContext requestContext, string resourcePath, object data)
        {
            try
            {
                var requestUri = ApiClientHelper.GenerateRequestUri(_dataHubQueryAPISettings.APIBaseUrl, resourcePath);

                var response = _httpRequestSender.Post(requestContext, requestUri, data, _dataHubQueryAPISettings.APIAuthorization);
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    var message = string.Format("The request completed and that there is no additional content to send: Request: POST {0} {1} Response: {2}",
                    requestUri,
                    response.StatusCode,
                    response.Content.ReadAsStringAsync().Result);
                    _logger.LogWarning(message);
                    return default(T);
                }
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    var message = string.Format("The request could not be completed: Request: POST {0} Response: {1} - {2}",
                    requestUri,
                    response.StatusCode,
                    response.Content.ReadAsStringAsync().Result);
                    throw new CXCommunicationException(ClientServiceErrorCode.RequestException,
                                                response.StatusCode,
                                                message,
                                                response.Headers.GetHeaderValueByName("Request-Id"));
                }
            }
            catch (CXCommunicationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CXUnhandledException(ClientServiceErrorCode.UnhandledException, ex, ex.Message);
            }
        }

        private async Task<T> PostDataAsObjectAsync<T>(IRequestContext requestContext, string resourcePath, object data)
        {
            try
            {
                var requestUri = ApiClientHelper.GenerateRequestUri(_dataHubQueryAPISettings.APIBaseUrl, resourcePath);

                var response = await PostAsync(requestContext, requestUri, data, _dataHubQueryAPISettings.APIAuthorization);
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    var message = string.Format("The request completed and that there is no additional content to send: Request: POST {0} {1} Response: {2}",
                    requestUri,
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync());
                    _logger.LogWarning(message);
                    return default(T);
                }
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>( await response.Content.ReadAsStringAsync());
                }
                else
                {
                    var message = string.Format("The request could not be completed: Request: POST {0} Response: {1} - {2}",
                    requestUri,
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync());
                    throw new CXCommunicationException(ClientServiceErrorCode.RequestException,
                                                response.StatusCode,
                                                message,
                                                response.Headers.GetHeaderValueByName("Request-Id"));
                }
            }
            catch (CXCommunicationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CXUnhandledException(ClientServiceErrorCode.UnhandledException, ex, ex.Message);
            }
        }

        public async Task<HttpResponseMessage> PostAsync(
            IRequestContext requestContext,
            string requestUri,
            object body,
            string authToken)
        {
            return await this._httpRequestSender.SendAsync(HttpRequestMessageBuilder.GetHttpRequestBuilder()
                .WithMethod(HttpMethod.Post).WithRequestUri(requestUri)
                .WithHeader("cxToken",
                    string.Format("{0}:{1}", (object) requestContext.CurrentOwnerId,
                        (object) requestContext.CurrentCustomerId))
                .WithHeader("Correlation-Id", requestContext.CorrelationId)
                .WithContent((HttpContent) new JsonContent(body)).WithAuthorization(authToken).Build());
        }

      
    }
}
