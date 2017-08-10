using System;
using Metrics;
using Metrics.Core;
using Metrics.MetricData;
using Metrics.Reporters;

namespace Fellow.Epi.Metrics.Manager.Metric
{
    class MetricManager : IMetricManager
    {
        private readonly MetricsContext _metricsContext;

        public MetricManager(MetricsContext metricsContext)
        {
            this._metricsContext = metricsContext;
        }

        public void TimeRequestsForOperation(Action operation, string operationName)
        {
            Timer timer = this._metricsContext.Timer(operationName, Unit.Requests);

            timer.Time(operation);
        }

        public T TimeRequestsForOperation<T>(Func<T> operation, string operationName)
        {
            Timer timer = this._metricsContext.Timer(operationName, Unit.Requests);

            return timer.Time(operation);
        }

        public string GenerateReport()
        {
            MetricsData metricsData = this._metricsContext.DataProvider.CurrentMetricsData;

            return StringReport.RenderMetrics(metricsData, HealthChecks.GetStatus);
        }

        public bool Healthy()
        {
            return HealthChecks.GetStatus().IsHealthy;
        }

        public void Reset()
        {
            this._metricsContext.Advanced.ResetMetricsValues();
        }
    }
}
