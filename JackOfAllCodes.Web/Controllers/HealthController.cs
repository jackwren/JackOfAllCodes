using Microsoft.AspNetCore.Mvc;

namespace JackOfAllCodes.Web.Controllers
{
    public class HealthController : Controller
    {
        public ActionResult Check()
        {
            return Content("Health Check OK");
        }
    }
}
