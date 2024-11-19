using System.Web.Mvc;

namespace Connexien.Web.Controllers
{
    [RoutePrefix("splash")]
    public class SplashController : Controller
    {
        public ActionResult Store()
        {
            return View();
        }

        [HttpGet, Route("form-builder")]
        public ActionResult FormBuilder()
        {
            return View();
        }

        public ActionResult Publisher()
        {
            return View();
        }

        [HttpGet, Route("find-expert")]
        public ActionResult FindAnExpert()
        {
            return View();
        }

        [HttpGet, Route("list-services")]
        public ActionResult ListServices()
        {
            return View();
        }

        public ActionResult Contributor()
        {
            return View();
        }
    }
}