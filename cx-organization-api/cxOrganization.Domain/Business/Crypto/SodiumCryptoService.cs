using Sodium;
using System;
using System.Text;

namespace cxOrganization.Domain.Business.Crypto
{
    public class SodiumCryptoService : ICryptoService
    {
        private readonly CryptoSetting _cryptoSetting;

        public SodiumCryptoService(CryptoSetting cryptoSetting)
        {
            _cryptoSetting = cryptoSetting;
        }

        public string EncryptToString(string message)
        {
            return Convert.ToBase64String(SecretBox.Create(message, _cryptoSetting.GetNonceAsBytes(), _cryptoSetting.GetKeyAsBytes()));
        }

        public string DecryptToString(string encryptedMessage)
        {
            var decryptedTextBytes = SecretBox.Open(Convert.FromBase64String(encryptedMessage), _cryptoSetting.GetNonceAsBytes(), _cryptoSetting.GetKeyAsBytes());
            return Encoding.UTF8.GetString(decryptedTextBytes);
        }
    }
}
