using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Settings
{
    public class AwsKmsEncyption
    {
        public bool Enabled { get; set; }
        public string AwsAccessKey { get; set; }
        public string AwsSecretKey { get; set; }
        public string KmsArn { get; set; }
        public string EncryptedHashKey { get; set; }
    }
}
