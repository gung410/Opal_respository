using System.Collections.Generic;
using Microservice.Uploader.Application.Models;

namespace Microservice.Uploader.Application.RequestDtos
{
    public class CreatePersonalFilesRequest
    {
        public List<PersonalFileModel> PersonalFiles { get; set; }
    }
}
