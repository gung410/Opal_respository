using System.Collections.Generic;
using System.Linq;

namespace cxOrganization.Business.CandidateList
{
    public static class CandidateListSorting
    {
        public static IEnumerable<CandidateListItem> SortCandidateListItemsDescending(List<CandidateListItem> candidateListItems, CandidateListSortField sortField)
        {
           

            switch (sortField)
            {
                case CandidateListSortField.JobMatch:
                    return candidateListItems.OrderByDescending(c => c.JobmatchTotalRate).ThenBy(c => c.FullName);
                case CandidateListSortField.ConnectedDate:
                    return candidateListItems.OrderByDescending(c => c.Connected).ThenBy(c => c.FullName);
                case CandidateListSortField.FirstName:
                    return candidateListItems.OrderByDescending(c => c.FirstName);
                case CandidateListSortField.LastName:
                    return candidateListItems.OrderByDescending(c => c.LastName);
                case CandidateListSortField.Age:
                    return candidateListItems.OrderBy(c => c.DateOfBirth);
                case CandidateListSortField.CvCompletenessStatus:
                    return candidateListItems.OrderByDescending(c => c.CvCompleteness);
            }
            return candidateListItems;
        }

        public static IEnumerable<CandidateListItem> SortCandidateListItemsAscending(List<CandidateListItem> candidateListItems, CandidateListSortField sortField)
        {
            switch (sortField)
            {
                
                case CandidateListSortField.JobMatch:
                    return candidateListItems.OrderBy(c => c.JobmatchTotalRate).ThenBy(c => c.FullName);
                case CandidateListSortField.ConnectedDate:
                    return candidateListItems.OrderBy(c => c.Connected).ThenBy(c => c.FullName);
                case CandidateListSortField.FirstName:
                    return candidateListItems.OrderBy(c => c.FirstName);
                case CandidateListSortField.LastName:
                    return candidateListItems.OrderBy(c => c.LastName);
                case CandidateListSortField.Age:
                    return candidateListItems.OrderByDescending(c => c.DateOfBirth);
                case CandidateListSortField.CvCompletenessStatus:
                    return candidateListItems.OrderBy(c => c.CvCompleteness);
            }
            return candidateListItems;
        }

        public static List<CandidateListItem> SortCandidateListItems(List<CandidateListItem> candidateListItems, CandidateListSortField sortField, SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.Ascending)
                return CandidateListSorting.SortCandidateListItemsAscending(candidateListItems, sortField)
                    .ToList();
            //if sortOrder is not set, default order is Descending
            return CandidateListSorting.SortCandidateListItemsDescending(candidateListItems, sortField)
                .ToList();
        }

        public static List<CandidateListMemberDto> SortCandidateListMembers(List<CandidateListMemberDto> candidateListMembers, CandidateListSortField sortField, SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.Ascending)
                return SortConnectionMembersAscending(candidateListMembers, sortField).ToList();
            return SortConnectionMembersDescending(candidateListMembers, sortField).ToList();


        }
        public static IEnumerable<CandidateListMemberDto> SortConnectionMembersDescending(List<CandidateListMemberDto> connectionMembers, CandidateListSortField sortField)
        {
            switch (sortField)
            {
                case CandidateListSortField.FirstName:
                    return connectionMembers.OrderByDescending(c => c.ConnectionMember.FirstName);
                case CandidateListSortField.LastName:
                    return connectionMembers.OrderByDescending(c => c.ConnectionMember.LastName);
                case CandidateListSortField.ConnectedDate:
                    return connectionMembers.OrderByDescending(c => c.ConnectionMember.Created).ThenBy(c => string.Format("{0} {1}", c.ConnectionMember.FirstName, c.ConnectionMember.LastName));
                case CandidateListSortField.Age:
                    return connectionMembers.OrderBy(c => c.ConnectionMember.DateOfBirth);
                case CandidateListSortField.CvCompletenessStatus:
                    return connectionMembers.OrderByDescending(c => c.CvCompleteness);
                default:
                    return connectionMembers;
            }
        }
        public static IEnumerable<CandidateListMemberDto> SortConnectionMembersAscending(List<CandidateListMemberDto> connectionMembers, CandidateListSortField sortField)
        {
            switch (sortField)
            {
                case CandidateListSortField.FirstName:
                    return connectionMembers.OrderBy(c => c.ConnectionMember.FirstName);
                case CandidateListSortField.LastName:
                    return connectionMembers.OrderBy(c => c.ConnectionMember.LastName);
                case CandidateListSortField.ConnectedDate:
                    return connectionMembers.OrderBy(c => c.ConnectionMember.Created).ThenBy(c => string.Format("{0} {1}", c.ConnectionMember.FirstName, c.ConnectionMember.LastName));
                case CandidateListSortField.Age:
                    return connectionMembers.OrderByDescending(c => c.ConnectionMember.DateOfBirth);
                case CandidateListSortField.CvCompletenessStatus:
                    return connectionMembers.OrderBy(c => c.CvCompleteness);
                default:
                    return connectionMembers;
            }
        }
    }
}