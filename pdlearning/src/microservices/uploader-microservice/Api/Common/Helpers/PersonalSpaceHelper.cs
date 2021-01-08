namespace Microservice.Uploader.Common.Helpers
{
    public class PersonalSpaceHelper
    {
        /// <summary>
        /// Convert Gigabyte to Bytes.
        /// </summary>
        /// <param name="input">Gigabyte number.</param>
        /// <returns>Bytes.</returns>
        public static double ConvertGigabyteToBytes(int input)
        {
            return double.Parse(input.ToString()) * 1024 * 1024 * 1024;
        }
    }
}
