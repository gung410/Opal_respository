namespace cxOrganization.Domain.Settings
{
    public class LearningCatalogAPISettings
    {
        public string APIBaseUrl { get; set; }
        public CatalogCodeConfiguration CatalogCodes { get; set; }

    }
    public class CatalogCodeConfiguration
    {
        public string OrgUnitType { get; set; }
        public string Designation { get; set; }
    }
}