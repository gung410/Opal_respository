using System.Collections.Generic;

namespace cxOrganization.Domain.Dtos.Users
{
    public class MassUserCreationValidationResultDto
    {
        public MassUserCreationValidationResultDto()
        {
            invalidMassUserCreationDto = new List<InvalidMassUserCreationDto>();
        }
        public static short LimitRecordsToBeUpLoaded { get { return 50; } }
        public List<InvalidMassUserCreationDto> invalidMassUserCreationDto { get; set; }
        public long TotalRecords { get; set; } = 0;
        public bool IsValidToMassUserCreation { get { return invalidMassUserCreationDto.Count == 0 && TotalRecords <= LimitRecordsToBeUpLoaded && TotalRecords > 0 && Exception == null; } }
        public long NumberOfValidRecords { get { return TotalRecords - NumberOfInValidRecords; } }
        public long NumberOfInValidRecords { get { return invalidMassUserCreationDto.Count; } }
        public MassUserCreationException Exception { get; set; }
    }

    public class InvalidMassUserCreationDto : MassUserCreationDto
    {
        public string Reason { get; set; }

        public static List<InvalidMassUserCreationDto> GenerateRowNumber(List<InvalidMassUserCreationDto> invalidMassUserCreationDtos)
        {
            var massUserCreations = new List<InvalidMassUserCreationDto>();
            int rowNumber = 0;
            foreach (var invalidUserCreatinoDto in invalidMassUserCreationDtos)
            {
                rowNumber++;
                massUserCreations.Add(new InvalidMassUserCreationDto
                {
                    Number = rowNumber,
                    Salutation = invalidUserCreatinoDto.Salutation,
                    Name = invalidUserCreatinoDto.Name,
                    OfficialEmail = invalidUserCreatinoDto.OfficialEmail,
                    Gender = invalidUserCreatinoDto.Gender,
                    PlaceOfWork = invalidUserCreatinoDto.PlaceOfWork,
                    AccountActiveFrom = invalidUserCreatinoDto.AccountActiveFrom,
                    DateOfExpiryAccount = invalidUserCreatinoDto.DateOfExpiryAccount,
                    ReasonForUserAccountRequest = invalidUserCreatinoDto.ReasonForUserAccountRequest,
                    SystemRole = invalidUserCreatinoDto.SystemRole,
                    Reason = invalidUserCreatinoDto.Reason,
                    PersonalSpaceLimitation = invalidUserCreatinoDto.PersonalSpaceLimitation
                }
            );

            }
            return massUserCreations;
        }
    }

    public class MassUserCreationException
    {
        public MassUserCreationErrorTypeEnum ErrorType { get; set; }
        public string Message { get; set; }
    }

    public enum MassUserCreationErrorTypeEnum
    {
        GeneralError,
        FileIsEmpty,
        FileFormatIsNotCorrect,
        FileExceedLimitRecord,
        FileTemplateIsInvalid,
        UserRowEmptyException
    }
}
