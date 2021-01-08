using System;
using cxOrganization.Domain.Dtos.BroadcastMessage;
using cxOrganization.Domain.Entities.BroadcastMessage;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core.Extentions;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Domain.Common;
using cxPlatform.Core.DatahubLog;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.Logging;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Entities;
using cxPlatform.Core;

namespace cxOrganization.Domain.Services
{
    public class BroadcastMessageService : IBroadcastMessageService
    {
        private readonly IBroadcastMessageRepository _broadcastMessageRepository;
        private readonly IDatahubLogger _datahubLogger;
        private readonly ILogger<BroadcastMessageService> _logger;
        private readonly IUserRepository _userRepository;

        public BroadcastMessageService(
            IBroadcastMessageRepository broadcastMessageRepository,
            IUserRepository userRepository,
            IDatahubLogger datahubLogger,
            ILogger<BroadcastMessageService> logger
        )
        {
            _broadcastMessageRepository = broadcastMessageRepository;
            _userRepository = userRepository;
            _datahubLogger = datahubLogger;
            _logger = logger;
        }

        public async Task<BroadcastMessageDto> GetBroadcastMessageByIdAsync(int broadcastMessageId)
        {
            var broadcastMessage = await _broadcastMessageRepository.GetBroadcastMessageByIdAsync(broadcastMessageId);
            if (broadcastMessage is null)
            {
                return null;
            }

            return BroadcastMessageDto.MapToBroadcastMessageDto(broadcastMessage);
        }

        public async Task<PaginatedList<BroadcastMessageDto>> GetBroadcastMessagesAsync(
            BroadcastMessageSearchRequest broadcastMessageSearchRequest)
        {
            // Get queryable
            var broadcastMessageListQueryable = _broadcastMessageRepository.GetBroadcastMessagesAsNoTracking();

            // Search by text
            if (!string.IsNullOrEmpty(broadcastMessageSearchRequest.SearchText))
            {
                broadcastMessageListQueryable = broadcastMessageListQueryable.Where(x => x.Title.Contains(broadcastMessageSearchRequest.SearchText));
            }

            // Search by status
            if (broadcastMessageSearchRequest.SearchStatus is object && broadcastMessageSearchRequest.SearchStatus.Any())
            {
                broadcastMessageListQueryable = broadcastMessageListQueryable.Where(x => broadcastMessageSearchRequest.SearchStatus.Contains(x.Status));
            }

            // Order
            if (broadcastMessageSearchRequest.OrderBy == OrderBy.Title)
            {
                broadcastMessageListQueryable = broadcastMessageSearchRequest.OrderType == OrderByType.Asc
                    ? broadcastMessageListQueryable.OrderBy(broadcastMessage => broadcastMessage.Title)
                    : broadcastMessageListQueryable.OrderByDescending(broadcastMessage => broadcastMessage.Title);
            }

            if (broadcastMessageSearchRequest.OrderBy == OrderBy.CreatedDate)
            {
                broadcastMessageListQueryable = broadcastMessageSearchRequest.OrderType == OrderByType.Asc
                    ? broadcastMessageListQueryable.OrderBy(broadcastMessage => broadcastMessage.CreatedDate)
                    : broadcastMessageListQueryable.OrderByDescending(broadcastMessage => broadcastMessage.CreatedDate);
            }


            // Paging and mapping to Dto
            var broadcastMessagePagingResult = await broadcastMessageListQueryable.ToPagingAsync(broadcastMessageSearchRequest.PageIndex, broadcastMessageSearchRequest.PageSize);
            var broadcastMessageList = broadcastMessagePagingResult
                .Items
                .Select(x => BroadcastMessageDto.MapToBroadcastMessageDto(x))
                .ToList();

            return new PaginatedList<BroadcastMessageDto>(
                broadcastMessageList,
                broadcastMessageSearchRequest.PageIndex,
                broadcastMessageSearchRequest.PageSize,
                broadcastMessagePagingResult.HasMoreData)
            {
                TotalItems = broadcastMessagePagingResult.TotalItems
            };
        }

        public async Task<BroadcastMessageDto> CreateBroadcastMessageAsync(BroadcastMessageCreationDto broadcastMessageCreationDto, IWorkContext workContext)
        {
            const string logHeader = "CreateBroadcastMessageAsync";

            var broadcastMessageEntity = BroadcastMessageEntity.MapToBroadcastMessageEntity(broadcastMessageCreationDto, new Guid(workContext.UserIdCXID));
            var createdBroadcastMessage = await _broadcastMessageRepository.CreateBroadcastMessageAsync(broadcastMessageEntity);

            var sendingMessageResult = SendBroadcastMessageToQueue(createdBroadcastMessage);

            _logger.LogInformation($"{logHeader}: sendingMessageResult status: {sendingMessageResult}");

            return sendingMessageResult ? BroadcastMessageDto.MapToBroadcastMessageDto(createdBroadcastMessage) : null;
        }

