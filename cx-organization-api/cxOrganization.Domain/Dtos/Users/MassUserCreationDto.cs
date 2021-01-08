using cxOrganization.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace cxOrganization.Domain.Dtos.Users
{
    public class MassUserCreationDto
    {
        public MassUserCreationDto()
        {
            //RowNumber++;
        }
        public string Salutation { get; set; }
        public string Name { get; set; }
        public string OfficialEmail { get; set; }
        public string Gender { get; set; }
        public string PlaceOfWork { get; set; }
        public string AccountActiveFrom { get; set; }
        public string DateOfExpiryAccount { get; set; }
        public string ReasonForUserAccountRequest { get; set; }
        public string SystemRole { get; set; }
        public string PersonalSpaceLimitation { get; set; }
        public int Number { get; set; }

        private const string NameAndReasonValidationPattern = @"[\""\&\'\<\>\\]";
        //public static int RowNumber { get; set; } = 0;

        public static List<MassUserCreationDto> GenerateRowNumber(List<MassUserCreationDto> massUserCreationDtos)
        {
            var massUserCreations = new List<MassUserCreationDto>();
            int rowNumber = 0;
            foreach (var userCreatinoDto in massUserCreationDtos)
            {
                rowNumber++;
                massUserCreations.Add(new MassUserCreationDto
                {
                    Number = rowNumber,
                    Salutation = userCreatinoDto.Salutation,
                    Name = userCreatinoDto.Name,
                    OfficialEmail = userCreatinoDto.OfficialEmail,
                    Gender = userCreatinoDto.Gender,
                    PlaceOfWork = userCreatinoDto.PlaceOfWork,
                    AccountActiveFrom = userCreatinoDto.AccountActiveFrom,
                    DateOfExpiryAccount = userCreatinoDto.DateOfExpiryAccount,
                    ReasonForUserAccountRequest = userCreatinoDto.ReasonForUserAccountRequest,
                    SystemRole = userCreatinoDto.SystemRole,
                    PersonalSpaceLimitation = userCreatinoDto.PersonalSpaceLimitation,
                }
            );
            }
            return massUserCreations;
        }

        public MassUserCreationResultDto CreateResult(string status)
        {
            return new MassUserCreationResultDto
            {
                Number = Number,
                Salutation = Salutation,
                Name = Name,
                OfficialEmail = OfficialEmail,
                Gender = Gender,
                PlaceOfWork = PlaceOfWork,
                AccountActiveFrom = AccountActiveFrom,
                DateOfExpiryAccount = DateOfExpiryAccount,
                ReasonForUserAccountRequest = ReasonForUserAccountRequest,
                SystemRole = SystemRole,
                PersonalSpaceLimitation = PersonalSpaceLimitation,
                Status = status
            };
        }

        public InvalidMassUserCreationDto CreateInvalidResult(string reason)
        {
            return new InvalidMassUserCreationDto
            {
                Number = Number,
                Salutation = Salutation,
                Name = Name,
                OfficialEmail = OfficialEmail,
                Gender = Gender,
                PlaceOfWork = PlaceOfWork,
                AccountActiveFrom = AccountActiveFrom,
                DateOfExpiryAccount = DateOfExpiryAccount,
                ReasonForUserAccountRequest = ReasonForUserAccountRequest,
                SystemRole = SystemRole,
                PersonalSpaceLimitation = PersonalSpaceLimitation,
                Reason = reason
            };
        }

        public bool IsValidName()
        {
            return !isMatchUnSupportedChars(this.Name);
        }

        public bool IsValidReason()
        {
            return !isMatchUnSupportedChars(this.ReasonForUserAccountRequest);
        }

        private bool isMatchUnSupportedChars(string chars)
        {
            return RegexUtils.isMatchPatternInRangeOfWords(chars, NameAndReasonValidationPattern);
        }

    }

    public class MassUserCreationResultDto : MassUserCreationDto
    {
        public string Status { get; set; } = "";
    }
}
