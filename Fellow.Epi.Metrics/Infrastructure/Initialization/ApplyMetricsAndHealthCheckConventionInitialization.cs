using System;
using System.Collections.Generic;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging.Compatibility;
using EPiServer.ServiceLocation;
using Fellow.Epi.Metrics.Infrastructure.Conventions;
using Fellow.Epi.Metrics.Manager.HealthCheckConvention;
using Metrics;

namespace Fellow.Epi.Metrics.Infrastructure.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class ApplyHealthCheckConventionsInitialization : IInitializableModule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Initialize(InitializationEngine context)
        {
            //Only allow internal metrics
            Metric.Config.WithInternalMetrics();

            IHealthCheckConventionManager conventionManager = context.Locate.Advanced.GetInstance<IHealthCheckConventionManager>();

            IEnumerable<IHealthCheckConvention> conventions = context.Locate.Advanced.GetAllInstances<IHealthCheckConvention>();

            try
            {
                foreach (IHealthCheckConvention convention in conventions)
                    convention.Apply(conventionManager);

            }
            catch (Exception e)
            {
                ILog logger = LogManager.GetLogger(typeof(ApplyHealthCheckConventionsInitialization));

                //Make sure that our application can start even though health checks potentially fail due to unexpected errors
                logger.Error("Health check initialization failed", e);

            }
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
