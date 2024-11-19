using System.Collections.Generic;
using System.Linq;
using System.Web;
using Connexien.Lib;

namespace Connexien.Web.Models
{

    public class SearchModel
    {
        #region Properties

        public long[] LicenseTypes { get; set; }
        public long[] DesigTypes { get; set; }
        public long[] ProductTypes { get; set; }
        public long[] ServiceTypes { get; set; }
        public Dictionary<long, string> ProductRefs { get; set; } 
        public Dictionary<long, string> ServiceRefs { get; set; } 

        public List<ProfileModel> Profiles { get; set; } 

        #endregion


        public static SearchModel Search(SearchModel model)
        {
            model.ProductTypes = model.ProductTypes ?? new long[0];
            model.ServiceTypes = model.ServiceTypes ?? new long[0];

            model.ProductRefs = model.ProductRefs ?? OptionModel.GetProducts();
            model.ServiceRefs = model.ServiceRefs ?? OptionModel.GetServices();

            using(var db = new ConnexienEntities())
            {
                var profiles = db.Profiles.Where(x => x.Deleted == null && x.Type == "provider" && (x.ProfileProductTypes.Any(y => model.ProductTypes.Contains(y.ProductTypeId)) || x.ProfileServiceTypes.Any(y => model.ServiceTypes.Contains(y.ServiceTypeId)))).ToList();
                model.Profiles = profiles.Where(x => x.UserId != 121).Select(x => new ProfileModel(x)).ToList();

                HttpContext.Current.Session["search"] = model;
                return model;
            }
        }

        public static SearchModel Filter(ProfileModel model)
        {
            var list = (SearchModel) HttpContext.Current.Session["search"];

            list.ProductTypes = list.ProductTypes ?? new long[0];
            list.ServiceTypes = list.ServiceTypes ?? new long[0];
            model.ProductTypes = model.ProductTypes ?? new long[0];
            model.ServiceTypes = model.ServiceTypes ?? new long[0];

            if (!list.ProductTypes.All(model.ProductTypes.Contains) || list.ProductTypes.Count() != model.ProductTypes.Count() || !list.ServiceTypes.All(model.ServiceTypes.Contains) || list.ServiceTypes.Count() != model.ServiceTypes.Count())
            {
                list.ProductTypes = model.ProductTypes;
                list.ServiceTypes = model.ServiceTypes;
                list = Search(list);
                HttpContext.Current.Session["search"] = list;
            }

            var results = new SearchModel
                              {
                                  LicenseTypes = list.LicenseTypes,
                                  DesigTypes = list.DesigTypes,
                                  ProductTypes = list.ProductTypes,
                                  ServiceTypes = list.ServiceTypes,
                                  ProductRefs = list.ProductRefs,
                                  ServiceRefs = list.ServiceRefs,
                                  Profiles = list.Profiles
                              };

            if (model.Licenses != null && model.Licenses.Any())
                results.Profiles = results.Profiles.Where(x => model.Licenses.Intersect(x.Licenses).Any()).ToList();

            if (model.Designations != null && model.Designations.Any())
                results.Profiles = results.Profiles.Where(x => model.Designations.Intersect(x.Designations).Any()).ToList();

            if (model.Focuses != null && model.Focuses.Any())
                results.Profiles = results.Profiles.Where(x => model.Focuses.Intersect(x.Focuses).Any()).ToList();

            if (model.ServiceStates != null && model.ServiceStates.Any())
                results.Profiles = results.Profiles.Where(x => x.AllStatesBit || model.ServiceStates.Intersect(x.ServiceStates).Any()).ToList();

            if (model.FeeType != null)
                results.Profiles = results.Profiles.Where(x => x.FeeType == model.FeeType || x.FeeType == "both").ToList();

            if (model.FeeType == "hourly" || model.FeeType == "both")
                results.Profiles = results.Profiles.Where(x => x.HourlyRate <= model.HourlyRate || model.HourlyRate == 0 || x.HourlyRate == null).ToList();

            if (model.Availability != null && model.Availability != "0")
                results.Profiles = results.Profiles.Where(x => x.Availability == model.Availability || (x.Availability == "short term" && model.Availability == "all")).ToList();

            if (model.Experience != null && model.Experience != "0")
                results.Profiles = results.Profiles.Where(x => x.Experience == model.Experience).ToList();

            if (model.EdLevel != null && model.EdLevel != "0")
                results.Profiles = results.Profiles.Where(x => x.EdLevel == model.EdLevel).ToList();

            results.Profiles = results.Profiles.Where(x => x.UserId != 121).ToList();

            return results;
        }
        
    }
}
