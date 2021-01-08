namespace LearnerApp.Models
{
    public class ErrorHandler
    {
        public InternalError Error { get; set; }

        public string GetErrorContent()
        {
            string errorMessage = string.Empty;

            if (Error != null && !string.IsNullOrEmpty(Error.Message))
            {
                errorMessage = Error.Message;
            }

            return errorMessage;
        }
    }
}
