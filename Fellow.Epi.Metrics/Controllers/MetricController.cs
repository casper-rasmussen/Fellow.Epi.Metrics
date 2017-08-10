using System.Web.Mvc;
using Fellow.Epi.Metrics.Manager.Metric;

namespace Fellow.Epi.Metrics.Controllers
{
    public class MetricController : Controller
    {
        private readonly IMetricManager _metricsManager;

        public MetricController(IMetricManager metricsManager)
        {
            this._metricsManager = metricsManager;

        }

        public ActionResult Index()
        {
            string output = this._metricsManager.GenerateReport();

            return Content(output, "text/plain");
        }

        public ActionResult Healthy()
        {
            bool state = this._metricsManager.Healthy();

            if(state)
                return Content("OK");
            else
                return Content("Not OK");
        }

        public ActionResult Reset()
        {
            this._metricsManager.Reset();

            return Content("OK");
        }
    }
}
