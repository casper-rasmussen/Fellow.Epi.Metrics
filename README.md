# Fellow.Epi.Metrics: Application Measurement and Monitoring made Easy

This Episerver add-on exposes key insights about platform performance and system health. It acts as a reporting platform useful as part of implementation, operation and incident management. By default, it allows you, as an Episerver developer, to easily register code to be measured along with integration endpoints to be monitored.

**Example performance outputs**
```
 [Fellow Metrics] DefaultContentRepository_Get
   Active Sessions = 0
        Total Time = 98.00 ms
             Count = 67 Requests
        Mean Value = 0.36 Requests/s
     1 Minute Rate = 0.73 Requests/s
     5 Minute Rate = 7.48 Requests/s
    15 Minute Rate = 11.03 Requests/s
             Count = 67 Requests
              Last = 0.00 ms
               Min = 0.00 ms
               Max = 42.03 ms
              Mean = 1.47 ms
            StdDev = 6.17 ms
            Median = 0.01 ms
              75% <= 0.02 ms
              95% <= 14.47 ms
              98% <= 21.41 ms
              99% <= 42.03 ms
            99.9% <= 42.03 ms

```

**Example monitoring output**
```

 ***** Health Checks - 2017-08-10T22:31:26.1213Z *****
    Is Healthy = No

    FAILED CHECKS

               SSO = FAILED: FAILED
    
    PASSED CHECKS
	       CRM = PASSED: OK
               ServiceBus = PASSED: OK
               ERP = PASSED: OK
               MA = PASSED: OK

```




