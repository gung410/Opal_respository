using cxOrganization.Domain.Business.Crypto;
using cxOrganization.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace cxOrganization.Domain.Security.User
{
    public class UserCryptoService : IUserCryptoService
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private readonly ICryptoService _cryptoService;

        public UserCryptoService(
            ILogger<UserCryptoService> logger,
            IOptions<AppSettings> options,
            ICryptoService cryptoService)
        {
            _logger = logger;
            _appSettings = options.Value;
            _cryptoService = cryptoService;
        }

        public string EncryptSSN(string ssn)
        {
            if (string.IsNullOrEmpty(ssn)) return string.Empty;
            return _appSettings.EncryptSSN ? _cryptoService.EncryptToString(ssn) : ssn;
        }

        public string DecryptSSN(string ssn)
        {
            if (string.IsNullOrEmpty(ssn)) return string.Empty;

            if (!_appSettings.EncryptSSN)
            {
                return ssn;
            }

            // TODO: Remove this try catch block once all the SSN in the database has been encrypted using this service.
            try
            {
                return _cryptoService.DecryptToString(ssn);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Cannot decrypt the ssn string '{ssn}'.");
                return ssn;
            }
        }
    }
}
