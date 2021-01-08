using System;

namespace cxOrganization.Domain.ApiClient
{
    public class CatalogItemDto
    {
        public Guid Id { get; set; }
        public string FullStatement { get; set; }
        public string Note { get; set; }
        public string Type { get; set; }
        public string DisplayText { get; set; }
        public string Code { get; set; }
        public string CodingScheme { get; set; }
        public string AbbreviatedStatement { get; set; }
        public Guid? TypeId { get; set; }
    }
}
