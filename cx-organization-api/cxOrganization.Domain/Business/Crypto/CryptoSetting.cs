using cxOrganization.Domain.Settings;
using System.Text;

namespace cxOrganization.Domain.Business.Crypto
{
    public class CryptoSetting
    {
        private byte[] _nonceAsBytes;
        private byte[] _keyAsBytes;
        public readonly AwsKmsEncyption AwsKmsEncyption;

        public CryptoSetting(string nonce, string key, AwsKmsEncyption awsKmsEncyption)
        {
            Nonce = nonce;
            Key = key;
            AwsKmsEncyption = awsKmsEncyption;
        }

        public string Nonce { get; private set; }
        public string Key { get; private set; }

        public byte[] GetNonceAsBytes()
        {
            if (_nonceAsBytes == null)
            {
                _nonceAsBytes = Encoding.UTF8.GetBytes(Nonce);
            }
            return _nonceAsBytes;
        }

        public byte[] GetKeyAsBytes()
        {
            if (_keyAsBytes == null)
            {
                _keyAsBytes = Encoding.UTF8.GetBytes(Key);
            }
            return _keyAsBytes;
        }
    }
}
