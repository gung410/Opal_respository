using LearnerApp.Common;
using Newtonsoft.Json;

namespace LearnerApp.Models
{
    public class AttributionElement
    {
        public string Id { get; set; }

        public string CourseId { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Source { get; set; }

        public CopyrightLicenseType LicenseType
        {
            set
            {
                LicenseTypeString = value switch
                {
                    CopyrightLicenseType.Perpetual => "Perpetual",
                    CopyrightLicenseType.SubscribedForLimitedPeriod => "Subscribed For Limited Period",
                    CopyrightLicenseType.FreeToUse => "Free To Use",
                    CopyrightLicenseType.CreativeCommons => "Creative Commons",
                    _ => string.Empty,
                };
            }
        }

        [JsonIgnore]
        public string LicenseTypeString { get; set; }
    }
}
