using System;
using System.Web.Mvc;

namespace Connexien.Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {

        public ActionResult GetUserProfiles()
        {
            var excelData = Lib.Utilities.GetMemberProfiles();

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"{DateTime.Now.ToString("yyyyMMddHHmm")}_MemberList.xlsx";
            return File(excelData, contentType, fileName);
        }

    }
}
