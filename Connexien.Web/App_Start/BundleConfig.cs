using System.Web;
using System.Web.Optimization;

namespace Connexien.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/libs/jquery-{version}.js",
                                                                     "~/Scripts/libs/bootstrap.js",
                                                                     "~/scripts/libs/jquery.bootstrap.wizard.js",
                                                                     //"~/Scripts/libs/chosen.jquery.js",
                                                                     "~/Scripts/libs/bootstrap-select.js",
                                                                     "~/Scripts/libs/toastr.js",
                                                                     "~/Scripts/libs/spin.js"));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include("~/Scripts/custom/custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/ratings").Include("~/Scripts/custom/ratings.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/libs/jquery.validate*",
                                                                        "~/Scripts/libs/jquery.unobtrusive*"));

            bundles.Add(new ScriptBundle("~/bundles/user").Include("~/Scripts/libs/typeahead.bundle.js", 
                                                                   "~/Scripts/custom/wizard.js", 
                                                                   "~/Scripts/custom/user.js"));

            bundles.Add(new StyleBundle("~/Content/bsOnly").Include("~/Content/css/bootstrap.css", 
                                                                    "~/Content/css/bootstrap-theme.css", 
                                                                    "~/Content/css/toastr.css"));

            bundles.Add(new StyleBundle("~/Content/base").Include("~/Content/css/bootstrap.css", 
                                                                  "~/Content/css/bootstrap-theme.css", 
                                                                  "~/Content/css/base.css", 
                                                                  //"~/Content/css/chosen.css", 
                                                                  "~/Content/css/bootstrap-select.css",
                                                                  "~/Content/css/toastr.css", 
                                                                  "~/Content/css/style.css"));

            bundles.Add(new StyleBundle("~/Content/fullbg").Include("~/Content/css/full-bg.css"));

            bundles.Add(new StyleBundle("~/Content/profilebg").Include("~/Content/css/profile-bg.css"));

        }
    }
}