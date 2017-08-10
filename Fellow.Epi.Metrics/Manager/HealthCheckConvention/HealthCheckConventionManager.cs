using System;
using System.Collections.Generic;
using Metrics;

namespace Fellow.Epi.Metrics.Manager.HealthCheckConvention
{
    class HealthCheckConventionManager : IHealthCheckConventionManager
    {
        private static readonly IList<string> Checks = new List<string>();

        public void IncludeHealthCheck(string name, Func<bool> check)
        {
            //Wrap it in a delegate
            Func<HealthCheckResult> healthCheck = () =>
            {
                bool success = check();

                if (!success)
                    return HealthCheckResult.Unhealthy();

                return HealthCheckResult.Healthy();
            };

            HealthChecks.RegisterHealthCheck(name, healthCheck);

            Checks.Add(name);
        }

        public bool IsHealthCheckIncluded(string name)
        {
            return Checks.Contains(name);
        }
    }
}
