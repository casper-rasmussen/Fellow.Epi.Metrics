using System;

namespace Fellow.Epi.Metrics.Manager.Metric
{
    public interface IMetricManager
    {
        void TimeRequestsForOperation(Action operation, string operationName);

        T TimeRequestsForOperation<T>(Func<T> operation, string operationName);

        bool Healthy();

        string GenerateReport();

        void Reset();
    }
}
