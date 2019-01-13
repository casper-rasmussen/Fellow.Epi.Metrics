using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Fellow.Epi.Metrics.Bootstrapper;
using StructureMap;

namespace Fellow.Epi.Metrics.Infrastructure.Initialization
{
	[InitializableModule]
	[ModuleDependency(typeof(ServiceContainerInitialization))]
	public class DependencyResolverInitialization : IConfigurableModule
	{
		public void ConfigureContainer(ServiceConfigurationContext context)
		{
			context.StructureMap().Configure(ConfigureContainer);
		}

		private static void ConfigureContainer(ConfigurationExpression container)
		{
			//Register Authentication
			container.AddRegistry<FellowEpiMetricsImplementationBootstrapper>();

		}

		public void Initialize(InitializationEngine context)
		{
		}

		public void Uninitialize(InitializationEngine context)
		{
		}

		public void Preload(string[] parameters)
		{
		}
	}
}
