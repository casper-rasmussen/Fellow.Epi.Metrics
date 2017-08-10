using Castle.DynamicProxy;
using Fellow.Epi.Metrics.Infrastructure.Interception;
using Fellow.Epi.Metrics.Manager.HealthCheckConvention;
using Fellow.Epi.Metrics.Manager.Metric;
using Metrics;
using StructureMap.Configuration.DSL;

namespace Fellow.Epi.Metrics.Bootstrapper
{
	public class FellowEpiMetricsImplementationBootstrapper : Registry
	{
		public FellowEpiMetricsImplementationBootstrapper()
		{
			this.For<IMetricManager>().Use<MetricManager>();
            this.For<IHealthCheckConventionManager>().UseIfNone<HealthCheckConventionManager>();

            //Metrics
            this.For<MetricsContext>().Use(Metric.Context("Fellow Metrics"));
        }
	}
}
