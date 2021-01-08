using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Services
{
    public class CandidatePoolMemberService : ICandidatePoolMemberService
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly IUserGroupValidator _candidateUserGroupValidator;
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserMappingService _userMappingService;
        private readonly IDepartmentValidator _departmentValidator;
        private readonly Func<ArchetypeEnum, IUserValidator> _userValidator;
        private readonly IUGMemberRepository _userGroupUserRepository;
        private readonly IUserGroupUserMappingService _userGroupUserMappingService;

        public CandidatePoolMemberService(IUserGroupRepository userGroupRepository,
            OrganizationDbContext organizationDbContext,
            CandidatePoolValidator userGroupValidator,
            IUserRepository userRepository,
            IDepartmentRepository departmentRepository,
            IUserMappingService userMappingService,
            IDepartmentValidator departmentValidator,
            Func<ArchetypeEnum, IUserValidator> userValidator,
            IUGMemberRepository userGroupUserRepository,
            IUserGroupUserMappingService userGroupUserMappingService)
        {
            _userGroupRepository = userGroupRepository;
            _organizationDbContext = organizationDbContext;
            _candidateUserGroupValidator = userGroupValidator;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _userMappingService = userMappingService;
            _departmentValidator = departmentValidator;
            _userValidator = userValidator;
            _userGroupUserRepository = userGroupUserRepository;
            _userGroupUserMappingService = userGroupUserMappingService;
        }
        public MemberDto AddMember(int candidatePoolId, MemberDto memberDto)
        {
            UserEntity user;
            UserGroupEntity group = null;
            _candidateUserGroupValidator.ValidateMemberDto(candidatePoolId, memberDto, ref group);
            user = _userRepository.GetById(memberDto.Identity.Id);
            if (user == null)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CANDIDATE_ID_NOT_FOUND, memberDto.Identity.Id);
            }
            if (user.ArchetypeId.HasValue && user.ArchetypeId.Value != (int)ArchetypeEnum.Candidate)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }

            if (memberDto.EntityStatus.StatusId == EntityStatusEnum.Active)
            {
                var result = _userGroupRepository.UpdateUG_U(candidatePoolId.ToString(), memberDto.Identity.Id.ToString(), new List<int>());
                if (result)
                {
                    memberDto = _userMappingService.ToMemberDto(user);
                    memberDto.EntityStatus.StatusId = EntityStatusEnum.Active;
                }
                else
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_CANDIDATEPOOL_ID_NOT_FOUND, candidatePoolId);
                }

            }
            else if (memberDto.EntityStatus.StatusId == EntityStatusEnum.Inactive || memberDto.EntityStatus.StatusId == EntityStatusEnum.Deactive)
            {
                var result = _userGroupRepository.UpdateUG_U(string.Empty, memberDto.Identity.Id.ToString(), new List<int>() { candidatePoolId });
                if (result)
                {
                    memberDto = _userMappingService.ToMemberDto(user);
                    memberDto.EntityStatus.StatusId = EntityStatusEnum.Inactive;
                }
                else
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_CANDIDATEPOOL_ID_NOT_FOUND, candidatePoolId);
                }
            }
            else
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_STATUSID_IS_INVALID);
            }
            _organizationDbContext.SaveChanges();
            return memberDto;
        }
        public List<MemberDto> GetMembers(int candidatePoolId)
        {
            var userGroup = _userGroupRepository.GetUserGroupById(candidatePoolId, true, true, EntityStatusEnum.All);
            var results = new List<MemberDto>();
            foreach (var item in userGroup.UGMembers)
            {
                if (item.User.ArchetypeId.HasValue && item.User.ArchetypeId.Value == (int)ArchetypeEnum.Candidate)
                {
                    var memberDto = new MemberDto
                    {
                        Identity = new IdentityDto
                        {
                            Id = item.UserId,
                            Archetype = ArchetypeEnum.Candidate,
                            CustomerId = item.User.CustomerId ?? 0,
                            ExtId = item.User.ExtId,
                            OwnerId = item.User.OwnerId
                        },
                        EntityStatus = new EntityStatusDto
                        {
                            EntityVersion = item.User.EntityVersion,
                            LastUpdated = item.User.LastUpdated,
                            LastUpdatedBy = item.User.LastUpdatedBy ?? 0,
                            StatusId = (EntityStatusEnum)item.User.EntityStatusId,
                            StatusReasonId = item.User.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)item.User.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                            LastExternallySynchronized = item.User.LastSynchronized,
                            ExternallyMastered = item.User.Locked == 1,
                            Deleted = item.User.Deleted.HasValue
                        },
                        Role = string.Empty
                    };
                    results.Add(memberDto);
                }
            }
            return results;
        }

        public MemberDto GetMember(int candidatePoolId, int memberId)
        {
            var userGroup = _userGroupRepository.GetUserGroupById(candidatePoolId, true, true, EntityStatusEnum.All);
            var ugmember = userGroup.UGMembers.FirstOrDefault(x => x.validFrom == null && x.ValidTo == null && x.UserId == memberId);
            var user = ugmember != null ? ugmember.User : null;
            if (user != null && (user.ArchetypeId.HasValue && user.ArchetypeId.Value == (int)ArchetypeEnum.Candidate))
            {
                return new MemberDto
                {
                    Identity = new IdentityDto
                    {
                        Id = user.UserId,
                        Archetype = ArchetypeEnum.Candidate,
                        CustomerId = user.CustomerId ?? 0,
                        ExtId = user.ExtId,
                        OwnerId = user.OwnerId
                    },
                    EntityStatus = new EntityStatusDto
                    {
                        EntityVersion = user.EntityVersion,
                        LastUpdated = user.LastUpdated,
                        LastUpdatedBy = user.LastUpdatedBy ?? 0,
                        StatusId = (EntityStatusEnum)user.EntityStatusId,
                        StatusReasonId = user.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)user.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                        LastExternallySynchronized = user.LastSynchronized,
                        ExternallyMastered = user.Locked == 1,
                        Deleted = user.Deleted.HasValue
                    },
                    Role = string.Empty
                };
            }
            else
                return null;
        }

        public List<MemberDto> GetUserGroupMemberShip(int userGroupId)
        {
            var result = new List<MemberDto>();
            var userGroupEntity = _userGroupRepository.GetUserGroupIncludeDepartmentType(userGroupId, EntityStatusEnum.All);
            if (userGroupEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_FOUND);
            }

            foreach (var deprtmentTypeUG in userGroupEntity.DT_UGs)
            {
                var item = deprtmentTypeUG.DepartmentType;
                if (item.ArchetypeId == (int)ArchetypeEnum.Level)
                {
                    result.Add(new MemberDto
                    {
                        Identity = new IdentityDto
                        {
                            Id = item.DepartmentTypeId,
                            Archetype = item.ArchetypeId.HasValue ? (ArchetypeEnum)item.ArchetypeId : ArchetypeEnum.Unknown,
                            //CustomerId = item.CustomerId ?? item.CustomerId.Value,
                            ExtId = item.ExtId,
                            OwnerId = item.OwnerId
                        },
                        EntityStatus = new EntityStatusDto
                        {
                            //EntityVersion = item.EntityVersion,
                            //LastUpdated = item.LastUpdated,
                            //LastUpdatedBy = item.LastUpdatedBy ?? item.LastUpdatedBy.Value,
                            //StatusId = (StatusEnum)item.EntityStatusId,
                            //StatusReasonId = (StatusReasonEnum)item.EntityStatusReasonId,
                            //ExternallySynchronized = item.LastSynchronized
                        },
                        Role = string.Empty
                    });
                }
            }

            var departmentEntity = _departmentRepository.GetById(userGroupEntity.DepartmentId);
            if (departmentEntity != null && departmentEntity.ArchetypeId.HasValue)
            {
                result.Add(new MemberDto
                {
                    Identity = new IdentityDto
                    {
                        Id = departmentEntity.DepartmentId,
                        Archetype = (ArchetypeEnum)departmentEntity.ArchetypeId,
                        CustomerId = departmentEntity.CustomerId ?? 0,
                        ExtId = departmentEntity.ExtId,
                        OwnerId = departmentEntity.OwnerId
                    },
                    EntityStatus = new EntityStatusDto
                    {
                        EntityVersion = departmentEntity.EntityVersion,
                        LastUpdated = departmentEntity.LastUpdated,
                        LastUpdatedBy = departmentEntity.LastUpdatedBy ?? 0,
                        StatusId = (EntityStatusEnum)departmentEntity.EntityStatusId,
                        StatusReasonId = departmentEntity.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)departmentEntity.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                        LastExternallySynchronized = departmentEntity.LastSynchronized,
                        ExternallyMastered = departmentEntity.Locked == 1,
                        Deleted = departmentEntity.Deleted.HasValue
                    },
                    Role = string.Empty
                });
            }
            return result;
        }

        protected EntityStatusEnum ToEntityStatusEnum(short status, int? entityStatusId)
        {
            var entityStatus = EntityStatusEnum.Unknown;
            if (entityStatusId.HasValue)
            {
                entityStatus = (EntityStatusEnum)entityStatusId;
            }
            //Return Active
            if (status == 0 && (entityStatus == EntityStatusEnum.Active || entityStatus == EntityStatusEnum.Unknown))
            {
                return EntityStatusEnum.Active;
            }

            //Return Inactive
            if (status == 1 || entityStatus == EntityStatusEnum.Inactive)
            {
                return EntityStatusEnum.Inactive;
            }

            return EntityStatusEnum.Deactive;
        }
    }
}
