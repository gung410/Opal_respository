using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using cxOrganization.Domain.Enums;

namespace cxOrganization.Business.CandidateList
{
    public class CandidateListArguments
    {

        public CandidateListArguments()
        {
            FallbackLocale = "en-US";
        }
        /// <summary>
        /// List of identity of candidates
        /// </summary>
        public List<long> CandidateIds { get; set; }

        /// <summary>
        /// List of extId of candidates
        /// </summary>
        public List<string> CandidateExtIds { get; set; }

        /// <summary>
        /// List code of job matches
        /// </summary>
        public List<string> JobCodes { get; set; }

        /// <summary>
        /// List gender of candidates
        /// </summary>
        public List<Gender> Genders { get; set; }

        /// <summary>
        /// List ranges of age
        /// </summary>
        public List<AgeRange> AgeRanges { get; set; }


        /// <summary>
        /// List id of connection source with type Corporate
        /// </summary>
        public List<long> CorporateIds { get; set; }


        /// <summary>
        /// List id of connection source with type CV
        /// </summary>
        public List<long> CvIds { get; set; }

        /// <summary>
        /// List id of connection source with type JobPosistion
        /// </summary>
        public List<long> JobIds { get; set; }

        /// <summary>
        /// List id of connection source with type Event
        /// </summary>
        public List<long> EventIds { get; set; }

        /// <summary>
        /// Include jobmatch information in candidate list
        /// </summary>
        public bool IncludeAssessment { get; set; }

        /// <summary>
        ///Include assessment information in candidate list 
        /// </summary>
        public bool IncludeJobmatch { get; set; }

        /// <summary>
        ///Include cv completeness information in candidate list 
        /// </summary>
        public bool IncludeCvCompleteness { get; set; }


        /// <summary>
        /// The maximum item for each page of candidate list
        /// </summary>
        public int? PageSize { get; set; }


        /// <summary>
        /// The index of page for getting
        /// </summary>
        public int? PageIndex { get; set; }

        /// <summary>
        /// A language code for getting display text
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// A language code for getting display text
        /// </summary>
        public string FallbackLocale { get; set; }

        /// <summary>
        /// The field which is used for sorting  <see cref="CandidateListSortField"/>
        /// </summary>
        public CandidateListSortField? SortField { get; set; }


        /// <summary>
        /// The sort order <see cref="SortOrder"/>
        /// </summary>
        public SortOrder? SortOrder { get; set; }

        /// <summary>
        /// The referrer token
        /// </summary>
        [Required]
        public string ReferrerToken { get; set; }

        /// <summary>
        /// The value which the completeness should belong to
        /// </summary>
        public List<CompletenessRange> CvCompletenessRanges { get; set; }

        /// <summary>
        /// List of activity extids of profile to get assessment and value. If value is not set, it will get from config
        /// </summary>
        public List<string> ProfileActivityExtIds { get; set; }

        /// <summary>
        /// List of activity extids of profile to get assessment fro joobmatch . If value is not set, it will get from config
        /// </summary>  
        public List<string> JobmatchActivityExtIds { get; set; }
    }
       
}