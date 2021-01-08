namespace Datahub.Queue.Manager.Probe
{
    public class HealthStatus
    {
        public bool IsAlive { get; set; }
        public string StatusDetails { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public override string ToString()
        {
            if(!string.IsNullOrEmpty(ErrorMessage))
            {
                return ErrorMessage;
            }
            if (!string.IsNullOrEmpty(StatusDetails))
            {
                return StatusDetails;
            }
            return "OK";
        }
    }
}
