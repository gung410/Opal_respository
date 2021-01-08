using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using cxOrganization.Business.Common;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.Validations
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
  
    public class IdentityValidateAttribute : ValidationAttribute
    {
        public bool Required { get; set; }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                if (Required)
                {
                    ErrorMessage = "{0} is required";
                    return false;
                }
                return true;
            }
            if (value is ICollection)
            {
                var collection = (ICollection) value;
                if (Required && collection.Count == 0)
                {
                    ErrorMessage = "{0} is required to have element.";
                    return false;
                }
                return ValidateCollectionObject(collection);
            }
            return ValidateSingleObject(value);

        }

        private bool ValidateSingleObject(object value)
        {
            var identityWithClaimDto = value as IdentityWithClaimDto;

            if (identityWithClaimDto != null)
            {
                return ValidateIdentity(identityWithClaimDto); 
            }
            var identityDto = value as IdentityDto;
            if (identityDto != null)
            {
                return ValidateIdentity(identityDto);
            }
            return true;
        }

        private bool ValidateCollectionObject(ICollection collection)
        {
            if (collection is ICollection<IdentityWithClaimDto>)
            {
                return ValidateCollectionObject((ICollection<IdentityWithClaimDto>) collection);
            }
            if (collection is ICollection<IdentityDto>)
            {
                return ValidateCollectionObject((ICollection<IdentityDto>) collection);
            }
            return true;
        }

        private bool ValidateCollectionObject<T>(ICollection<T> identities) where T : IdentityDto
        {
            var failIndexs = new List<int>();
            var index = 0;
            foreach (var identity in identities)
            {
                if (IsInvalid(identity))
                {
                    failIndexs.Add(index);
                }
                index++;
            }
            if (failIndexs.Count > 0)
            {
                SetErrorMessage<T>(failIndexs);
                return false;
            }
            return true;
        }

        private bool ValidateIdentity<T>(T identity) where T : IdentityDto
        {
            if (IsInvalid(identity))
            {
                SetErrorMessage<T>();
                return false;
            }
            return true;
        }

        private bool IsInvalid<T>(T identityDto) where T : IdentityDto
        {
            var missingIdAndExtId = (identityDto.Id == null || identityDto.Id <= 0)
                                    && string.IsNullOrEmpty(identityDto.ExtId);

            if (!missingIdAndExtId) return false;

            var identityWithClaim = identityDto as IdentityWithClaimDto;
            return identityWithClaim != null && (identityWithClaim.Claims == null || identityWithClaim.Claims.Count == 0);
           
        }

        private void SetErrorMessage<T>() where T : IdentityDto
        {
            var identityFields = GetIdentityFields<T>();
            ErrorMessage = string.Format("The {{0}} is required one of its sub-fields ({0}) must have value at least.", identityFields);
        }

        private void SetErrorMessage<T>(List<int> indexs) where T : IdentityDto
        {
            var indexString = string.Join(",", indexs.Select(i => string.Format("[{0}]", i)));
            var identityFields = GetIdentityFields<T>();

            ErrorMessage =
                string.Format(
                    "The elements {0} of {{0}} is required one of its sub-fields ({1}) must have value at least.",
                    indexString, identityFields);
        }

        private string GetIdentityFields<T>() where T : IdentityDto
        {
            if (typeof(T) == typeof(IdentityWithClaimDto))
                return "id, extId, claims";
            return "id, extId";
        }
    }

}