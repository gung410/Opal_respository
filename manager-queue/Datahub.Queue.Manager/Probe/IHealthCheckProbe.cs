namespace Datahub.Queue.Manager.Probe
{
    interface IHealthCheckProbe
    {
        HealthStatus ExecuteProbe(params string[] probeParameters);
    }
}
