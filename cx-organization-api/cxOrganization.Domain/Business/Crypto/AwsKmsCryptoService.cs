using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using cxOrganization.Domain.Settings;
using Microsoft.Extensions.Options;
using Sodium;
using System;
using System.IO;
using System.Text;

namespace cxOrganization.Domain.Business.Crypto
{
    public class AwsKmsCryptoService : ICryptoService
    {
        private readonly AmazonKeyManagementServiceClient _amazonKeyManagementServiceClient;

        private readonly AwsKmsEncyption _awsKmsEncyption;

        public AwsKmsCryptoService(AmazonKeyManagementServiceClient amazonKeyManagementServiceClient, IOptions<AwsKmsEncyption> options)
        {
            _amazonKeyManagementServiceClient = amazonKeyManagementServiceClient;
            _awsKmsEncyption = options.Value;
        }

        public string EncryptToString(string message)
        {
            MemoryStream plaintext = new MemoryStream(Encoding.UTF8.GetBytes(message));

            EncryptRequest encryptRequest = new EncryptRequest()
            {
                KeyId = _awsKmsEncyption.KmsArn,
                Plaintext = plaintext
            };
            var result = _amazonKeyManagementServiceClient.EncryptAsync(encryptRequest).GetAwaiter().GetResult();
            var bytes = result.CiphertextBlob.ToArray();
            return Convert.ToBase64String(bytes);
        }

        public string DecryptToString(string encryptedMessage)
        {
            var bytes = Convert.FromBase64String(encryptedMessage);
            MemoryStream ciphertextBlob = new MemoryStream(bytes);
            // Write ciphertext to memory stream

            DecryptRequest decryptRequest = new DecryptRequest()
            {
                CiphertextBlob = ciphertextBlob
            };
            var plainTextBytes = _amazonKeyManagementServiceClient.DecryptAsync(decryptRequest).GetAwaiter().GetResult().Plaintext.ToArray();
            return Encoding.UTF8.GetString(plainTextBytes);
        }


    }
}
