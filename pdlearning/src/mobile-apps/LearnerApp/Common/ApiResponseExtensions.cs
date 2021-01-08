using LearnerApp.Models;

namespace LearnerApp.Common
{
    public static class ApiResponseExtensions
    {
        public static bool HasEmptyResult<T>(this ApiResponse<T> response)
        {
            return response == null || response.Payload == null;
        }
    }
}
