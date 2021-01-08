using System;

namespace cxOrganization.Business.CandidateList
{
    public static class JobFamilyMapping
    {

        public static MatchRate GetMatchRate(string ipLetters, JobFamilyEnum jobFamily, out string matchLetter)
        {
            var rating = MatchRate.Neutral;
            matchLetter = ipLetters;
            switch (jobFamily)
            {
                case JobFamilyEnum.Sales:
                    //Sales and Related	
                    if (ipLetters.StartsWith("EC"))
                    {
                        rating = MatchRate.Good;
                        matchLetter = "EC";
                    }
                    else if (ipLetters.StartsWith("ER"))
                    {
                        rating = MatchRate.Decent;
                        matchLetter = "ER";

                    }
                    else if (ipLetters.StartsWith("CE"))
                    {
                        rating = MatchRate.Decent;
                        matchLetter = "CE";

                    }
                    else if (ipLetters.StartsWith("E"))
                    {
                        rating = MatchRate.Decent;
                        matchLetter = "E";

                    }

                    break;
                case JobFamilyEnum.Engineering:
                    //Architecture and Engineering
                    if (ipLetters.StartsWith("IR"))
                    {
                        rating = MatchRate.Good;
                        matchLetter = "IR";

                    }
                    else if (ipLetters.StartsWith("RI"))
                    {
                        rating = MatchRate.Good;
                        matchLetter = "RI";
                    }

                    break;
                case JobFamilyEnum.ArtsDesignMedia:
                    //Arts, Design, Entertainment, Sports, and Media	
                    if (ipLetters.StartsWith("AE"))
                    {
                        rating = MatchRate.Good;
                        matchLetter = "AE";

                    }
                    else if (ipLetters.StartsWith("EA"))
                    {
                        rating = MatchRate.Good;
                        matchLetter = "EA";

                    }
                    else if (ipLetters.StartsWith("AR"))
                    {
                        matchLetter = "AR";
                        rating = MatchRate.Decent;
                    }
                    else if (ipLetters.StartsWith("EC"))
                    {
                        matchLetter = "EC";
                        rating = MatchRate.Decent;
                    }
                    else if (ipLetters.StartsWith("AI"))
                    {
                        matchLetter = "AI";
                        rating = MatchRate.Decent;
                    }
                    break;

                case JobFamilyEnum.FinHRBizOps:
                    //Business and Financial Operations
                    if (ipLetters.StartsWith("CE"))
                    {
                        matchLetter = "CE";
                        rating = MatchRate.Good;
                    }
                    else if (ipLetters.StartsWith("EC"))
                    {
                        matchLetter = "EC";
                        rating = MatchRate.Good;
                    }
                    else if (ipLetters.StartsWith("IE"))
                    {
                        matchLetter = "IE";
                        rating = MatchRate.Decent;
                    }
                    else if (ipLetters.StartsWith("CI"))
                    {
                        matchLetter = "CI";
                        rating = MatchRate.Decent;
                    }
                    else if (ipLetters.StartsWith("SA"))
                    {
                        matchLetter = "SA";
                        rating = MatchRate.Decent;
                    }
                    break;
                case JobFamilyEnum.TeachingSocialSvc:
                    //Community and Social Service
                    if (ipLetters.StartsWith("SI"))
                    {
                        matchLetter = "SI";
                        rating = MatchRate.Good;
                    }
                    else if (ipLetters.StartsWith("SE"))
                    {
                        matchLetter = "SE";
                        rating = MatchRate.Good;
                    }
                    else if (ipLetters.StartsWith("IS"))
                    {
                        matchLetter = "IS";
                        rating = MatchRate.Decent;
                    }
                    else if (ipLetters.StartsWith("ES"))
                    {
                        matchLetter = "ES";
                        rating = MatchRate.Decent;
                    }
                    else if (ipLetters.StartsWith("S"))
                    {
                        matchLetter = "S";
                        rating = MatchRate.Decent;
                    }
                    break;

                case JobFamilyEnum.ITAndAnalytics:
                    //Computer and Mathematical
                    if (ipLetters.StartsWith("IC"))
                    {
                        matchLetter = "IC";
                        rating = MatchRate.Good;
                    }
                    else if (ipLetters.StartsWith("CI"))
                    {
                        matchLetter = "CI";
                        rating = MatchRate.Good;
                    }
                    else if (ipLetters.StartsWith("EC"))
                    {
                        matchLetter = "EC";
                        rating = MatchRate.Decent;
                    }
                    else if (ipLetters.StartsWith("IE"))
                    {
                        matchLetter = "IE";
                        rating = MatchRate.Decent;
                    }
                    else if (ipLetters.StartsWith("EI"))
                    {
                        matchLetter = "EI";
                        rating = MatchRate.Decent;
                    }
                    else if (ipLetters.StartsWith("IR"))
                    {
                        matchLetter = "IR";
                        rating = MatchRate.Decent;
                    }

                    break;
                case JobFamilyEnum.Management:
                    //Management
                    if (ipLetters.StartsWith("EC"))
                    {
                        matchLetter = "EC";
                        rating = MatchRate.Good;
                    }
                    else if (ipLetters.StartsWith("ES"))
                    {
                        matchLetter = "ES";
                        rating = MatchRate.Decent;
                    }
                    else if (ipLetters.StartsWith("CE"))
                    {
                        matchLetter = "CE";
                        rating = MatchRate.Decent;
                    }
                    else if (ipLetters.StartsWith("E"))
                    {
                        matchLetter = "E";
                        rating = MatchRate.Decent;
                    }

                    break;
                default:
                    rating = MatchRate.Neutral;
                    break;
            }
            return rating;
        }

        public static JobFamilyEnum GetJobFamilyEnum(string jobFamilyName)
        {
            jobFamilyName = jobFamilyName.ToLower();
            switch (jobFamilyName)
            {
                case "sales":
                    return JobFamilyEnum.Sales;
                case "arch engr":
                    return JobFamilyEnum.Engineering;
                case "arts design sports":
                    return JobFamilyEnum.ArtsDesignMedia;
                case "biz ops":
                    return JobFamilyEnum.FinHRBizOps;
                case "community social":
                    return JobFamilyEnum.TeachingSocialSvc;
                case "math":
                    return JobFamilyEnum.ITAndAnalytics;
                case "management":
                    return JobFamilyEnum.Management;
                default:
                    throw new ArgumentException(string.Format("Cannot map JobFamilyName({0})", jobFamilyName));
            }
        }
    }
}
