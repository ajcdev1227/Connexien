using Connexien.Lib;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Connexien.Web.Models
{
    public class OptionModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public static List<SelectListItem> GetLicenses(long[] selected = null, string type = "", string placeholder = "", bool sort = true)
        {
            var list = string.IsNullOrEmpty(placeholder)
                        ? new List<SelectListItem> { new SelectListItem { Selected = true, Text = placeholder, Value = "0" } }
                        : new List<SelectListItem>();

            using (var db = new ConnexienEntities())
            {
                var tmp = string.IsNullOrEmpty(type) ? db.Licenses : db.Licenses.Where(x => x.Type == type.ToLower());
                var opts = sort ? tmp.OrderBy(x => x.Sort).ToList() : tmp.ToList();
                var tmpList = new List<SelectListItem>();

                var selList = new List<long>();

                if (selected != null)
                {
                    selList.AddRange(selected);
                }

                opts.ForEach(x => list.Add(new SelectListItem { Selected = selList.Contains(x.Id), Text = x.Name, Value = x.Id.ToString(CultureInfo.InvariantCulture) }));
                //list = new MultiSelectList(tmpList);
            }

            return list;
        }

        public static List<SelectListItem> GetDesignations(long[] selected = null, string placeholder = "", bool sort = true)
        {
            var list = string.IsNullOrEmpty(placeholder)
                        ? new List<SelectListItem> { new SelectListItem { Selected = true, Text = placeholder, Value = "0" } }
                        : new List<SelectListItem>();

            using (var db = new ConnexienEntities())
            {
                var tmp = db.Designations.ToList();
                var opts = sort ? tmp.OrderBy(x => x.Abbrev).ToList() : tmp.ToList();

                var selList = new List<long>();

                if (selected != null)
                {
                    selList.AddRange(selected);
                }

                opts.ForEach(x => list.Add(new SelectListItem { Selected = selList.Contains(x.Id), Text = x.Abbrev + "- " + x.Name, Value = x.Id.ToString(CultureInfo.InvariantCulture) }));
            }

            return list.Count == 0 ? null : list;
        }

        public static List<SelectListItem> GetFocuses(string[] selected = null, string placeholder = "", bool sort = true)
        {
            var list = string.IsNullOrEmpty(placeholder)
                        ? new List<SelectListItem> { new SelectListItem { Selected = true, Text = placeholder, Value = "0" } }
                        : new List<SelectListItem>();

            var selList = new List<string>();

            if (selected != null)
            {
                selList.AddRange(selected);
            }

            var opts = new List<string> { "Broker/Dealer", "Investment Adviser", "Banking", "Insurance" };
            opts.ForEach(x => list.Add(new SelectListItem { Selected = selList.Contains(x), Text = x, Value = x }));

            return list.Count == 0 ? null : list;
        }

        public static List<SelectListItem> GetSectorTypes(string[] selected = null, string placeholder = "", bool sort = true)
        {
            var list = string.IsNullOrEmpty(placeholder)
                        ? new List<SelectListItem> { new SelectListItem { Selected = true, Text = placeholder, Value = "" } }
                        : new List<SelectListItem>();

            var selList = new List<string>();

            if (selected != null)
            {
                selList.AddRange(selected);
            }

            using (var db = new ConnexienEntities())
            {
                var opts = db.SectorTypes.ToList();
                opts.ForEach(x => list.Add(new SelectListItem { Selected = selList.Contains(x.Id), Text = x.Name, Value = x.Id }));

                return list.Count == 0 ? null : list;
            }
        }

        public static List<SelectListItem> GetSectorTypesForFormBuilder()
        {
            var list = GetSectorTypes();

            foreach (var selectListItem in list)
            {
                selectListItem.Value = selectListItem.Value.Replace("CP", "FB");
            }

            return list;
        }

        public static List<SelectListItem> GetContentPartnerPrices(decimal selected = 0, string placeholder = "", bool sort = true)
        {
            var list = string.IsNullOrEmpty(placeholder)
                        ? new List<SelectListItem> { new SelectListItem { Selected = true, Text = placeholder, Value = "" } }
                        : new List<SelectListItem>();

            decimal otherValue = 99999M;

            var selList = new List<decimal>();
            var opts = new List<decimal> { 25M, 50M, 75M, 100M, 125M, otherValue };

            if (selected != 0)
            {
                if (opts.Contains(selected))
                {
                    selList.Add(selected);
                }
                else
                {
                    selList.Add(otherValue);
                }
            }

            using (var db = new ConnexienEntities())
            {
                opts.ForEach(x => list.Add(new SelectListItem { Selected = selList.Contains(x), Text = x == otherValue ? "Other" : x.ToString("C"), Value = x.ToString() }));

                return list.Count == 0 ? null : list;
            }
        }

        public static Dictionary<string, SelectList> GetServiceTypes(long[] selected = null, string placeholder = "")
        {
            using (var db = new ConnexienEntities())
            {
                var opts = db.ServiceTypes.GroupBy(x => x.Name);
                return opts.ToDictionary(x => x.Key, x => new SelectList(x, "Id", "SubCategory"));
            }
        }

        public static Dictionary<string, SelectList> GetProductTypes(long[] selected = null, string placeholder = "")
        {
            using (var db = new ConnexienEntities())
            {
                var opts = db.ProductTypes.GroupBy(x => x.Name);
                return opts.ToDictionary(x => x.Key, x => new SelectList(x, "Id", "SubCategory"));
            }
        }

        //--------------------------------------

        public static Dictionary<long, string> GetProducts()
        {

            if (HttpContext.Current.Cache["products"] == null)
            {
                HttpContext.Current.Cache["products"] = new Dictionary<long, string>();

                using (var db = new ConnexienEntities())
                {
                    HttpContext.Current.Cache["products"] = db.ProductTypes.ToDictionary(x => x.Id, x => x.SubCategory);
                }
            }

            return (Dictionary<long, string>)HttpContext.Current.Cache["products"];
        }


        public static Dictionary<long, string> GetServices()
        {

            if (HttpContext.Current.Cache["services"] == null)
            {
                HttpContext.Current.Cache["services"] = new Dictionary<long, string>();

                using (var db = new ConnexienEntities())
                {
                    HttpContext.Current.Cache["services"] = db.ServiceTypes.ToDictionary(x => x.Id, x => x.SubCategory);
                }
            }

            return (Dictionary<long, string>)HttpContext.Current.Cache["services"];
        }

    }
}
