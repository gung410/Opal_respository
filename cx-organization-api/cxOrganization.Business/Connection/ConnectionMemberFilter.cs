using System;
using System.Collections.Generic;
using cxOrganization.Domain.Enums;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.Connection
{
    public class ConnectionMemberFilter
    {
        public List<ConnectionType> ConnectionTypes { get; set; }
        public List<ArchetypeEnum> SourceArchetypes { get; set; }
        public List<int> SourceIds { get; set; }
        public List<string> SourceExtIds { get; set; }
        public List<string> SourceReferrerTokens { get; set; }
        public List<string> SourceReferrerResources { get; set; }
        public List<ArchetypeEnum> SourceReferrerArchetypes { get; set; }
        public List<EntityStatusEnum> SourceStatuses { get; set; }
        public Dictionary<ConnectionType, List<int>> OtherTypeSourceIds { get; set; }
        public List<ArchetypeEnum> MemberArchetypes { get; set; }
        public List<int> MemberIds { get; set; }
        public List<string> MemberExtIds { get; set; }
        public List<EntityStatusEnum> MemberStatuses { get; set; }
        public List<string> MemberReferrerTokens { get; set; }
        public List<string> MemberReferrerResources { get; set; }
        public List<ArchetypeEnum> MemberReferrerArchetypes { get; set; }
        public List<AgeRange> MemberAgeRanges { get; set; }
        public List<Gender> MemberGenders { get; set; }
        public DateTime? ValidFromBefore { get; set; }
        public DateTime? ValidFromAfter { get; set; }
        public DateTime? ValidToBefore { get; set; }
        public DateTime? ValidToAfter { get; set; }
        public int TopItems { get; set; }
        public bool DistinctByUser { get; set; }
        public string MemberSearchKey { get; set; }
    }
}