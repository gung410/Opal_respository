namespace cxOrganization.Domain.Business.Crypto
{
    public interface ICryptoService
    {
        string EncryptToString(string message);
        string DecryptToString(string encryptedMessage);

    }
}
