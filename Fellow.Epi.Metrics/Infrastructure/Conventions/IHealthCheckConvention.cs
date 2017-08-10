using Fellow.Epi.Metrics.Manager.HealthCheckConvention;

namespace Fellow.Epi.Metrics.Infrastructure.Conventions
{
    public interface IHealthCheckConvention
    {
        void Apply(IHealthCheckConventionManager healthCheckConventionManager);
    }
}
