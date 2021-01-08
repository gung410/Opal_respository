using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Business.CQRSClientServices
{
    public class MoveAssesmentClientService : IMoveAssesmentClientService
    {
        private readonly string _apiBaseUrl;
        private readonly string _apiAuthorization;
        private string mediaType ="application/json";

        public string ExecuteAssessmentCommand(string schoolCommands, int ownerid, int customerId)
        {
            return PostSync("/moveassessment/executecommands", schoolCommands, new RequestContext() { CurrentOwnerId = ownerid, CurrentCustomerId = customerId });

        }

        public string GetAssessmentCommands(string schoolStates, int ownerid, int customerId)
        {
            return PostSync("/moveassessment/analyze", schoolStates, new RequestContext() { CurrentOwnerId = ownerid, CurrentCustomerId = customerId });
        }

        public string GetAssessmentStates(string school, int ownerid, int customerId)
        {
            return PostSync("/moveassessment/buildstates", school, new RequestContext() { CurrentOwnerId = ownerid, CurrentCustomerId = customerId });
        }



        public MoveAssesmentClientService(string apiBaseUrl, string apiAuthorization)
        {
            if (string.IsNullOrEmpty(apiBaseUrl))
            {
                throw new ArgumentNullException("apiBaseUrl", "Base URL cannot be NULL.");
            }
            if (string.IsNullOrEmpty(apiAuthorization))
            {
                throw new ArgumentNullException("apiAuthorization", "Authorization cannot be NULL.");
            }
            if (!apiBaseUrl.EndsWith("/"))
            {
                apiBaseUrl = apiBaseUrl + "/";
            }
            _apiBaseUrl = apiBaseUrl;
            _apiAuthorization = apiAuthorization;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="conexusBaseDto"></param>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        protected string PostSync(
            string requestUri,
            string jsonData,
            IRequestContext requestContext)
        {
            try
            {
                var httpClient = new HttpClient();
                InitialRequest(httpClient, requestContext);
                var postBody = jsonData;
                HttpResponseMessage response = httpClient.PostAsync(requestUri, new StringContent(postBody, Encoding.UTF8, mediaType)).GetAwaiter().GetResult();
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    var message = string.Format("The request completed and that there is no additional content to send: Request: GET {0} {1} Response: {2}",
                    requestUri,
                    response.StatusCode,
                    response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    throw new CXCommunicationException(ClientServiceErrorCode.RequestException, response.StatusCode, message, requestContext.CorrelationId);
                }
                if (response.IsSuccessStatusCode)
                {
                    //var responseVersions = ExtraDataAsObject(response, conexusBaseDto, dtoTemplate);
                    return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
                else
                {
                    var message = string.Format("The request could not be completed: Request: POST {0} {1} Response: {2} - {3}",
                    requestUri,
                    postBody,
                    response.StatusCode,
                    response.Content.ReadAsStringAsync().Result);
                    throw new CXCommunicationException(ClientServiceErrorCode.RequestException, response.StatusCode, message, requestContext.CorrelationId);
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


        protected void InitialRequest(HttpClient httpClient, IRequestContext requesContext)
        {
            httpClient.Timeout = Timeout.InfiniteTimeSpan;
            httpClient.BaseAddress = new Uri(_apiBaseUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", _apiAuthorization);
            if (requesContext != null)
            {
                httpClient.DefaultRequestHeaders.Add("cxToken", string.Format("{0}:{1}",
                    requesContext.CurrentOwnerId, requesContext.CurrentCustomerId));
                if (!string.IsNullOrEmpty(requesContext.CorrelationId))
                    httpClient.DefaultRequestHeaders.Add("Correlation-Id", requesContext.CorrelationId);
            }
        }
    }
}
