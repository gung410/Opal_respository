using Newtonsoft.Json.Linq;

namespace cxOrganization.Business.JsonAnalyzer
{
    public interface IImportSchoolAnalyzer
    {
        dynamic GetSchoolDataState(string schoolData, int ownerid, int customerId);
        dynamic AnalyzeSchoolState(string schoolDataState, int ownerid, int customerId);
        dynamic ExecuteSchoolCommand(string commands, int currentOwnerId, int currentCustomerId);
    }
}