using Microsoft.AspNetCore.Http;
using System;

namespace cxOrganization.Domain.Dtos.Users
{
    public class MassUserCreationValidationContract
    {
        public IFormFile File { get; set; }
    }
}
