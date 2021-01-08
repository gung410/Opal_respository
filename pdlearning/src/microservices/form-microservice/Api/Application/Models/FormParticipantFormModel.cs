using Microservice.Form.Domain.Entities;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Models
{
    public class FormParticipantFormModel
    {
        public FormParticipantFormModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormParticipantFormModel(FormEntity form, FormParticipant formParticipant)
        {
            FormParticipant = formParticipant;
            Form = form;
        }

        public FormParticipant FormParticipant { get; set; }

        public FormEntity Form { get; set; }
    }
}