        public async Task<BroadcastMessageDto> UpdateBroadcastMessageAsync(BroadcastMessageDto broadcastMessage, IWorkContext workContext)
        {
            const string logHeader = "UpdateBroadcastMessageAsync";

            var existingBroadcastMessageEntity = await _broadcastMessageRepository.GetBroadcastMessageByIdAsync(broadcastMessage.BroadcastMessageId.Value);

            if (existingBroadcastMessageEntity is null)
            {
                _logger.LogInformation($"{logHeader}: Broadcast message with Id: {broadcastMessage.BroadcastMessageId} is not found!");
                return null;
            }

            // Parse data, please be auto mapping!
            existingBroadcastMessageEntity.Recipient = new Recipient(
                broadcastMessage.Recipients.DepartmentIds,
                broadcastMessage.Recipients.RoleIds,
                broadcastMessage.Recipients.UserIds,
                broadcastMessage.Recipients.GroupIds);
            existingBroadcastMessageEntity.Status = broadcastMessage.Status;
            existingBroadcastMessageEntity.BroadcastContent = broadcastMessage.BroadcastContent;
            existingBroadcastMessageEntity.ValidFromDate = broadcastMessage.ValidFromDate;
            existingBroadcastMessageEntity.ValidToDate = broadcastMessage.ValidToDate;
            existingBroadcastMessageEntity.MonthRepetition = broadcastMessage.MonthRepetition;
            existingBroadcastMessageEntity.DayRepetitions = broadcastMessage.DayRepetitions;
            existingBroadcastMessageEntity.TargetUserType = broadcastMessage.TargetUserType;
            existingBroadcastMessageEntity.SendMode = broadcastMessage.SendMode;
            existingBroadcastMessageEntity.RecurrenceType = broadcastMessage.RecurrenceType;
            existingBroadcastMessageEntity.NumberOfRecurrence = broadcastMessage.NumberOfRecurrence;
            existingBroadcastMessageEntity.OwnerId = broadcastMessage.OwnerId;
            existingBroadcastMessageEntity.Title = broadcastMessage.Title;
            existingBroadcastMessageEntity.LastUpdatedBy = new Guid(workContext.UserIdCXID);

            var updatedBroadcastMessage = await _broadcastMessageRepository.UpdateBroadcastMessageAsync(existingBroadcastMessageEntity);

            if (updatedBroadcastMessage is object)
            {
                return BroadcastMessageDto.MapToBroadcastMessageDto(updatedBroadcastMessage);
            }

            return null;
        }

        public async Task<BroadcastMessageDto> ChangeBroadcastMessageStatusAsync(BroadcastMessageChangeStatusDto broadcastMessageChangeStatusDto, IWorkContext workContext)
        {
            var existingBroadcastMessageEntity = await _broadcastMessageRepository.GetBroadcastMessageByIdAsync(broadcastMessageChangeStatusDto.BroadcastMessageId);
            if (existingBroadcastMessageEntity is null)
            {
                return null;
            }

            existingBroadcastMessageEntity.Status = broadcastMessageChangeStatusDto.Status;
            existingBroadcastMessageEntity.LastUpdatedBy = new Guid(workContext.UserIdCXID);


            var updatedEntity = await _broadcastMessageRepository.UpdateBroadcastMessageAsync(existingBroadcastMessageEntity);

            return BroadcastMessageDto.MapToBroadcastMessageDto(updatedEntity);
        }

        public async Task<BroadcastMessageDto> DeleteBroadcastMessageAsync(int broadcastMessageId)
        {
            var existingBroadcastMessageEntity = await _broadcastMessageRepository.GetBroadcastMessageByIdAsync(broadcastMessageId);
            if (existingBroadcastMessageEntity is null)
            {
                return null;
            }

            var deletedBroadcastMessage = await _broadcastMessageRepository.DeleteBroadcastMessageAsync(broadcastMessageId);

            return BroadcastMessageDto.MapToBroadcastMessageDto(deletedBroadcastMessage);
        }

        public void SendScheduledBroadcastMessages()
        {
            var currentDateTime = DateTime.Now;
            var RecurringBroadcastMessages = _broadcastMessageRepository.GetBroadcastMessagesAsNoTracking()
                .Where(message => message.RecurrenceType != RecurrenceType.None
                    && message.ValidFromDate <= currentDateTime
                    && message.ValidToDate > currentDateTime
                    && !message.Deleted.HasValue);

            foreach (var RecurringBroadcastMessage in RecurringBroadcastMessages)
            {
                CheckAndSendRecurringMessage(RecurringBroadcastMessage, currentDateTime);
            }
        }

