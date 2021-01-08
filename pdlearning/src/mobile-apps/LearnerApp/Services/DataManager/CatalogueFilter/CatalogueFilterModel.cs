using System.Collections.Generic;
using System.Linq;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Models.PdCatelogue;
using LearnerApp.Models.Search;

namespace LearnerApp.Services.DataManager.CatalogueFilter
{
    public class CatalogueFilterModel
    {
        public MetadataTag TypePDActivitySelect { get; set; }

        public MetadataTag ModeOfLearningSelect { get; set; }

        public List<MetadataTag> PdoCategorySelect { get; set; } = new List<MetadataTag>();

        public List<MetadataTag> ServiceSchemesSelect { get; set; } = new List<MetadataTag>();

        public List<MetadataTag> DevelopmentalRolesSelect { get; set; } = new List<MetadataTag>();

        public List<MetadataTag> TeachingLevelSelect { get; set; } = new List<MetadataTag>();

        public List<MetadataTag> SubjectSelect { get; set; } = new List<MetadataTag>();

        public List<MetadataTag> LearningFrameworkSelect { get; set; } = new List<MetadataTag>();

        public List<MetadataTag> LearningAreaSelect { get; set; } = new List<MetadataTag>();

        public List<MetadataTag> LearningDimensionSelect { get; set; } = new List<MetadataTag>();

        public MetadataTag NatureCourseSelect { get; set; }

        public MetadataTag LearningSubAreaSelect { get; set; }

        public MetadataTag CourseLevelSelect { get; set; }

        public JobDesignation JobDesignation { get; set; }

        public List<CommunitySearchTypeEnum> CommunityTypes { get; set; } = new List<CommunitySearchTypeEnum>();

        public AttachmentTypeEnum? AttachmentType { get; set; }

        public bool IsEmpty()
        {
            return
                TypePDActivitySelect == null
                && ModeOfLearningSelect == null
                && NatureCourseSelect == null
                && LearningSubAreaSelect == null
                && CourseLevelSelect == null
                && AttachmentType == null
                && JobDesignation == null
                && PdoCategorySelect.IsNullOrEmpty()
                && ServiceSchemesSelect.IsNullOrEmpty()
                && DevelopmentalRolesSelect.IsNullOrEmpty()
                && TeachingLevelSelect.IsNullOrEmpty()
                && SubjectSelect.IsNullOrEmpty()
                && LearningFrameworkSelect.IsNullOrEmpty()
                && LearningAreaSelect.IsNullOrEmpty()
                && LearningDimensionSelect.IsNullOrEmpty()
                && CommunityTypes.IsNullOrEmpty();
        }

        public string[] GetTagsIds()
        {
            return PdoCategorySelect
                .Concat(ServiceSchemesSelect)
                .Concat(DevelopmentalRolesSelect)
                .Concat(TeachingLevelSelect)
                .Concat(SubjectSelect)
                .Concat(LearningFrameworkSelect)
                .Concat(LearningAreaSelect)
                .Concat(LearningDimensionSelect)
                .Concat(new[]
                {
                    TypePDActivitySelect,
                    ModeOfLearningSelect,
                    NatureCourseSelect,
                    LearningSubAreaSelect,
                    CourseLevelSelect,
                })
                .Where(x => x != null)
                .Select(x => x.TagId)
                .ToArray();
        }
    }
}
