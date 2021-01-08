using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetClassRunsByClassRunCodesRequest
    {
        public List<string> ClassRunCodes { get; set; }
    }
}
