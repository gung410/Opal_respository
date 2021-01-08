using System;
using LearnerApp.Services.ExceptionHandler;
using UIKit;

namespace LearnerApp.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        public static void Main(string[] args)
        {
#if DEBUG
            UIApplication.Main(args, null, "AppDelegate");
#else
            try
            {
                UIApplication.Main(args, null, "AppDelegate");
            }
            catch (Exception ex)
            {
                new ExceptionHandler().HandleException(ex);
            }
#endif
        }
    }
}
