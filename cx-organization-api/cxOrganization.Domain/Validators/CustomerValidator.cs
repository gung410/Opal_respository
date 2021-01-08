using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using cxOrganization.Client.Customers;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class CustomerValidator : ICustomerValidator
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILanguageRepository _languageRepository;
        public CustomerValidator(ICustomerRepository customerRepository,
            ILanguageRepository languageRepository)
        {
            _customerRepository = customerRepository;
            _languageRepository = languageRepository;
        }
        public CustomerEntity Validate(ConexusBaseDto dto)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(dto, null, null);
            var isValId = Validator.TryValidateObject(dto, context, validationResults, true);
            if (!isValId)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_DEPARTMENT_PROPERTY_VALIDATION, validationResults);
            }
            var cusDto = (CustomerDto)dto;
            if (cusDto.LanguageId == 0)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_FIELD_REQUIRED, "LanguageId");
            }
            else if (cusDto.LanguageId > 0)
            {
                var language = _languageRepository.GetById(cusDto.LanguageId);
                if (language == null)
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_LANGUAGEID_NOT_FOUND);
                }
            }
            if (string.IsNullOrEmpty(cusDto.Identity.ExtId))
            {
                var customerByExtId = _customerRepository.GetCustomerByExtId(cusDto.Identity.ExtId);
                if(customerByExtId != null && cusDto.Identity.Id != customerByExtId.CustomerId)
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMERID_IS_NOT_CORRECT);
                }
            }
            if (dto.Identity.Id > 0)
            {
                return _customerRepository.GetById(dto.Identity.Id);
            }
            return null;

        }
    }
}
