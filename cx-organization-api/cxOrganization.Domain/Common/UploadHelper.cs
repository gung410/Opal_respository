using cxOrganization.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Common
{
    public static class UploadHelper
    {
        public static string GetStorageFullFilePath(char filePathDelimiter, string importStorageFolder, string subfolder, int ownerId, int customerId, string fileName)
        {
            var folder =
                $"{importStorageFolder.Trim(filePathDelimiter)}{filePathDelimiter}{subfolder.Trim(filePathDelimiter)}";

            var fullFilePath = $"{folder}{filePathDelimiter}{ownerId}_{customerId}{filePathDelimiter}{fileName}".Trim(filePathDelimiter);

            return fullFilePath;
        }
    }
}
