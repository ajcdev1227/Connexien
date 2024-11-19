using Connexien.Lib;
using Connexien.Lib.Payment;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using Stripe.Checkout;

namespace Connexien.Web.Models
{
	public class PublicationModel
	{
		#region Properties

		public long Id { get; set; }

		public long ProfileId { get; set; }

		public string ProfileGuid { get; set; }

		public string ArticleGuid { get; set; }

		public DateTime Created { get; set; }

		public DateTime? Published { get; set; }

		[Required(ErrorMessage = "Please provide a title for the publication article.")]
		public string Title { get; set; }

		[Required(ErrorMessage = "Please provide an author(s) for the publication article.")]
		public string Authors { get; set; }

		[Required(ErrorMessage = "Please provide a description for the publication article.")]
		public string Description { get; set; }

		public long[] ServiceTags { get; set; }

		public string[] SectorTags { get; set; }

		public string ProductTags { get; set; }

		public string MimeType { get; set; }

		public string Filename { get; set; }

		public HttpPostedFileBase File { get; set; }

		public string PaymentStatus { get; set; }

		public string PaymentTxn { get; set; }

		public string Visibility { get; set; }

		public long Views { get; set; }

		public string CartUrl { get; set; }

		public string StripeToken { get; set; }

		public string StripeEmail { get; set; }

		#endregion

		public PublicationModel()
		{
		}

		public PublicationModel(ProfileArticle article)
		{
			Id = article.Id;
			ProfileId = article.ProfileId;
			ProfileGuid = article.Profile.Guid;
			ArticleGuid = article.Guid;
			Created = article.Created;
			Published = article.Published;
			Title = article.Title;
			Authors = article.Authors;
			Description = article.Description;
			ServiceTags = article.ProfileArticleTags.Where(x => x.ServiceTypeId != null).Select(x => x.ServiceTypeId ?? 0).ToArray();
			ProductTags = string.Join(",", article.ProfileArticleTags.Where(x => x.ProductTypeId != null).Select(x => x.ProductTypeId ?? "").ToArray());
			SectorTags = article.ProfileArticleTags.Where(x => x.SectorTypeId != null).Select(x => x.SectorTypeId ?? "").ToArray();
			MimeType = article.MimeType;
			Filename = article.Filename;
			PaymentStatus = article.PaymentStatus;
			PaymentTxn = article.PaymentTxn;
			Visibility = article.Visibility;
			Views = article.Views;
		}

		public static PublicationModel Get(string guid)
		{
			//var userId = UserModel.GetUserId();

			using (var db = new ConnexienEntities())
			{
				var a = db.ProfileArticles.FirstOrDefault(x => x.Guid == guid);
				return a == null ? null : new PublicationModel(a);
			}
		}

		public static bool Delete(long id)
		{
			var userId = UserModel.GetUserId();

			try
			{

				using (var db = new ConnexienEntities())
				{
					var a = db.ProfileArticles.FirstOrDefault(x => x.Id == id && x.Profile.UserId == userId);
					if (a == null) return false;

					db.ProfileArticles.Remove(a);
					db.SaveChanges();

					return true;
				}
			}
			catch (Exception ex)
			{
				Exceptions.Log(ex, UserModel.GetIp(), "Delete Publication");
				return false;
			}
		}

		public static List<PublicationModel> Search(string q, bool all = false)
		{
			if (string.IsNullOrWhiteSpace(q) && !all)
				return new List<PublicationModel>();

			using (var db = new ConnexienEntities())
			{
				if (string.IsNullOrWhiteSpace(q) && all)
					return db.ProfileArticles.Where(x => x.Published != null).ToList().Select(x => new PublicationModel(x)).ToList();

				var keywords = q.ToLower().Trim().Split(' ');
				var results = new List<PublicationModel>();

				foreach (var k in keywords)
				{
					results.AddRange(db.ProfileArticles.Where(x => x.Published != null && (x.Title.ToLower().Contains(k) || x.Description.ToLower().Contains(k) || x.Authors.ToLower().Contains(k))).ToList().Select(x => new PublicationModel(x)).ToList());
				}

				var agg = results.GroupBy(x => x.Id).OrderByDescending(x => x.Count()).ToList().Select(x => x.First()).ToList();
				return agg;
			}
		}

