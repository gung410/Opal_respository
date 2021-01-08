using System;
using System.Collections.Generic;
using cxOrganization.Adapter.JobMatch.Models;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.CandidateList
{
    public class CandidateListConfig
    {
        public CandidateListConfig()
        {
            JobmatchConfig = new CandidateListJobmatchConfig();
        }
        public List<string> ProfileActivityExtIds { get; set; }
        public List<string> JobmatchActivityExtIds { get; set; }
        public List<int> CandidateListProfileStatusTypeIds { get; set; }
        public string LetterAlternativeExtId { get; set; }
        public List<string> ProfileAlternativeExtIds { get; set; }
        public string ColorLevelGroupTag { get; set; }
        public int CandidateListDefaultPageSize { get; set; }
        public int CandidateListMaxConnectionMembers { get; set; }
        public string RiasecLanguageCode { get; set; }
        public TimeSpan CacheTimeOut { get; set; }
        public bool CacheCandidateList { get; set; }
        public List<EntityStatusEnum> AcceptedMemberStatuses { get; set; }
        public bool IncludeExpiredMember { get; set; }

        public CandidateListJobmatchConfig JobmatchConfig { get; set; }
    }
    public class CandidateListJobmatchConfig
    {
        public bool UseJobmatchService { get; set; }
        public int CacheJobmatchInSeconds { get; set; }
        public ClassificationEnum Classification { get; set; }
        public RiasecLetterCombinationEnum Combination { get; set; }
        public JobmatchGroupLevel GroupLevel { get; set; }

    }
}