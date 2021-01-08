using System.Collections.Generic;
using Microservice.LnaForm.Application.Models;

namespace Microservice.LnaForm.Application.RequestDtos
{
    public class ImportFormRequest
    {
       public List<ImportFormModel> FormWithQuestionsSections { get; set; }
    }
}
