using System.Text;

namespace cxOrganization.Domain.Business.Crypto
{
    public class CryptoSetting
    {
        private byte[] _nonceAsBytes;
        private byte[] _keyAsBytes;

        public CryptoSetting(string nonce, string key)
        {
            Nonce = nonce;
            Key = key;
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
