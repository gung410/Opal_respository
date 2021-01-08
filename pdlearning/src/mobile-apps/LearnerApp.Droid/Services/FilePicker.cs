using LearnerApp.Droid.Services;

[assembly: Xamarin.Forms.Dependency(typeof(FilePicker))]

namespace LearnerApp.Droid.Services
{
    public class FilePicker : IFilePicker
    {
        public string[] GetMineTypeForCannotParticipate()
        {
            string[] mineTypes = { "image/png", "image/jpeg", "application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };

            return mineTypes;
        }
    }
}
