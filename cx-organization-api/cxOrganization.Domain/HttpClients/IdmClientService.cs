
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Dtos;
using cxOrganization.Domain.BaseEnums;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using cxOrganization.Domain.Common;

namespace cxOrganization.Domain.HttpClients
{
    public enum IdentityServerStatus
    {
        //Idp Status:
        Inactive = 0,
        Invite = 1,
        Active = 2,
        Suspended = 3,
        DeActivated = 4
    }
    public class IdmClientService : IIdentityServerClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILoginServiceUserRepository _loginServiceRepository;
        private readonly IConfiguration _configuration;
        private string _accessToken;
        private List<UserTypeEntity> _userTypeEntities;
        private readonly List<MappingDto> _mappingDto;
        private readonly IUserTypeRepository _userTypeRepository;

        public IdmClientService(IConfiguration configuration, HttpClient httpClient, ILoginServiceUserRepository loginServiceUserRepository,
            IUserTypeRepository userTypeRepository, IOptions<List<MappingDto>> options)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _loginServiceRepository = loginServiceUserRepository;
            InitialRequest();
            _mappingDto = options.Value;
            _userTypeRepository = userTypeRepository;
        }

        private void InitialRequest()
        {
            var baseUrl = _configuration["IDM_BASEURL"];
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
        }

        public async Task<string> GetAccessToken()
        {
            var grantType = _configuration["IDM_GRANT_TYPE"];
            var clientSecret = _configuration["IDM_CLIENT_SECRET"];
            var clientId = _configuration["IDM_CLIENT_ID"];
            var scope = _configuration["IDM_SCOPE"];

            var formVariables = new List<KeyValuePair<string, string>>();
            formVariables.Add(new KeyValuePair<string, string>("grant_type", grantType));
            formVariables.Add(new KeyValuePair<string, string>("client_id", clientId));
            formVariables.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
            formVariables.Add(new KeyValuePair<string, string>("scope", scope));

            if (_configuration["IDM_GRANT_TYPE"].ToLower() == "password")
            {
                formVariables.Add(new KeyValuePair<string, string>("username", _configuration["IDM_USERNAME"]));
                formVariables.Add(new KeyValuePair<string, string>("password", _configuration["IDM_PASSWORD"]));
            }
            var formContent = new FormUrlEncodedContent(formVariables);
            formContent.Headers.ContentType.CharSet = "UTF-8";
            var response = _httpClient.PostAsync("connect/token", formContent);
            var content = await response.EnsureSuccessStatusCodeAsync();
            var jsonString = await content.ReadAsStringAsync();
            var tokenData = JObject.Parse(jsonString);
            return await Task.FromResult(tokenData.GetValue("access_token").Value<string>());

        }

        public async Task<UserIdentityResponseDto> UpdateUserAsync(long userInternalId,
            UserGenericDto userGenericDto,
            bool keepUserLogin,
            bool isRejectingUser = false)
        {
            await HandleToken(_httpClient);
            var userLoginService = _loginServiceRepository.GetLoginServiceUsers(userIds: new List<int> { (int)userInternalId }, userEntityStatuses: new List<EntityStatusEnum> { EntityStatusEnum.All }).FirstOrDefault();
            if (userLoginService == null)
                throw new cxHttpResponseException(System.Net.HttpStatusCode.NotFound, $"User with id {userInternalId} is not mapped to identity server");
            var userIdentity = await GetUserByIdAsync(userLoginService.PrimaryClaimValue);
            if (userIdentity == null)
                throw new cxHttpResponseException(System.Net.HttpStatusCode.NotFound, $"User with id {userInternalId}-{userLoginService.PrimaryClaimValue} is not existed in identity server");
            userIdentity.FirstName = userGenericDto.FirstName;
            userIdentity.LastName = userGenericDto.LastName;
            userIdentity.Gender = userGenericDto.Gender;
            //userIdentity.IsLocked = userGenericDto.EntityStatus.StatusId == EntityStatusEnum.IdentityServerLocked;
            userIdentity.UnlockUser = userGenericDto.EntityStatus.StatusId != EntityStatusEnum.IdentityServerLocked;
            userIdentity.LockoutEnabled = true;
            userIdentity.Status = MapIdpStatus(userGenericDto.EntityStatus.StatusId);
            userIdentity.DateOfBirth = userGenericDto.DateOfBirth;

            /* In case we are rejecting current user from pending level,
             * we need to update Username to dummy value so that both username and email can be reused */
            if (isRejectingUser)
            {
                userIdentity.Username = userGenericDto.EmailAddress;
            }    
            userIdentity.EmailAddress = userGenericDto.EmailAddress;
            userIdentity.PhoneNumber = userGenericDto.MobileNumber;
            userIdentity.CountryCode = userGenericDto.MobileCountryCode.ToString();
            userIdentity.ExpirationDate = userGenericDto.EntityStatus.ExpirationDate;
            userIdentity.ActiveDate = userGenericDto.EntityStatus.ActiveDate;
            userIdentity.Roles = AssignUserRole(userGenericDto);
            userIdentity.KeepLogin = keepUserLogin;
            UserIdentityOtpDto userIdentityOtpDto = null;
            if (userGenericDto.ResetOtp.HasValue && userGenericDto.ResetOtp.Value)
            {
                var otpResponse = _httpClient.PostAsync($"users/generateotp/{userLoginService.PrimaryClaimValue}?getOtpExpiration=true", null);
                var otpContent = await otpResponse.EnsureSuccessStatusCodeAsync();
                var otpString = await otpContent.ReadAsStringAsync();

                //Only Deserialize to UserIdentityOtp when generating otp with getOtpExpiration=true, Otherwise response will return as string 
                userIdentityOtpDto = JsonConvert.DeserializeObject<UserIdentityOtpDto>(otpString);
                userIdentity.OTPValue = userIdentityOtpDto.Otp;
            }
            var httpContent = new StringContent(JsonConvert.SerializeObject(userIdentity), Encoding.UTF8, "application/json");
            var response = _httpClient.PutAsync($"users/{userLoginService.PrimaryClaimValue}", httpContent);
            var content = await response.EnsureSuccessStatusCodeAsync();

            var userIdentityResponseDto = new UserIdentityResponseDto()
            {
                User = userIdentity
            };

            if (userIdentityOtpDto != null)
            {
                userIdentityResponseDto.Otp = userIdentityOtpDto.Otp;
                userIdentityResponseDto.OtpExpiration = userIdentityOtpDto.OtpExpiration;
            }

            return userIdentityResponseDto;
        }

        private string[] AssignUserRole(UserGenericDto userGenericDto)
        {
            _userTypeEntities = _userTypeEntities ?? _userTypeRepository.GetAllUserTypesInCache();
            if (userGenericDto.CustomData != null && userGenericDto.CustomData.Any())
            {
                var roles = new List<string>();
                var receivedUserTypes = new List<UserTypeDto>();
                foreach (var item in userGenericDto.CustomData)
                {
                    var jArray = item.Value as JArray;
                    if (jArray == null) continue;

                    receivedUserTypes.AddRange(jArray.ToObject<List<UserTypeDto>>());
                }
                var userTypeIds = receivedUserTypes.Select(x => x.Identity.Id);
                var userTypes = _userTypeEntities.Where(x => userTypeIds.Contains(x.UserTypeId));
                foreach (var info in _mappingDto)
                {
                    var match = userTypes.FirstOrDefault(x => x.ExtId == info.From);
                    if (match != null)
                    {
                        roles.Add(info.To);
                    }
                }
                return roles.ToArray();

            }
            return null;
        }

        private int MapIdpStatus(EntityStatusEnum statusId)
        {
            //Idp Status:
            //Inactive = 0,
            //Invite = 1,
            //Active = 2,
            //Suspended = 3,
            //DeActivated = 4
            switch (statusId)
            {
                case EntityStatusEnum.Pending:
                    return 3;
                case EntityStatusEnum.Deactive:
                    return 4;
                case EntityStatusEnum.Active:
                    return 2;
                case EntityStatusEnum.Inactive:
                    return 3;
                case EntityStatusEnum.New:
                    return 2;
                default:
                    return 0;
            }
        }

        private async Task<UserIdentityDto> GetUserByIdAsync(string userId)
        {
            await HandleToken(_httpClient);
            var response = _httpClient.GetAsync($"users?id={userId}");
            var content = await response.EnsureSuccessStatusCodeAsync();
            var json = await content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserIdentityPagingDto>(json);
            return user.Items.FirstOrDefault();
        }

        private async Task HandleToken(HttpClient client)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                _accessToken = await GetAccessToken();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            }
            else
            {
                var jwthandler = new JwtSecurityTokenHandler();
                var jwttoken = jwthandler.ReadJwtToken(_accessToken);
                var expDate = jwttoken.ValidTo;
                if (expDate < DateTime.UtcNow.AddMinutes(1) || string.IsNullOrEmpty(_accessToken))
                {
                    _accessToken = await GetAccessToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                }
                else
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                }
            }
        }

        public async Task<UserIdentityResponseDto> CreateUserAsync(UserGenericDto userGenericDto)
        {
            var userIdm = await GetIdmUserByEmail(userGenericDto.EmailAddress);
            if (userIdm == null)
            {
                await HandleToken(_httpClient);
                var userIdentity = new UserIdentityDto();
                userIdentity.FirstName = userGenericDto.FirstName;
                userIdentity.LastName = userGenericDto.LastName;
                userIdentity.Gender = userGenericDto.Gender;
                //userIdentity.IsLocked = userGenericDto.EntityStatus.StatusId == EntityStatusEnum.IdentityServerLocked;
                userIdentity.Status = MapIdpStatus(userGenericDto.EntityStatus.StatusId);
                userIdentity.UnlockUser = userGenericDto.EntityStatus.StatusId != EntityStatusEnum.IdentityServerLocked;
                userIdentity.DateOfBirth = userGenericDto.DateOfBirth;
                userIdentity.EmailAddress = userGenericDto.EmailAddress;
                userIdentity.PhoneNumber = userGenericDto.MobileNumber;
                userIdentity.CountryCode = userGenericDto.MobileCountryCode.ToString();
                userIdentity.Username = userGenericDto.EmailAddress;
                userIdentity.LockoutEnabled = true;
                userIdentity.IsRequestOTP = true;
                userIdentity.ActiveDate = userGenericDto.EntityStatus.ActiveDate;
                userIdentity.ExpirationDate = userGenericDto.EntityStatus.ExpirationDate;
                userIdentity.Roles = AssignUserRole(userGenericDto);
                var httpContent = new StringContent(JsonConvert.SerializeObject(userIdentity), Encoding.UTF8, "application/json");
                var response = _httpClient.PostAsync($"users", httpContent);
                var content = await response.EnsureSuccessStatusCodeAsync();
                var json = await content.ReadAsStringAsync();
                var postResult = JsonConvert.DeserializeObject<UserIdentityResponseDto>(json);
                return postResult;
            }
            else
            {
                return await UpdateUserAsync(userGenericDto.Identity.Id.Value, userGenericDto, true);
            }


        }

        private async Task<UserIdentityDto> GetIdmUserByEmail(string emailAddress)
        {
            await HandleToken(_httpClient);
            string queryString = "";
            QueryHelpers.AddQueryString(queryString, "emailAddress", emailAddress);
            queryString = QueryHelpers.AddQueryString(queryString, "pageSize", "1");
            QueryHelpers.AddQueryString(queryString, "pageIndex", "1");
            var responseTask = _httpClient.GetAsync($"users?id={emailAddress}");
            var response = await responseTask.EnsureSuccessStatusCodeAsync();
            var json = await response.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserIdentityPagingDto>(json);
            return user.Items.FirstOrDefault();
        }

        public async Task DeleteUserAsync(int userInternalId)
        {
            await HandleToken(_httpClient);
            var userLoginService = _loginServiceRepository.GetLoginServiceUsers(userIds: new List<int> { userInternalId }, userEntityStatuses: new List<EntityStatusEnum> { EntityStatusEnum.All }).FirstOrDefault();
            if (userLoginService == null)
                throw new cxHttpResponseException(System.Net.HttpStatusCode.NotFound, $"User with id {userInternalId} is not mapped to identity server", "", "");
            var response = _httpClient.DeleteAsync($"users/{userLoginService.PrimaryClaimValue}");
            var content = await response.EnsureSuccessStatusCodeAsync();
        }

        public async Task ChangeArchiveUserStatusAsync(int userId, EntityStatusEnum? lastEntityStatusId)
        {
            await HandleToken(_httpClient);

            var userLoginService = _loginServiceRepository.GetLoginServiceUsers(userIds: new List<int> { userId },
                                                                                userEntityStatuses: new List<EntityStatusEnum> { EntityStatusEnum.All })
                                                          .FirstOrDefault();

            if (userLoginService == null)
                throw new cxHttpResponseException(System.Net.HttpStatusCode.NotFound, $"User with id {userId} is not mapped to identity server", "", "");

            if (lastEntityStatusId.HasValue)
            {
                var unarchiveResponse = _httpClient.PostAsync($"users/{userLoginService.PrimaryClaimValue}/unarchive?lastentitystatusid={(int)lastEntityStatusId}", null);
                await unarchiveResponse.EnsureSuccessStatusCodeAsync();
            }
            else
            {
                var archiveResponse = _httpClient.PostAsync($"users/{userLoginService.PrimaryClaimValue}/archive", null);
                await archiveResponse.EnsureSuccessStatusCodeAsync();
            }
        }

        public async Task<List<UserIdentityDto>> GetUsersAsync(UserFilterParams userFilterParams)
        {
            await HandleToken(_httpClient);
            long totalPages = 1;
            long pageIndex = 1;
            long pageSize = 100;
            var users = new List<UserIdentityDto>();
            var queryString = "users?";
            foreach (var item in userFilterParams.GetType().GetProperties())
            {
                if (item.GetValue(userFilterParams) != null)
                    queryString += $"{item.Name}={item.GetValue(userFilterParams)}&";
            }
            queryString += "PageIndex={0}&PageSize={1}";
            while (pageIndex <= totalPages)
            {
                var query = string.Format(queryString, pageIndex, pageSize);
                var response = _httpClient.GetAsync(query);
                var content = await response.EnsureSuccessStatusCodeAsync();
                var json = await content.ReadAsStringAsync();
                var results = JsonConvert.DeserializeObject<UserIdentityPagingDto>(json);
                if (results.Items == null || results.Items.Count <= 0)
                    break;
                pageIndex = results.PagingHeader.PageNumber + 1;
                totalPages = results.PagingHeader.TotalPages;
                users.AddRange(results.Items);
            }

            return users;
        }

        public async Task<UserIdentityResponseDto> UpdateUserStatusAsync(string userInternalId, IdmUserStatus userStatus)
        {
            await HandleToken(_httpClient);
            var userIdentity = await GetUserByIdAsync(userInternalId);
            if (userIdentity == null)
                throw new cxHttpResponseException(System.Net.HttpStatusCode.NotFound, $"User with id {userInternalId} is not mapped to identity server");
            userIdentity.Status = (int)userStatus;

            var httpContent = new StringContent(JsonConvert.SerializeObject(userIdentity), Encoding.UTF8, "application/json");
            var response = _httpClient.PutAsync($"users/{userInternalId}", httpContent);
            var content = await response.EnsureSuccessStatusCodeAsync();
            return new UserIdentityResponseDto()
            {
                User = userIdentity
            };
        }
    }
}
