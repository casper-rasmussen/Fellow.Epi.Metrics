using System;
using Castle.DynamicProxy;
using Fellow.Epi.Metrics.Manager.Metric;

namespace Fellow.Epi.Metrics.Infrastructure.Interception
{
    public class ApplyMetricsTimingInterceptor : IInterceptor
    {
        private readonly IMetricManager _metricManager;

        public ApplyMetricsTimingInterceptor(IMetricManager metricManager)
        {
            this._metricManager = metricManager;
        }

        public void Intercept(IInvocation invocation)
        { 
            string operationName = String.Format("{0}_{1}", invocation.TargetType.Name, invocation.Method.Name);

            this._metricManager.TimeRequestsForOperation(() => invocation.Proceed(), operationName);
        }
    }
}
