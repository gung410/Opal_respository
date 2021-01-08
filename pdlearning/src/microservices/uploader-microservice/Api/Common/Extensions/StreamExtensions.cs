using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Uploader.Common.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> ToByteArrayAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            MemoryStream ms = new();
            await stream.CopyToAsync(ms, cancellationToken);
            return ms.ToArray();
        }
    }
}
