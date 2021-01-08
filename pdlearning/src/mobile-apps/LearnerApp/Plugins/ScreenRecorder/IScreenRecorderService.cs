using System;
using System.Threading.Tasks;

namespace LearnerApp.Plugins.ScreenRecorder
{
    public interface IScreenRecorderService
    {
        Task StartRecording(ScreenRecorderManager manager, string fileName, Action<string> onRecordTimeout);

        Task<string> StopRecording();

        Task<bool> IsRecording();
    }
}
