using cxOrganization.Domain.Business.Crypto;
using cxOrganization.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace cxOrganization.Domain.Security.User
{
    public class UserCryptoService : IUserCryptoService
    {
        private readonly ILogger _logger;
        private readonly AwsKmsEncyption _kmsOptions;
        private readonly AppSettings _appSettings;
        private readonly IEnumerable<ICryptoService> _cryptoServices;
        private readonly string _hashKey;
        public UserCryptoService(
            ILogger<UserCryptoService> logger,
            IOptions<AppSettings> options, IOptions<AwsKmsEncyption> kmsOptions,
            IEnumerable<ICryptoService> cryptoServices)
        {
            _logger = logger;
            _kmsOptions = kmsOptions.Value;
            _appSettings = options.Value;
            _cryptoServices = cryptoServices;
            _hashKey ??= GetHashKeyValue(_kmsOptions);
        }

        private string GetHashKeyValue(AwsKmsEncyption kmsOptions)
        {
            if (kmsOptions.Enabled)
            {
                var cryptoService = _cryptoServices.FirstOrDefault(x => x.GetType() == typeof(AwsKmsCryptoService));
                return cryptoService.DecryptToString(kmsOptions.EncryptedHashKey);
            }
            return string.Empty;
        }

        public string EncryptSSN(string ssn)
        {
            ICryptoService cryptoService;
            if (_kmsOptions.Enabled)
                cryptoService = _cryptoServices.FirstOrDefault(x => x.GetType() == typeof(AwsKmsCryptoService));
            else
                cryptoService = _cryptoServices.FirstOrDefault(x => x.GetType() == typeof(SodiumCryptoService));
            if (string.IsNullOrEmpty(ssn)) return string.Empty;
            return _appSettings.EncryptSSN ? cryptoService.EncryptToString(ssn) : ssn;
        }

        public string ComputeHashSsn(string ssn)
        {
            // Create a SHA256 HMAC
            using HMACSHA256 hash = new HMACSHA256(Encoding.UTF8.GetBytes(_hashKey));
            // ComputeHash - returns byte array  
            var hashValue = string.Concat(ssn);
            byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(hashValue));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        private string GenerateNearlyRamdomSalt(string pickedString)
        {
            pickedString ??= string.Empty;
            return pickedString.PadLeft(8, 'x');
        }


        public string DecryptSSN(string ssn)
        {
            if (string.IsNullOrEmpty(ssn)) return string.Empty;
            if (!_appSettings.EncryptSSN)
            {
                return ssn;
            }
            ICryptoService cryptoService;
            //using Sodium, the lengh will not surpass 60 char length
            if (_kmsOptions.Enabled && ssn.Length > 60)
            {
                cryptoService = _cryptoServices.FirstOrDefault(x => x.GetType() == typeof(AwsKmsCryptoService));
            }
            else
            {
                cryptoService = _cryptoServices.FirstOrDefault(x => x.GetType() == typeof(SodiumCryptoService));
            }
            // TODO: Remove this try catch block once all the SSN in the database has been encrypted using this service.
            try
            {
                return cryptoService.DecryptToString(ssn);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Cannot decrypt the ssn string '{ssn}'.");
                return ssn;
            }
        }
    }
}

