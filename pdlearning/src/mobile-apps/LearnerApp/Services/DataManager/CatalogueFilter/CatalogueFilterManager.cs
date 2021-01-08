using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearnerApp.Services.DataManager.CatalogueFilter
{
    public class CatalogueFilterManager
    {
        private const string PropertyKey = "CatalogueFilter";

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static CatalogueFilterManager Current { get; } = new CatalogueFilterManager();

        public CatalogueFilterModel GetCatalogueFilterModel()
        {
            var emptyCatalogueFilter = new CatalogueFilterModel();
            if (!App.Current.Properties.ContainsKey(PropertyKey))
            {
                return emptyCatalogueFilter;
            }

            var catalogueFilterModelObject = App.Current.Properties[PropertyKey] as CatalogueFilterModel;
            if (catalogueFilterModelObject != null)
            {
                return catalogueFilterModelObject;
            }
            else
            {
                return emptyCatalogueFilter;
            }
        }

        public async Task SaveCatalogueFilterModel(CatalogueFilterModel catalogueFilterModel)
        {
            App.Current.Properties[PropertyKey] = catalogueFilterModel;
            await App.Current.SavePropertiesAsync();
        }
    }
}
