using NPOI;

namespace cxOrganization.Domain.Exceptions
{
    public class UnsupportFileTemplateException : UnsupportedFileFormatException
    {
        public UnsupportFileTemplateException(string message) : base(message) {}
    }
}
