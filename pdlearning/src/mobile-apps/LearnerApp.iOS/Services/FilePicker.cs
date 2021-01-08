using LearnerApp.iOS.Services;
using MobileCoreServices;

[assembly: Xamarin.Forms.Dependency(typeof(FilePicker))]

namespace LearnerApp.iOS.Services
{
    public class FilePicker : IFilePicker
    {
        public string[] GetMineTypeForCannotParticipate()
        {
            string[] mineTypes = { UTType.JPEG, UTType.PNG, UTType.PDF, "org.openxmlformats.wordprocessingml.document" };

            return mineTypes;
        }
    }
}
