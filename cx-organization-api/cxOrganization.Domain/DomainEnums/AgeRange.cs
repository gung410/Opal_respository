using System.ComponentModel;

namespace cxOrganization.Domain.Enums
{
    public enum AgeRange
    {
        [Description("Under 20")]
        UnderTwenty = 0,
        [Description("20-29")]
        Twenties = 20,
        [Description("30-39")]
        Thirties = 30,
        [Description("40-49")]
        Forties = 40,
        [Description("50 And greater")]
        FiftyAndGreater = 50
    }
}
