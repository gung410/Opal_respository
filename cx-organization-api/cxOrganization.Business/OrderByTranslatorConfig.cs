using cxOrganization.Business.Connection;
using cxOrganization.Domain.Entities;
using cxPlatform.Core.Extentions.OrderByExtension;

namespace cxOrganization.Business
{
    public static class OrderByTranslatorConfig
    {
        public static void Configure()
        {
            RegisterConnectionMemberDto();
        }

        private static void RegisterConnectionMemberDto()
        {

            OrderByTranslator.CreateMap<ConnectionMemberDto, UGMemberEntity>()
                .ForMember(dto => dto.Identity.Id, entity => entity.UGMemberId)
                .ForMember(dto => dto.Identity.ExtId, entity => entity.ExtId)
                .ForMember(dto => dto.UserIdentity.Id, entity => entity.UserId)
                .ForMember(dto => dto.UserIdentity.ExtId, entity => entity.User.ExtId)
                .ForMember(dto => dto.FirstName, entity => entity.User.FirstName)
                .ForMember(dto => dto.LastName, entity => entity.User.LastName)
                .ForMember(dto => dto.DateOfBirth, entity => entity.User.DateOfBirth)
                .ForMember(dto => dto.EmailAddress, entity => entity.User.Email)
                .ForMember(dto => dto.Gender, entity => entity.User.Gender)
                .ForMember(dto => dto.MobileNumber, entity => entity.User.Mobile)
                .ForMember(dto => dto.MobileCountryCode, entity => entity.User.CountryCode)
                .ForMember(dto => dto.EntityStatus.LastUpdated, entity => entity.LastUpdated)
                .ForMember(dto => dto.EntityStatus.LastUpdatedBy, entity => entity.LastUpdatedBy)
                .ForMember(dto => (int)dto.EntityStatus.StatusId, entity => entity.EntityStatusId)
                .ForMember(dto => (int)dto.EntityStatus.StatusReasonId, entity => entity.EntityStatusReasonId);


        }
    }
}
