using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using cxOrganization.Business.Common;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.Validations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class ArchetypeIdentityValidateAttribute : ValidationAttribute
    {
        protected readonly List<ArchetypeEnum> ExpectedArchetypes;
        protected readonly List<ArchetypeGroup> GroupArchetypes;

        public ArchetypeIdentityValidateAttribute(ImmutableArray<ArchetypeEnum> expectedArchetypes)
        {
            this.ExpectedArchetypes = expectedArchetypes == null ? null : expectedArchetypes.ToList();
        }

        public ArchetypeIdentityValidateAttribute(params ArchetypeEnum[] expectedArchetypes)
        {
            this.ExpectedArchetypes = expectedArchetypes == null ? null : expectedArchetypes.ToList();
        }

        public ArchetypeIdentityValidateAttribute(params ArchetypeGroup[] groupArchetypes)
        {
            this.ExpectedArchetypes = new List<ArchetypeEnum>();
            this.GroupArchetypes = groupArchetypes == null ? new List<ArchetypeGroup>() : groupArchetypes.ToList();
            if (this.GroupArchetypes.Contains(ArchetypeGroup.User))
            {
                ExpectedArchetypes.AddRange(DomainDefinition.UserArchetypes);
            }
            if (this.GroupArchetypes.Contains(ArchetypeGroup.Department))
            {
                ExpectedArchetypes.AddRange(DomainDefinition.DepartmentArchetypes);
            }
            if (this.GroupArchetypes.Contains(ArchetypeGroup.UserGroup))
            {
                ExpectedArchetypes.AddRange(DomainDefinition.UserGroupArchetypes);
            }
        }
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            if (value is IdentityBaseDto)
                return ValidateSingleObject((IdentityBaseDto)value);
         
            if (value is ICollection<IdentityDto>)
                return ValidateCollectionObject(((ICollection<IdentityDto>) value).Cast<IdentityBaseDto>().ToList());
            if (value is ICollection<IdentityBaseDto>)
                return ValidateCollectionObject((ICollection<IdentityBaseDto>) value);
            if (value is ICollection<IdentityWithClaimDto>)
                return ValidateCollectionObject(((ICollection<IdentityWithClaimDto>)value).Cast<IdentityBaseDto>().ToList());
            return true;
        }

        private bool ValidateCollectionObject(ICollection<IdentityBaseDto> identities)
        {
            var failIndexs = new List<int>();
            var index = 0;
            foreach (var identity in identities)
            {
                if (!IsValidArchetype(identity.Archetype))
                {
                    failIndexs.Add(index);
                }
                index++;
            }
            if (failIndexs.Count > 0)
            {
                var indexString = string.Join(",", failIndexs.Select(i => string.Format("[{0}]", i)));

                ErrorMessage =
                  string.Format("The elements {0} of {{0}} are expected to have {1}.",
                      indexString,GetArchetypeType());

                return false;
            }
            return true;
        }

        private bool ValidateSingleObject(IdentityBaseDto value)
        {
            if (!IsValidArchetype(value.Archetype))
            {
                ErrorMessage = string.Format("The {{0}} is expected to have {0}.", GetArchetypeType());
                return false;
            }
            return true;
        }

        protected virtual bool IsValidArchetype(ArchetypeEnum archetype)
        {
            if (ExpectedArchetypes != null && ExpectedArchetypes.Count > 0)
                return ExpectedArchetypes.Contains(archetype);
            return archetype != ArchetypeEnum.Unknown;
        }

        protected virtual string GetArchetypeType()
        {
            if(GroupArchetypes!=null && GroupArchetypes.Count>0)
            {
               return string.Format("archetype in follow groups {0}", string.Join(", ", GroupArchetypes));
            }

            if (ExpectedArchetypes != null && ExpectedArchetypes.Count > 0)
            {
                if (ExpectedArchetypes.Count == 1)
                    return string.Format("{0} archetype", ExpectedArchetypes[0]);
                return string.Format("archetype in following list {0}", string.Join(",", ExpectedArchetypes));
            }
            return "specific archetype";
        }
    }
}