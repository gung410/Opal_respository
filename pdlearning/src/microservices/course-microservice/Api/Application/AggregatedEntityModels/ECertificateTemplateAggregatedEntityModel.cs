using Microservice.Course.Application.AggregatedEntityModels.Abstractions;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.AggregatedEntityModels
{
    public class ECertificateTemplateAggregatedEntityModel : BaseAggregatedEntityModel
    {
        public ECertificateTemplate Template { get; set; }

        public ECertificateLayout Layout { get; set; }

        public static ECertificateTemplateAggregatedEntityModel New(ECertificateTemplate template)
        {
            return new ECertificateTemplateAggregatedEntityModel()
            {
                Template = template
            };
        }

        public ECertificateTemplateAggregatedEntityModel WithLayout(ECertificateLayout layout)
        {
            Layout = layout;

            return this;
        }
    }
}
