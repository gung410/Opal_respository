namespace Microservice.WebinarAutoscaler.Common.Extensions
{
    public static class StringExtension
    {
        public static string ConvertIpAddressToPattern(this string value)
        {
            return value?.Replace(".", string.Empty);
        }
    }
}
