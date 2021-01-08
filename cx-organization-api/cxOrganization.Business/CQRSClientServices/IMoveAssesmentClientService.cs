namespace cxOrganization.Business.CQRSClientServices
{
    public interface IMoveAssesmentClientService
    {
        string ExecuteAssessmentCommand(string schoolCommands, int ownerid, int customerId);
        string GetAssessmentCommands(string schoolStates, int ownerid, int customerId);
        string GetAssessmentStates(string school, int ownerid, int customerId);
    }
}