		public static List<PublicationModel> Filter(PublicationModel model)
		{

			var list = Search(model.Title, true);

			if (model.ServiceTags != null && model.ServiceTags.Any())
				list = list.Where(x => x.ServiceTags.Intersect(model.ServiceTags).Any()).ToList();

			if (model.ProductTags != null && model.ProductTags.Any())
				list = list.Where(x => x.ProductTags.Intersect(model.ProductTags).Any()).ToList();

			return list;
		}


		//--------------------------------------------------------

		public static bool Submit(PublicationModel model, out string guid)
		{
			guid = Guid.NewGuid().ToString();

			try
			{
				var userId = UserModel.GetUserId();

				using (var db = new ConnexienEntities())
				{
					var userProfile = UserModel.GetProfiles().FirstOrDefault();
					var profile = db.Profiles.FirstOrDefault(x => x.Id == model.ProfileId && x.UserId == userId && x.Deleted == null);
					if (profile == null || model.File == null || model.File.ContentLength == 0)
					{
						return false;
					}

                    var sessionService = new SessionService();
                    Session session = sessionService.Get(model.StripeToken);

                    #region Upload File

                    var file = model.File;
					var ext = Path.GetExtension(model.File.FileName);

					var article = new ProfileArticle
					{
						Created = DateTime.UtcNow,
						Guid = guid,
						Title = model.Title,
						Authors = model.Authors,
						Description = model.Description,
						MimeType = Utilities.MimeTypeMap.GetMimeType(ext),
						PaymentStatus = session.Status,
						Visibility = model.Visibility,
						Views = 0,
						Published = DateTime.UtcNow,
						PaymentTxn = session.PaymentIntentId
                    };

					if (model.SectorTags != null && model.SectorTags.Any())
					{
						foreach (var t in model.SectorTags)
						{
							article.ProfileArticleTags.Add(new ProfileArticleTag { SectorTypeId = t });
						}
					}

					if (model.ProductTags != null && model.ProductTags.Any())
					{
						string[] productTags = model.ProductTags.Split(',');
						foreach (var t in productTags)
						{
							article.ProfileArticleTags.Add(new ProfileArticleTag { ProductTypeId = t });
						}
					}

					article.Filename = article.Guid + "." + ext;
					if (file != null && file.ContentLength > 0)
					{
						ConnexienStorageBlob.UploadFileToStorageBlob(file, ConnexienStorageContainers.StorageContainer.Publications, article.Filename);
					}

					profile.ProfileArticles.Add(article);
					db.SaveChanges();

					#endregion

					return true;
				}

			}
			catch (Exception ex)
			{

				var ip = UserModel.GetIp();
				Exceptions.Log(ex, ip, "PublicationModel/Submit");
				return false;
			}
		}

		public static bool Update(PublicationModel model)
		{

			try
			{
				var userId = UserModel.GetUserId();

				using (var db = new ConnexienEntities())
				{

					var a = db.ProfileArticles.FirstOrDefault(x => x.Id == model.Id && x.Profile.UserId == userId && x.Profile.Deleted == null);
					if (a == null || model.File == null || model.File.ContentLength == 0)
					{
						return false;
					}

					a.Title = string.IsNullOrWhiteSpace(model.Title) ? a.Title : model.Title;
					a.Authors = string.IsNullOrWhiteSpace(model.Authors) ? a.Authors : model.Authors;
					a.Description = model.Description;

					#region Upload File


					var ext = Path.GetExtension(model.File.FileName);
					a.MimeType = Utilities.MimeTypeMap.GetMimeType(ext);
					ConnexienStorageBlob.UploadFileToStorageBlob(model.File, ConnexienStorageContainers.StorageContainer.Publications, a.Filename);


					foreach (var t in a.ProfileArticleTags.ToList())
					{
						db.ProfileArticleTags.Remove(t);
					}


					foreach (var t in model.ServiceTags)
					{
						a.ProfileArticleTags.Add(new ProfileArticleTag { ServiceTypeId = t });
					}

					string[] productTags = model.ProductTags.Split(',');
					foreach (var t in productTags)
					{
						a.ProfileArticleTags.Add(new ProfileArticleTag { ProductTypeId = t });
					}

					db.SaveChanges();


					#endregion

					return true;
				}

			}
			catch (Exception ex)
			{

				var ip = UserModel.GetIp();
				Exceptions.Log(ex, ip, "PublicationModel/Update");
				return false;
			}
		}
	}
}