        private void CheckAndSendRecurringMessage(BroadcastMessageEntity broadcastMessage, DateTime currentDateTime)
        {
            switch (broadcastMessage.RecurrenceType)
            {
                case RecurrenceType.Week:
                    if (broadcastMessage.DayRepetitions.Contains((DayRepetition)currentDateTime.DayOfWeek)
                        && currentDateTime.Hour == broadcastMessage.ValidFromDate.Hour
                        && currentDateTime.Minute == broadcastMessage.ValidFromDate.Minute)
                    {
                        SendBroadcastMessageToQueue(BroadcastMessageEntity.GenerateRecurringMessage(broadcastMessage, currentDateTime));
                    }

                    break;
                case RecurrenceType.Month:
                    if (broadcastMessage.MonthRepetition == MonthRepetition.OnDay)
                    {
                        if (currentDateTime.Day == broadcastMessage.ValidFromDate.Day
                            && currentDateTime.Hour == broadcastMessage.ValidFromDate.Hour
                            && currentDateTime.Minute == broadcastMessage.ValidFromDate.Minute)
                        {
                            SendBroadcastMessageToQueue(BroadcastMessageEntity.GenerateRecurringMessage(broadcastMessage, currentDateTime));
                        }
                    }
                    if (broadcastMessage.MonthRepetition == MonthRepetition.InOrder)
                    {
                        if (currentDateTime.DayOfWeek == broadcastMessage.ValidFromDate.DayOfWeek
                            && DateTimeHelper.GetWeekOfMonth(currentDateTime) == DateTimeHelper.GetWeekOfMonth(broadcastMessage.ValidFromDate)
                            && currentDateTime.Hour == broadcastMessage.ValidFromDate.Hour
                            && currentDateTime.Minute == broadcastMessage.ValidFromDate.Minute)
                        {
                            SendBroadcastMessageToQueue(BroadcastMessageEntity.GenerateRecurringMessage(broadcastMessage, currentDateTime));
                        }
                    }

                    break;
            }
        }

        private bool SendBroadcastMessageToQueue(BroadcastMessageEntity broadcastMessage)
        {
            const string logHeader = "UpdateBroadcastMessageAsync";

            if (broadcastMessage is null)
            {
                return false;
            }

            // Get original userIds from DB
            var DbUsers = new PaginatedList<UserEntity>();
            var DbusersWithLowerId = new PaginatedList<UserEntity>();
            if (broadcastMessage.Recipient.UserIds.Any())
            {
                DbUsers = _userRepository.GetUsers(extIds:
                    broadcastMessage.Recipient.UserIds.Any()
                    ? broadcastMessage.Recipient.UserIds.Select(userId => userId.ToString().ToUpper()).ToList()
                    : null);
                DbusersWithLowerId = _userRepository.GetUsers(extIds:
                    broadcastMessage.Recipient.UserIds.Any()
                    ? broadcastMessage.Recipient.UserIds.Select(userId => userId.ToString().ToLower()).ToList()
                    : null);
            }

            List<string> userIdLowers = DbUsers.Items.Select(user => user.ExtId).ToList();
            List<string> userIdUppers = DbusersWithLowerId.Items.Select(user => user.ExtId).ToList();
            var userIds = userIdLowers.Concat(userIdUppers).Distinct();

            var broadcastMessageCommand = DomainHelper.GenerateBroadcastCommunicationCommand(
                    Guid.NewGuid().ToString(),
                    broadcastMessage.OwnerId.ToString(),
                    broadcastMessage.Title,
                    broadcastMessage.BroadcastContent,
                    broadcastMessage.Recipient.DepartmentIds.ToList(),
                    broadcastMessage.Recipient.GroupIds.ToList(),
                    broadcastMessage.Recipient.RoleIds.ToList(),
                    userIds.Any() ? userIds.ToList() : new List<string>(),
                    broadcastMessage.TargetUserType,
                    broadcastMessage.SendMode,
                    broadcastMessage.ValidFromDate,
                    broadcastMessage.ValidToDate,
                    broadcastMessage.BroadcastMessageId.ToString()
            );

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter>
                    {
                        new StringEnumConverter()
                    }
            };

            var message = _datahubLogger.WriteCommandLog(broadcastMessageCommand, useMessageRoutingActionAsRoutingKey: true, jsonSerializerSettings: jsonSerializerSettings);
            if (!string.IsNullOrEmpty(message))
            {
                _logger.LogInformation($"{logHeader}: command of sending broadcast message '{broadcastMessage.BroadcastMessageId}' has been published: {message}");
                return true;
            }

            var json = JsonConvert.SerializeObject(broadcastMessageCommand, jsonSerializerSettings);
            _logger.LogInformation($"{logHeader}: Datahub logger has not writtten command message of sending broadcast message '{broadcastMessage.BroadcastMessageId}' with response: {json}");
            return false;
        }
    }
}
