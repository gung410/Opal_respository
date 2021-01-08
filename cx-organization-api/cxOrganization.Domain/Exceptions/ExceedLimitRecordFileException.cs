using System.IO;

namespace cxOrganization.Domain.Services
{
    public class ExceedLimitRecordFileException : IOException
    {
        public ExceedLimitRecordFileException() : base()
        {
        }
        public ExceedLimitRecordFileException(string message) : base(message)
        {
        }

    }
}
