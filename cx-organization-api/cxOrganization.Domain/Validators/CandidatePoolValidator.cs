using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class CandidatePoolValidator : UserGroupValidator
    {
        private readonly IHierarchyDepartmentService _hierachyDepartmentService;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAdvancedWorkContext _workContext;

        public CandidatePoolValidator(IHierarchyDepartmentService hierachyDepartmentService,
            IOwnerRepository ownerRepository,
            IUserGroupRepository userGroupRepository,
            IUserRepository userRepository,
            IAdvancedWorkContext workContext) 
            : base(ownerRepository, workContext,userGroupRepository)
        {
            _hierachyDepartmentService = hierachyDepartmentService;
            _userGroupRepository = userGroupRepository;
            _userRepository = userRepository;
            _workContext = workContext;
        }

        public override UserGroupEntity Validate(ConexusBaseDto dto, IAdvancedWorkContext workContext = null)
        {
            //TODO: Consider to move some common code to base class

            //Archetype must be CandidatePool
            if (dto.Identity.Archetype != ArchetypeEnum.CandidatePool)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            var groupDto = (CandidatePoolDto)dto;


            if ((groupDto.ParentDepartmentId == null || groupDto.ParentDepartmentId <= 0)
                 && (groupDto.ParentUserId == null || groupDto.ParentUserId <= 0))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, "ParentDepartmentId or ParentUserId must have value");
            }


            VailidateParentDeparment(groupDto);


            ValidateParentUser(groupDto);

            var userGroupEntity = base.Validate(dto);
            //Take user entity from existing previous query

            ValidateExistingData(groupDto, userGroupEntity);

            return userGroupEntity;
        }

        private static void ValidateExistingData(CandidatePoolDto groupDto, UserGroupEntity userGroupEntity)
        {
            if (userGroupEntity != null)
            {

                if (userGroupEntity.UserId == null && (userGroupEntity.DepartmentId != null &&
                                                       userGroupEntity.DepartmentId != groupDto.ParentDepartmentId))
                {
                    //If an usergroup is owned by an Department, we do not allow changing department
                    throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM,
                        string.Format("{0} is not belong to the parent department with id {1}",
                            groupDto.Identity.Archetype, userGroupEntity.DepartmentId));
                }


                if (userGroupEntity.UserId != null && userGroupEntity.UserId != groupDto.ParentUserId)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, string.Format("{0} is not belong to the parent user with id {1}", groupDto.Identity.Archetype, userGroupEntity.UserId));
                }

                if (!userGroupEntity.UserGroupTypeId.HasValue || userGroupEntity.ArchetypeId != (int)ArchetypeEnum.CandidatePool)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
                }
            }
        }

        private void ValidateParentUser(CandidatePoolDto groupDto)
        {
            if (groupDto.ParentUserId > 0)
            {
                var user = _userRepository.GetById(groupDto.ParentUserId.Value);
                if (user == null || user.EntityStatusId != (int) EntityStatusEnum.Active || user.Deleted != null)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, string.Format("ParentUserId: {0}", groupDto.ParentUserId));
                }

                if (groupDto.Identity.CustomerId != user.CustomerId)
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMERID_IS_NOT_CORRECT);
                }
            }
        }

        private void VailidateParentDeparment(CandidatePoolDto groupDto)
        {
            if (groupDto.ParentDepartmentId > 0)
            {
                var department = _hierachyDepartmentService.GetH_DByDepartmentID(groupDto.ParentDepartmentId.Value, allowGetDepartmentDeleted: true, includeDepartment: true);
                if (department == null)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, string.Format("ParentDepartmentId: {0}", groupDto.ParentDepartmentId));
                }

                if (groupDto.Identity.CustomerId != department.Department.CustomerId)
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMERID_IS_NOT_CORRECT);
                }
            }
        }

        public override UserEntity ValidateMemberDto(int candidatepoolId, MemberDto member, ref UserGroupEntity userGroup)
        {

            var memberEntity = _userRepository.GetUsers(userIds: new List<int>() { (int)member.Identity.Id }).Items.FirstOrDefault();
            if (memberEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
            }
            //var includeProperties = QueryExtension.CreateIncludeProperties<UserGroupEntity>(x => x.Users, x => x.Department);
            userGroup = _userGroupRepository.GetUserGroupByIds(userGroupIds: new List<int>() { candidatepoolId },
                allowArchetypeIds: new List<int>() { (int)ArchetypeEnum.CandidatePool }, 
                includeProperties: null,
                filters: null).FirstOrDefault();
            if (userGroup == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CANDIDATE_POOL_NOT_FOUND);
            }
            if (userGroup.Department.CustomerId != _workContext.CurrentCustomerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMER_ID_NOT_MATCH);
            }
            ArchetypeEnum enumMemberArchetypeId = (ArchetypeEnum)memberEntity.ArchetypeId;
            if (member.Identity.Archetype != enumMemberArchetypeId && (member.Identity.Archetype != ArchetypeEnum.Candidate || member.Identity.Archetype != ArchetypeEnum.Employee))
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
            }
            if (memberEntity.CustomerId != member.Identity.CustomerId || memberEntity.CustomerId != _workContext.CurrentCustomerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMER_ID_NOT_MATCH);
            }
            return memberEntity;

        }
        public override UserGroupEntity ValidateMembership(MemberDto memberDto)
        {
            var usergroup = base.ValidateMembership(memberDto);

            if (usergroup.ArchetypeId != (short)ArchetypeEnum.CandidatePool)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
            }

            return usergroup;
        }
        public override UserGroupEntity Validate(int userGroupId)
        {
            var entity = base.Validate(userGroupId);
            if(entity.ArchetypeId != (short)ArchetypeEnum.CandidatePool)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            return entity;
        }
    }
}
