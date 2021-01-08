using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable SA1402 // File may only contain a single type
namespace Conexus.Opal.AccessControl.RequestDtos
{
    public class SaveUserRequestDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public int DepartmentId { get; set; }

        public SaveUserRequestDtoIdentity Identity { get; set; }
    }

    public class SaveUserRequestDtoIdentity
    {
        public int Id { get; set; }

        public Guid ExtId { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
