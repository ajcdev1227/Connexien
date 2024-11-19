using Connexien.Web.Models;
using System.Web.Mvc;

namespace Connexien.Web.Controllers
{

    public class AController : Controller
    {
        public ActionResult Index(string id)
        {
            var article = PublicationModel.Get(id);

            if (article == null)
                return RedirectToAction("Index", "Home");

            Lib.Utilities.LogArticleView(article.ArticleGuid, UserModel.GetUserId());
            return View(article);
        }

        [AllowAnonymous]
        public ActionResult Download(string id)
        {
            var guid = id.Split('.')[0];

            var article = PublicationModel.Get(guid);
            if (article == null)
                return RedirectToAction("Index", "A", new { guid });

            Response.AppendHeader("Content-Disposition", "inline; filename=" + id);
            var file = Lib.Utilities.GetArticle(article.Filename);
            return File(file, article.MimeType);
        }

    }
}
