using System.IO;

namespace cxOrganization.Domain.Extensions
{
    public static class StreamExtension
    {
        public static void ResetPosition(this Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
        }
    }
}
