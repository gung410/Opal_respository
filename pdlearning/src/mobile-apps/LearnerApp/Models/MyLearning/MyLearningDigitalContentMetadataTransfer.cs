using System.Collections.Generic;
using System.Linq;
using LearnerApp.Common;

namespace LearnerApp.Models.MyLearning
{
    public class MyLearningDigitalContentMetadataTransfer
    {
        public MyLearningDigitalContentMetadataTransfer(List<MetadataTag> metadataTagging, Resource myDigitalContentMetadata, MyLearningDigitalContentCardData myDigitalContentCardData, bool isDigitalLearningClosed)
        {
            var myDigitalContentDetails = myDigitalContentCardData.MyDigitalContentDetails;
            MyDigitalContentDetails = myDigitalContentDetails;
            ViewsCount = myDigitalContentCardData.MyDigitalContentSummary.ViewsCount;
            DownloadsCount = myDigitalContentCardData.MyDigitalContentSummary.DownloadsCount;
            SharesCount = myDigitalContentCardData.MyDigitalContentSummary.SharesCount;
            IsDigitalLearningClosed = isDigitalLearningClosed;
            TotalLike = myDigitalContentCardData.MyDigitalContentSummary.TotalLike;
            IsLike = myDigitalContentCardData.MyDigitalContentSummary.IsLike;

            if (metadataTagging.IsNullOrEmpty() || myDigitalContentMetadata == null)
            {
                return;
            }

            DescriptionValue = myDigitalContentDetails.Description ?? GlobalSettings.NotAvailable;
            TitleValue = myDigitalContentDetails.Title ?? GlobalSettings.NotAvailable;
            string fileExtension = myDigitalContentDetails.FileExtension ?? "document";
            TypeValue = fileExtension;
            TypeImageValue = $"{fileExtension}.svg";
            UploadDateValue = myDigitalContentDetails.ChangedDate.ToString("dd/MM/yyyy");

            PreRequisties = myDigitalContentMetadata.PreRequisties ?? GlobalSettings.NotAvailable;
            Ownership = myDigitalContentDetails.Ownership switch
            {
                CopyrightOwnership.MoeOwned => "MOE-Owned",
                CopyrightOwnership.MoeOwnedWithLicensedMaterial => "MOE-Owned with Licensed material",
                CopyrightOwnership.MoeCoOwned => "MOE Co-Owned",
                CopyrightOwnership.MoeCoOwnedWithLicensedMaterial => "MOE Co-Owned with Licensed material",
                CopyrightOwnership.LicensedMaterials => "Licensed materials",
                _ => GlobalSettings.NotAvailable
            };

            LicenseType = myDigitalContentDetails.LicenseType switch
            {
                CopyrightLicenseType.Perpetual => "Perpetual Licence",
                CopyrightLicenseType.SubscribedForLimitedPeriod => "Subscribed for a Limited Period",
                CopyrightLicenseType.FreeToUse => "Free to use",
                CopyrightLicenseType.CreativeCommons => "Creative Commons Licence",
                _ => GlobalSettings.NotAvailable
            };

            PermissionsValue = myDigitalContentDetails.TermsOfUse ?? GlobalSettings.NotAvailable;
            CopyrightOwnerValue = myDigitalContentDetails.Publisher ?? GlobalSettings.NotAvailable;
            LicenseTerritory = myDigitalContentDetails.LicenseTerritory switch
            {
                CopyrightLicenseTerritory.Singapore => "Singapore",
                CopyrightLicenseTerritory.SingaporeAndHongKong => "Singapore & HongKong",
                CopyrightLicenseTerritory.Worldwide => "Worldwide",
                _ => GlobalSettings.NotAvailable
            };

            ObjectivesOutcomes = myDigitalContentMetadata.ObjectivesOutCome ?? GlobalSettings.NotAvailable;

            // MainSubjectAreaValue
            if (!myDigitalContentMetadata.Tags.IsNullOrEmpty())
            {
                foreach (var item in myDigitalContentMetadata.Tags)
                {
                    var metadata = metadataTagging.FirstOrDefault(p => p.TagId == item);

                    if (metadata != null)
                    {
                        KeyValuePairs[item] = metadata;
                    }
                }
            }

            var metadataTags = KeyValuePairs.Values.Where(key => key.GroupCode == MetadataTagGroupCourse.MAINSUBJECTAREA);
            var mainSubjectArea = metadataTags.FirstOrDefault(main => main.TagId.Equals(myDigitalContentMetadata.MainSubjectAreaTagId));
            MainSubjectAreaValue = mainSubjectArea?.DisplayText ?? GlobalSettings.NotAvailable;
        }

        public string PreRequisties { get; set; }

        public string Ownership { get; set; }

        public string LicenseType { get; set; }

        public string LicenseTerritory { get; set; }

        public string ObjectivesOutcomes { get; set; }

        public string MainSubjectAreaValue { get; set; }

        public string PermissionsValue { get; set; }

        public string CopyrightOwnerValue { get; set; }

        public string DescriptionValue { get; set; }

        public string TitleValue { get; set; }

        public string TypeValue { get; set; }

        public string TypeImageValue { get; set; }

        public string UploadDateValue { get; set; }

        public int ViewsCount { get; set; }

        public int DownloadsCount { get; set; }

        public int SharesCount { get; set; }

        public int TotalLike { get; set; }

        public bool IsDigitalLearningClosed { get; set; }

        public bool IsLike { get; set; }

        public Dictionary<string, MetadataTag> KeyValuePairs { get; set; } = new Dictionary<string, MetadataTag>();

        public MyDigitalContentDetails MyDigitalContentDetails { get; set; }
    }
}
