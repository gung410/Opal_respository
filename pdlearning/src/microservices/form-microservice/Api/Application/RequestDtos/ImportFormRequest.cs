using System.Collections.Generic;
using Microservice.Form.Application.Models;

namespace Microservice.Form.Application.RequestDtos
{
    public class ImportFormRequest
    {
       public List<ImportFormModel> FormWithQuestionsSections { get; set; }
    }
}
