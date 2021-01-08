using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Services
{
    public class ClassMemberService : IClassMemberService
    {
        private readonly IUserRepository _userRepository;
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly IWorkContext _workContext;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserMappingService _userMappingService;
        private readonly IDepartmentMappingService _departmentMappingService;
        private readonly IDepartmentValidator _departmentValidator;
        private readonly IDepartmentTypeRepository _departmentTypeRepository;
        private readonly IDepartmentService _departmentService;
        private readonly IOwnerRepository _ownerRepository;


        public ClassMemberService(OrganizationDbContext organizaionorganizationDbContext,
            IUserRepository userRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IWorkContext workContext,
            IDepartmentRepository departmentRepository,
            IUserMappingService userMappingService,
            IDepartmentMappingService departmentMappingService,
            ClassValidator departmentValidator,
            IDepartmentTypeRepository departmentTypeRepository,
            IDepartmentService departmentService,
            IOwnerRepository ownerRepository)
        {
            _organizationDbContext = organizaionorganizationDbContext;
            _userRepository = userRepository;
            _workContext = workContext;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _departmentRepository = departmentRepository;
            _userMappingService = userMappingService;
            _departmentMappingService = departmentMappingService;
            _departmentValidator = departmentValidator;
            _departmentTypeRepository = departmentTypeRepository;
            _departmentService = departmentService;
            _ownerRepository = ownerRepository;
        }

        public MemberDto AddOrRemoveMember(HierarchyDepartmentValidationSpecification validationSpecification, int schoolId, int classId, MemberDto memberDto)
        {
            _departmentValidator.ValidateHierarchyDepartment(validationSpecification);
            //Employee will be added if necessary
            if (memberDto.Identity.Archetype != ArchetypeEnum.Learner && memberDto.Identity.Archetype != ArchetypeEnum.Employee)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }
            if (!memberDto.Identity.Id.HasValue)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_ID_IS_REQUIRED);
            }
            var userId = (int)memberDto.Identity.Id.Value;
            var departmentClass = _departmentRepository.GetById(classId);
            if (departmentClass.ArchetypeId != (int)ArchetypeEnum.Class)
                throw new CXValidationException(cxExceptionCodes.VALIDATION_DEPARTMENT_IS_NOT_CLASS);
            var user = _userRepository.GetById(userId);
            //check matching archetypr of memberdto and userentity
            ArchetypeEnum userArchetypeEnum = (ArchetypeEnum)user.ArchetypeId;
            if (memberDto.Identity.Archetype != userArchetypeEnum)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            if (user == null)
                throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
            if (memberDto.EntityStatus.StatusId == EntityStatusEnum.Active)
            {
                memberDto = AddMember(schoolId, classId, memberDto, user);
            }
            else if (memberDto.EntityStatus.StatusId == EntityStatusEnum.Inactive)
            {
                memberDto = RemoveMember(classId, user);
            }
            else
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_STATUSID_IS_INVALID);
            }

            return memberDto;
        }

        private MemberDto AddMember(int schoolId, int classId, MemberDto memberDto, UserEntity user)
        {
            AddMemberToClass(schoolId, classId, user);
            memberDto = _userMappingService.ToMemberDto(user);
            memberDto.EntityStatus.StatusId = EntityStatusEnum.Active;
            return memberDto;
        }
        private void AddMemberToClass(int schoolId, int classId, UserEntity user)
        {
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var hierarchyId = currentHD.HierarchyId;
            var schoolHd = _hierarchyDepartmentRepository.GetHdByHierarchyIdAndDepartmentId(hierarchyId, schoolId, true);
            var classes = _hierarchyDepartmentRepository.GetChildHds(schoolHd.Path, includeInActiveStatus: true);
            var classIds = classes.Select(x => x.DepartmentId);
            if (user.DepartmentId != schoolId && !classIds.Contains(user.DepartmentId))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_IS_NOT_BELONG_TO_SCHOOL);
            }
            user.DepartmentId = classId;

            var currentOwner = _ownerRepository.GetById(_workContext.CurrentOwnerId);

            _userRepository.Update(user, currentOwner.UseHashPassword, currentOwner.UseOTP, currentOwner.DefaultHashMethod);
            _organizationDbContext.SaveChanges();
        }

        private MemberDto RemoveMember(int classId, UserEntity user)
        {
            MemberDto memberDto;
            RemoveUserFromClass(classId, user);
            memberDto = _userMappingService.ToMemberDto(user);
            memberDto.EntityStatus.StatusId = EntityStatusEnum.Inactive;
            return memberDto;
        }
        private void RemoveUserFromClass(int classId, UserEntity user)
        {
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var currentClass = _hierarchyDepartmentRepository.GetHdByHierarchyIdAndDepartmentId(currentHD.HierarchyId, classId, true);
            var parentHdId = currentClass.ParentId ?? 0;
            var schoolHd = _hierarchyDepartmentRepository.GetById(parentHdId);
            if (schoolHd != null)
            {
                user.DepartmentId = schoolHd.DepartmentId;
                var currentOwner = _ownerRepository.GetById(_workContext.CurrentOwnerId);
                _userRepository.Update(user, currentOwner.UseHashPassword, currentOwner.UseOTP, currentOwner.DefaultHashMethod);
                _organizationDbContext.SaveChanges();
            }
        }

        public MemberDto GetMember(HierarchyDepartmentValidationSpecification validationSpecification, int memberId)
        {
            var parentHd = _departmentValidator.ValidateHierarchyDepartment(validationSpecification);
            var user = _userRepository.GetById(memberId);
            if (user.DepartmentId != parentHd.DepartmentId)
            {
                user = null;
            }
            if (user != null && (user.ArchetypeId.HasValue && (user.ArchetypeId.Value == (int)ArchetypeEnum.Learner || user.ArchetypeId.Value == (int)ArchetypeEnum.Employee)))
                return _userMappingService.ToMemberDto(user);
            else
                return null;
        }

        public List<MemberDto> GetMembers(HierarchyDepartmentValidationSpecification validationSpecification, int classId)
        {
            _departmentValidator.ValidateHierarchyDepartment(validationSpecification);
            var users = _userRepository.GetUsersByDepartment(classId, false, false, false, false, true);
            var results = new List<MemberDto>();
            foreach (var user in users)
            {
                if (user.ArchetypeId.HasValue && (user.ArchetypeId.Value == (int)ArchetypeEnum.Learner || user.ArchetypeId.Value == (int)ArchetypeEnum.Employee))
                {
                    var memberDto = _userMappingService.ToMemberDto(user);
                    results.Add(memberDto);
                }
            }
            return results;
        }

        public MemberDto UpdateMember(HierarchyDepartmentValidationSpecification validationSpecification, MemberDto memberDto)
        {
            var classHd = _departmentValidator.ValidateHierarchyDepartment(validationSpecification);
            if (!memberDto.Identity.Id.HasValue)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_ID_IS_REQUIRED);
            }
            var userId = (int)memberDto.Identity.Id.Value;
            var user = _userRepository.GetById(userId);
            if (user == null)
                throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
            if (memberDto.EntityStatus.StatusId == EntityStatusEnum.Inactive)
            {
                memberDto = RemoveMember(classHd.DepartmentId, user);
            }
            else
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_STATUSID_IS_INVALID);
            }
            return memberDto;
        }

        public List<MemberDto> GetClassMemberShip(HierarchyDepartmentValidationSpecification validationSpecicication, int schoolId, int classId)
        {
            _departmentValidator.ValidateHierarchyDepartment(validationSpecicication);
            var result = new List<MemberDto>();
            var classEntity = _departmentRepository.GetDepartmentIncludeDepartmentTypes(classId, _workContext.CurrentOwnerId);
            //var classHd = _hierarchyDepartmentService.GetHdByHierarchyIdAndDepartmentId(hierarchyId, classId);
            if (classEntity == null || (classEntity.ArchetypeId.HasValue && classEntity.ArchetypeId.Value != (int)ArchetypeEnum.Class))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CLASS_NOT_FOUND);
            }
            foreach (var departmenttypeDepartment in classEntity.DT_Ds)
            {
                var item = departmenttypeDepartment?.DepartmentType;
                if (item != null && item.ArchetypeId == (int)ArchetypeEnum.Level)
                {
                    result.Add(ConvertDepartmentTypeToMemberDto(item));
                }
            }
            var schoolEntity = _departmentRepository.GetById(schoolId);
            if (schoolEntity != null && (schoolEntity.ArchetypeId.HasValue && schoolEntity.ArchetypeId.Value == (int)ArchetypeEnum.School))
            {
                result.Add(_departmentMappingService.ToMemberDto(schoolEntity));
            }
            return result;
        }
        private MemberDto ConvertDepartmentTypeToMemberDto(DepartmentTypeEntity departmentType)
        {
            return new MemberDto
            {
                Identity = new IdentityDto
                {
                    Id = int.Parse(departmentType.ExtId),
                    Archetype = departmentType.ArchetypeId.HasValue ? (ArchetypeEnum)departmentType.ArchetypeId : ArchetypeEnum.Unknown,
                    CustomerId = _workContext.CurrentCustomerId,
                    ExtId = departmentType.ExtId,
                    OwnerId = departmentType.OwnerId
                },
                EntityStatus = new EntityStatusDto
                { },
                Role = string.Empty
            };
        }

        public MemberDto AddLearnerToClass(int learnerId, MemberDto classDto)
        {
            if (_workContext.CurrentCustomerId != classDto.Identity.CustomerId
                || _workContext.CurrentOwnerId != classDto.Identity.OwnerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMER_AND_OWNER_ID_NOTMATCH_WITH_CXTOKEN);
            }
            UserEntity user = _userRepository.GetUserByIds(new List<int> { learnerId }, null).FirstOrDefault();
            if (user == null)
                throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
            if (user.ArchetypeId != (short)ArchetypeEnum.Learner && user.ArchetypeId != (short)ArchetypeEnum.Employee)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }
            DepartmentEntity departmentClass = _departmentRepository.GetDepartmentByIdsAndArchetypeId(departmentIds: new List<long?> { classDto.Identity.Id },
                allowArchetypeIds: new List<int> { (int)ArchetypeEnum.Class }).FirstOrDefault();
            if (departmentClass == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CLASS_NOT_FOUND);
            }
            if (departmentClass.ArchetypeId != (int)ArchetypeEnum.Class)
                throw new CXValidationException(cxExceptionCodes.VALIDATION_DEPARTMENT_IS_NOT_CLASS);

            AddMemberToClass(departmentClass.H_D.FirstOrDefault().Parent.DepartmentId, (int)classDto.Identity.Id, user);

            return classDto;

        }
        public MemberDto RemoveLearnerFromClass(int learnerId, MemberDto classDto)
        {
            if (_workContext.CurrentCustomerId != classDto.Identity.CustomerId
                || _workContext.CurrentOwnerId != classDto.Identity.OwnerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMER_AND_OWNER_ID_NOTMATCH_WITH_CXTOKEN);
            }
            var includeProperties = QueryExtension.CreateIncludeProperties<UserEntity>(x => x.Department);
            var user = _userRepository.GetUserByIds(new List<int> { learnerId }, includeProperties).FirstOrDefault();
            if (user == null)
                throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
            if (user.ArchetypeId != (short)ArchetypeEnum.Learner && user.ArchetypeId != (short)ArchetypeEnum.Employee)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }
            if (user.Department.ArchetypeId != (short)ArchetypeEnum.Class)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_BELONG_TO_PARENT_DEPARTMENT);
            }
            if (user.DepartmentId != classDto.Identity.Id)
                throw new CXValidationException(cxExceptionCodes.VALIDATION_DO_NOT_ALLOW_TO_MOVE);

            RemoveUserFromClass((int)classDto.Identity.Id, user);

            return classDto;

        }
        public MemberDto AddTypeToClass(int departmentId, MemberDto departmentTypeDto, bool isUniqueDepartmentType = false)
        {
            DepartmentEntity department = _departmentValidator.ValidateDepartment(departmentId);

            DepartmentTypeEntity addingDepartmentType = _departmentTypeRepository.GetDepartmentTypeByExtId(departmentTypeDto.Identity.Id.ToString());
            if (addingDepartmentType.ArchetypeId != (int)departmentTypeDto.Identity.Archetype)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
            }
            if (addingDepartmentType.ArchetypeId != (int)departmentTypeDto.Identity.Archetype)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }

            _departmentService.UpdateDepartmentType(department, addingDepartmentType, isUniqueDepartmentType);
            return departmentTypeDto;

        }
        public MemberDto RemoveDepartmentTypeClass(int departmentId, MemberDto departmentTypeDto)
        {
            DepartmentEntity department = _departmentValidator.ValidateDepartment(departmentId);

            DepartmentTypeEntity removingDepartmentType = _departmentTypeRepository.GetDepartmentTypeByExtId(departmentTypeDto.Identity.Id.ToString());
            if (removingDepartmentType.ArchetypeId != (int)departmentTypeDto.Identity.Archetype)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
            }
            if (removingDepartmentType.ArchetypeId != (int)departmentTypeDto.Identity.Archetype)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }

            if (!department.DT_Ds.Any(f => f.DepartmentType != null && f.DepartmentType.ExtId == departmentTypeDto.Identity.ExtId))
            {
                return departmentTypeDto;
            }

            department.DT_Ds
                .Remove(department.DT_Ds
                    .FirstOrDefault(t => t.DepartmentType != null && t.DepartmentType.ExtId == removingDepartmentType.ExtId));
            _organizationDbContext.SaveChanges();
            return departmentTypeDto;

        }
        public List<MemberDto> GetClassMemberships(int classId)
        {
            DepartmentEntity classEntity = _departmentRepository.GetDepartments(ownerId: 0,
                departmentIds: new List<int> { classId },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All },
                userIds: null,
                customerIds: null,
                extIds: null,
                parentDepartmentId: 0,
                childrenDepartmentId: 0,
                lastUpdatedAfter: null,
                lastUpdatedBefore: null,
                archetypeIds: new List<int> { (int)ArchetypeEnum.Class },
                departmetTypeExtIds: null).Items.FirstOrDefault();
            List<MemberDto> result = new List<MemberDto>();
            foreach (var dtd in classEntity.DT_Ds)
            {
                var item = dtd.DepartmentType;
                if (item != null && item.ArchetypeId == (int)ArchetypeEnum.Level)
                {
                    result.Add(ConvertDepartmentTypeToMemberDto(item));
                }
            }
            var schoolHD = classEntity.H_D.FirstOrDefault().Parent;
            var schoolEntity = _departmentRepository.GetById(schoolHD.DepartmentId);
            if (schoolEntity != null && (schoolEntity.ArchetypeId.HasValue && schoolEntity.ArchetypeId.Value == (int)ArchetypeEnum.School))
            {
                result.Add(_departmentMappingService.ToMemberDto(schoolEntity));
            }
            return result;
        }

        public Dictionary<string, List<MemberDto>> GetClassesMemberships(List<string> classExtIds, int ownerId, int customerId)
        {
            var result = new Dictionary<string, List<MemberDto>>();
            var classEntities = _departmentRepository.GetDepartments(ownerId: ownerId,
                departmentIds: null,
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All },
                userIds: null,
                customerIds: new List<int> { customerId },
                extIds: classExtIds,
                parentDepartmentId: 0,
                childrenDepartmentId: 0,
                lastUpdatedAfter: null,
                lastUpdatedBefore: null,
                archetypeIds: new List<int> { (int)ArchetypeEnum.Class },
                departmetTypeExtIds: null).Items.ToList();
            foreach (var item in classEntities)
            {
                List<MemberDto> membersDtos = new List<MemberDto>();

                foreach (var dtd in item.DT_Ds)
                {
                    var departmentType = dtd.DepartmentType;
                    if (departmentType != null && departmentType.ArchetypeId == (int)ArchetypeEnum.Level)
                    {
                        membersDtos.Add(ConvertDepartmentTypeToMemberDto(departmentType));
                    }
                }
                var schoolHD = item.H_D.FirstOrDefault().Parent;
                var schoolEntity = _departmentRepository.GetById(schoolHD.DepartmentId);
                if (schoolEntity != null && (schoolEntity.ArchetypeId.HasValue && schoolEntity.ArchetypeId.Value == (int)ArchetypeEnum.School))
                {
                    membersDtos.Add(_departmentMappingService.ToMemberDto(schoolEntity));
                }
                result.Add(item.ExtId, membersDtos);
            }
            return result;
        }
    }
}