It relies on the latest version of [Metrics.NET](https://github.com/Recognos/Metrics.NET), which has been extended to seamlessly integrate with Episerver.

## Installation and Prerequisites

You can get the latest version of Fellow.Epi.Metrics through [Episervers NuGet Feed](http://nuget.episerver.com/en/OtherPages/Package/?packageId=Fellow.Epi.Metrics).
Be aware that Fellow.Epi.Metrics requires EPiServer.Framework version 9.0.0 or higher.

Please use this GitHub project for any issues, questions or other kinds of feedback.

**Add-on Prerequisites**

You are required to setup a dedicated ASP.NET MVC route to access collected measurement and health outputs. Simply add this small snippet to your RouteConfig.cs or the RegisterRoutes method in Global.asax

```
	routes.MapRoute("MetricsRoute",					 
		"metric/{action}", //Hint: Adjust with your own URL segment if preferred
		new { controller = "Metric", action = "Index" }
	);

```

Next off, you are recommended to apply access control to govern the URL used for measurement and health outputs. Place following in your Web.config and remember to adjust according to your designated path, as specified in the MVC route, and authorization needs.

```	
  <location path="metric">
    <system.web>
      <authorization>
        <allow roles="WebAdmins"/>
        <deny users="*"/>
      </authorization>
    </system.web>
  </location>
```

## Usage

### Measurement
Enables you to apply timing measures around code - e.g. execution time of a given method or entire class, responsible of calling an external endpoint.

#### Code guidelines
The add-on relies on aspect oriented principles to ease application of code measurement. By relying on the StructureMap IoC container, via the decorator pattern, you are easily able to decorate an abstraction, an implementation or just a single method with measurement logic. It means, that after the registration, your code are automatically measured.

```
	//Use the IoC container to apply logic to a given abstraction or implementation - here it's Episerver's IContentRepository.
	container.For<IContentRepository>()
	//Use DecorateAllWith() to add a ApplyMetricsTimingInterceptor, through Castle.Core dynamic proxies, to apply measurements   
	.DecorateAllWith((c, i) => proxyGenerator.CreateInterfaceProxyWithTarget(i, new ApplyMetricsTimingInterceptor(c.GetInstance<IMetricManager>())));
	
```

#### Examples

**Apply measuring to any Episerver abstraction - e.g. all methods within Episerver's implementation of IContentRepository**

```
	// See Registration section for context of use.

	container.For<IContentRepository>().DecorateAllWith((c, i) => proxyGenerator.CreateInterfaceProxyWithTarget(i, new ApplyMetricsTimingInterceptor(c.GetInstance<IMetricManager>())));          
```

**Apply measures to all methods in your own abstraction**
```
	// See Registration section for context of use.

	container.For<IDocumentRepository>().DecorateAllWith((c, i) => proxyGenerator.CreateInterfaceProxyWithTarget(i, new ApplyMetricsTimingInterceptor(c.GetInstance<IMetricManager>())));          
```

**Apply measures directly to all methods within an implementation**
```
	// See Registration section for context of use.

    	container.For<UrlResolver>().DecorateAllWith((c, i) => proxyGenerator.CreateClassProxyWithTarget(i, new ApplyMetricsTimingInterceptor(c.GetInstance<IMetricManager>())));
```

**Scope measurement to a given method or methods**
```
	// See Registration section for context of use.

	IProxyGenerationHook methodHook = new SomeMethodsHook("MethodName");
	container.For<INavigationManager>().DecorateAllWith((c, i) => proxyGenerator.CreateInterfaceProxyWithTarget(i, new ProxyGenerationOptions(methodHook), new 	ApplyMetricsTimingInterceptor(c.GetInstance<IMetricManager>())));
```
```
	// See Registration section for context of use.

	IProxyGenerationHook methodHook = new SomeMethodsHook("MethodName1", "MethodName2");
    	container.For<INavigationManager>().DecorateAllWith((c, i) => proxyGenerator.CreateInterfaceProxyWithTarget(i, new ProxyGenerationOptions(methodHook), new 	ApplyMetricsTimingInterceptor(c.GetInstance<IMetricManager>())));
```

### Monitoring

The add-on also provides you with a unified way of performing application or system health checks. A health check is a small self-test, which your application performs, to verify that a component or responsibility is responding correctly.

```
class AuthenticationServerHealthCheckConvention : IHealthCheckConvention
{
	private const string Name = "SSO";

	//Dependency to validate
	private readonly Func<IAuthenticationManager> _authenticationManager;

	public AuthenticationServerHealthCheckConvention(Func<IAuthenticationManager> authenticationManager)
	{
		this._authenticationManager = authenticationManager;
	}

	public void Apply(IHealthCheckConventionManager healthCheckConventionManager)
	{
		bool skip = healthCheckConventionManager.IsHealthCheckIncluded(Name);

		if (!skip)
		{
			healthCheckConventionManager.IncludeHealthCheck(Name, () =>
			{
            			//Delegate to execute when requesting a healthcheck
				//Health check logic goes here. SSO runs if the service is healthy
				return this._authenticationManager.Invoke().Healthy();
			});
		}
	}
}

```

### Registration

Registration of measurements and monitors are done through an Episerver Initialization Module.


```
[InitializableModule]
[ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
public class MetricsInitialization : IConfigurableModule
{
	public void ConfigureContainer(ServiceConfigurationContext context)
	{
		context.Container.Configure(ConfigureContainer);
	}

	private static void ConfigureContainer(ConfigurationExpression container)
	{

		//Health Check
		container.For<IHealthCheckConvention>().Add<AuthenticationServerHealthCheckConvention>();

		//Measurements
		ProxyGenerator proxyGenerator = new ProxyGenerator();

		container.For<IContentRepository>().DecorateAllWith((c, i) => proxyGenerator.CreateInterfaceProxyWithTarget(i, new ApplyMetricsTimingInterceptor(c.GetInstance<IMetricManager>())));

		IProxyGenerationHook methodHook = new SomeMethodsHook("GetUserNavigation");
		container.For<INavigationManager>().DecorateAllWith((c, i) => 
            	proxyGenerator.CreateInterfaceProxyWithTarget(i, new ProxyGenerationOptions(methodHook), new ApplyMetricsTimingInterceptor(c.GetInstance<IMetricManager>())));

	}

	public void Initialize(InitializationEngine context)
	{
	}

	public void Uninitialize(InitializationEngine context)
	{
	}

}

```

If you are unfamiliar with the use of Episerver Initialization Modules, then please visit the [documentation here](http://world.episerver.com/documentation/Items/Developers-Guide/Episerver-Framework/7/Initialization/Creating-an-Initialization-Module/)

	
## Reporting

Measurement and health outputs are exposed via a text feed, which can be accessed according to the route configured. As per above routing example, the address would be e.g. https://yourdomain.com/metric.

**Example output**

```
Fellow Metrics - 2017-08-10T22:31:26.1373Z


***** Timers - 2017-08-10T22:31:26.1213Z *****

    [Fellow Metrics] DefaultContentRepository_Get
   Active Sessions = 0
        Total Time = 98.00 ms
             Count = 67 Requests
        Mean Value = 0.36 Requests/s
     1 Minute Rate = 0.73 Requests/s
     5 Minute Rate = 7.48 Requests/s
    15 Minute Rate = 11.03 Requests/s
             Count = 67 Requests
              Last = 0.00 ms
               Min = 0.00 ms
               Max = 42.03 ms
              Mean = 1.47 ms
            StdDev = 6.17 ms
            Median = 0.01 ms
              75% <= 0.02 ms
              95% <= 14.47 ms
              98% <= 21.41 ms
              99% <= 42.03 ms
            99.9% <= 42.03 ms

    [Fellow Metrics] DefaultUrlResolver_GetUrl
   Active Sessions = 0
        Total Time = 226.00 ms
             Count = 5 Requests
        Mean Value = 0.03 Requests/s
     1 Minute Rate = 0.05 Requests/s
     5 Minute Rate = 0.56 Requests/s
    15 Minute Rate = 0.82 Requests/s
             Count = 5 Requests
              Last = 0.79 ms
               Min = 0.79 ms
               Max = 121.58 ms
              Mean = 45.08 ms
            StdDev = 54.47 ms
            Median = 1.61 ms
              75% <= 101.45 ms
              95% <= 121.58 ms
              98% <= 121.58 ms
              99% <= 121.58 ms
            99.9% <= 121.58 ms

    [Fellow Metrics] NavigationManager_GetUserNavigation
   Active Sessions = 0
        Total Time = 2.00 ms
             Count = 2 Requests
        Mean Value = 0.01 Requests/s
     1 Minute Rate = 0.02 Requests/s
     5 Minute Rate = 0.22 Requests/s
    15 Minute Rate = 0.33 Requests/s
             Count = 2 Requests
              Last = 0.02 ms
               Min = 0.02 ms
               Max = 1.71 ms
              Mean = 0.87 ms
            StdDev = 0.84 ms
            Median = 1.71 ms
              75% <= 1.71 ms
              95% <= 1.71 ms
              98% <= 1.71 ms
              99% <= 1.71 ms
            99.9% <= 1.71 ms


***** Health Checks - 2017-08-10T22:31:26.1213Z *****

        Is Healthy = No


    FAILED CHECKS

               SSO = FAILED: FAILED
    
    PASSED CHECKS
	       CRM = PASSED: OK
               ServiceBus = PASSED: OK
               ERP = PASSED: OK
               MA = PASSED: OK

```

**Reset collected metrics**

You can always reset any collected metrics by browsing https://yourdomain.com/metric/reset/. Remember, the URL segment will be as per your route configuration.

**Automated monitoring**

Episerver Managed Services relies on an enterprise monitoring tools named Pingdom. It enables you to perform automated application health checks, in which you should include feedback from this add-on.

Setting up a HTTP health prope is possible by requesting https://yourdomain.com/metric/healthy/.

It returns any of below responses, according to the application health checks, with an HTTP Status Code of 200 OK.
```
OK
```

```
Not OK
```
