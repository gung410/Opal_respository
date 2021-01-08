using cxOrganization.Adapter.JobMatch.Models;
using System.Collections.Generic;

namespace cxOrganization.Adapter.JobMatch
{
    public interface IJobMatchAdapter
    {
        List<JobmatchesDto> GetJobmatchesFromRiasecLetters(string correlationId, string riasecLetters, List<ClassificationEnum> classifications, RiasecLetterCombinationEnum combination, string locale);
        List<JobmatchDto> GetJobmatchesFromRiasecLettersByGroupLevel(string correlationId, JobmatchGroupLevel groupLevel, string riasecLetters, List<ClassificationEnum> classifications, RiasecLetterCombinationEnum combination, string locale);

    }
}
