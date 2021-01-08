using System;
using LearnerApp.Common;

namespace LearnerApp.Models.MyLearning
{
    public class MyDigitalContentDetails
    {
        private string _originalObjectId;

        public string FileExtension { get; set; }

        public MyDigitalContentType Type { get; set; }

        public string Id { get; set; }

        public string OriginalObjectId
        {
            get
            {
                if (_originalObjectId == Guid.Empty.ToString())
                {
                    _originalObjectId = Id;
                }

                return _originalObjectId;
            }

            set
            {
                _originalObjectId = value;
            }
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public long FileSize { get; set; }

        public string FileLocation { get; set; }

        public string FileName { get; set; }

        public string HtmlContent { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset ChangedDate { get; set; }

        public string OwnerId { get; set; }

        public CopyrightOwnership Ownership { get; set; }

        public CopyrightLicenseType LicenseType { get; set; }

        public string TermsOfUse { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public CopyrightLicenseTerritory LicenseTerritory { get; set; }

        public string Publisher { get; set; }

        public string FileType { get; set; }

        public bool IsAllowReusable { get; set; }

        public bool IsAllowDownload { get; set; }

        public bool IsAllowModification { get; set; }

        public string AcknowledgementAndCredit { get; set; }

        public string Remarks { get; set; }

        public AttributionElement[] AttributionElements { get; set; }

        public string PrimaryApprovingOfficerId { get; set; }

        public string AlternativeApprovingOfficerId { get; set; }

        public bool IsSearchableInCatalogue { get; set; }

        public string SignedUrl { get; set; }
    }
}
