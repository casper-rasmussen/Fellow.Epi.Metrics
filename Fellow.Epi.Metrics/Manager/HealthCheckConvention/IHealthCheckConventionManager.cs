using System;

namespace Fellow.Epi.Metrics.Manager.HealthCheckConvention
{
    public interface IHealthCheckConventionManager
    {
        void IncludeHealthCheck(string name, Func<bool> check);
        bool IsHealthCheckIncluded(string name);
    }
}
