namespace Thunder.Platform.EntityFrameworkCore.Logging
{
    public interface IQueryTrackingSource
    {
        string GetAllTrackingInformation();

        void PushTrackingInformation(string trackingInformation);

        string PopTrackingInformation();
    }
}
