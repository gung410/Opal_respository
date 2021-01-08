using NPOI;

namespace cxOrganization.Domain.Exceptions
{
    public class UnsupportedFileExtensionException : UnsupportedFileFormatException
    {
        public UnsupportedFileExtensionException(string message) : base(message)
        {
        }
    }
}
