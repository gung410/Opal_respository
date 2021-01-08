namespace cxOrganization.Domain.Security.User
{
    public interface IUserCryptoService
    {
        string DecryptSSN(string ssn);
        string EncryptSSN(string ssn);
    }
}