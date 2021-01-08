using System.IO;

namespace cxOrganization.Domain.Services
{
    public class UserRowEmptyException : IOException
    {
        public UserRowEmptyException() : base()
        {
        }
        public UserRowEmptyException(string message) : base(message)
        {
        }

    }
}
