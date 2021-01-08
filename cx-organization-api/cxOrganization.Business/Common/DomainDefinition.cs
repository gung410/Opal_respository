using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cxOrganization.Client.Departments;
using cxOrganization.Client.UserGroups;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Dtos.Users;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.Common
{
    public static class DomainDefinition
    {
        public static ImmutableArray<ArchetypeEnum> UserArchetypes = GetUserArchetypes().ToImmutableArray();
        public static readonly ImmutableArray<ArchetypeEnum> DepartmentArchetypes = GetDepartmentArchetypes().ToImmutableArray();
        public static readonly ImmutableArray<ArchetypeEnum> UserGroupArchetypes = GetUserGroupArchetypes().ToImmutableArray();

        private static readonly Dictionary<ArchetypeEnum, string> ResourcesByArchetype = DefineResourcesByArchetype();
        private static readonly Dictionary<Type, ArchetypeEnum> ArchetypesByType = DefineArchetypesByType();
      
        public static ArchetypeEnum GetArchetype(Type type)
        {
            if (ArchetypesByType.ContainsKey(type))
                return ArchetypesByType[type];
            return ArchetypeEnum.Unknown;
        }
        public static ArchetypeEnum GetArchetype<T>()
        {
            return GetArchetype(typeof(T));
        }
        public static bool IsDepartmentArchetype(ArchetypeEnum archetype)
        {
            return DepartmentArchetypes.Contains(archetype);

        }
        public static bool IsUserGroupArchetype(ArchetypeEnum archetype)
        {
            return UserGroupArchetypes.Contains(archetype);
        }
        public static bool IsUserArchetype(ArchetypeEnum archetype)
        {
            return UserArchetypes.Contains(archetype);

        }
        public static string GetResourceName(ArchetypeEnum archetype)
        {
            if (ResourcesByArchetype.ContainsKey(archetype))
                return ResourcesByArchetype[archetype];
            return archetype.ToString();

        }
   

        private static ArchetypeEnum[] GetUserArchetypes()
        {
            return new []
            {
                ArchetypeEnum.Candidate,
                ArchetypeEnum.Employee,
                ArchetypeEnum.Learner,

            };
        }

        private static ArchetypeEnum[] GetDepartmentArchetypes()
        {
            return new[]
            {
                ArchetypeEnum.DataOwner,
                ArchetypeEnum.Company,
                ArchetypeEnum.Country,
                ArchetypeEnum.OrganizationalUnit,
                ArchetypeEnum.CandidateDepartment,
                ArchetypeEnum.SchoolOwner,
                ArchetypeEnum.School,
                ArchetypeEnum.Class



            };
        }

        private static ArchetypeEnum[] GetUserGroupArchetypes()
        {
            return new[]
            {
                ArchetypeEnum.CandidatePool,
                ArchetypeEnum.TeachingGroup

            };
        }

        private static Dictionary<ArchetypeEnum, string> DefineResourcesByArchetype()
        {
            return new Dictionary<ArchetypeEnum, string>
            {
                {ArchetypeEnum.DataOwner, "dataowners"},
                {ArchetypeEnum.Country, "countries"},
                {ArchetypeEnum.Company, "companies"},
                {ArchetypeEnum.OrganizationalUnit, "organizationalunits"},
                {ArchetypeEnum.Candidate, "candidates"},
                {ArchetypeEnum.Employee, "employees"},
                {ArchetypeEnum.CandidatePool, "candidatepools"},
                {ArchetypeEnum.Role, "roles"},
                {ArchetypeEnum.SchoolOwner, "schoolOwners"},
                {ArchetypeEnum.Class, "classes"},
                {ArchetypeEnum.TeachingGroup, "teachinggroups"},
                {ArchetypeEnum.Learner, "learners"}


            };
        }

        private static Dictionary<Type, ArchetypeEnum> DefineArchetypesByType()
        {
            return new Dictionary<Type, ArchetypeEnum>
            {
                {typeof(DataOwnerDto), ArchetypeEnum.DataOwner},
                {typeof(CountryDto), ArchetypeEnum.Country},
                {typeof(CompanyDto), ArchetypeEnum.Company},
                {typeof(OrganizationalUnitDto), ArchetypeEnum.OrganizationalUnit},
                {typeof(CandidateDto), ArchetypeEnum.Candidate},
                {typeof(EmployeeDto), ArchetypeEnum.Employee},
                {typeof(CandidatePoolDto), ArchetypeEnum.CandidatePool},
                {typeof(UserTypeDto), ArchetypeEnum.Role},
                {typeof(SchoolOwnerDto), ArchetypeEnum.SchoolOwner},
                {typeof(ClassDto), ArchetypeEnum.Class},
                {typeof(TeachingGroupDto), ArchetypeEnum.TeachingGroup},
                {typeof(LearnerDto), ArchetypeEnum.Learner}


            };
        }
    }

}
