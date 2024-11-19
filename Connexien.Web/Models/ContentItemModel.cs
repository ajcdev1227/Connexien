using Connexien.Lib;
using Connexien.Lib.Communication;
using Connexien.Lib.Communication.EmailParameters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Connexien.Web.Models
{
	public class ContentItemModel
	{
		#region Properties

		public long Id { get; set; }

		public long ProfileId { get; set; }

		public string ProfileGuid { get; set; }

		public string ContentItemGuid { get; set; }

		public DateTime Created { get; set; }

		public DateTime Updated { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Authors { get; set; }

		[Required]
		[StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters.")]
		public string Description { get; set; }

		[Required(ErrorMessage = "Please select an industry")]
		public string Sector { get; set; }

		[Required]
		[DataType(DataType.Currency)]
		[Range(typeof(Decimal), "1.00", "1000000000000", ErrorMessage = "Price needs to be greater than 0")]
		public decimal Price { get; set; }

		[Range(1, 10000000)]
		public int Version { get; set; }

		[Range(1, 10000000)]
		public int Pages { get; set; }

		public string MimeType { get; set; }

		public string Filename { get; set; }
		public HttpPostedFileBase File { get; set; }

		[Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the Content Partner Terms and Conditions.")]
		public bool AcceptedTermsAndConditions { get; set; }

		//public string CartUrl { get; set; }

		#endregion

		public ContentItemModel()
		{
		}

		public ContentItemModel(ContentItem contentItem)
		{
			Id = contentItem.Id;
			ProfileId = contentItem.ProfileId;
			ProfileGuid = contentItem.Profile.Guid;
			ContentItemGuid = contentItem.Guid;
			Created = contentItem.Created;
			Updated = contentItem.Updated;
			Title = contentItem.Title;
			Authors = contentItem.Authors;
			Description = contentItem.Description;
			Sector = contentItem.Sector;
			Price = contentItem.Price;
			Version = contentItem.Version;
			Pages = contentItem.Pages;
			MimeType = contentItem.MimeType;
			Filename = contentItem.Filename;
		}

		public static ContentItemModel Get(string guid)
		{
			//var userId = UserModel.GetUserId();

			using (var db = new ConnexienEntities())
			{
				var a = db.ContentItems.FirstOrDefault(x => x.Guid == guid);
				return a == null ? null : new ContentItemModel(a);
			}
		}

		public static bool Delete(long id)
		{
			var userId = UserModel.GetUserId();

			try
			{

				using (var db = new ConnexienEntities())
				{
					var a = db.ContentItems.FirstOrDefault(x => x.Id == id && x.Profile.UserId == userId);
					if (a == null)
					{
						return false;
					}

					db.ContentItems.Remove(a);
					db.SaveChanges();

					return true;
				}
			}
			catch (Exception ex)
			{
				Exceptions.Log(ex, UserModel.GetIp(), "Delete Content Item");
				return false;
			}
		}

		public static List<ContentItemModel> Search(string q, bool all = false)
		{
			if (string.IsNullOrWhiteSpace(q) && !all)
			{
				return new List<ContentItemModel>();
			}

			using (var db = new ConnexienEntities())
			{
				if (string.IsNullOrWhiteSpace(q) && all)
				{
					return db.ContentItems.ToList().Select(x => new ContentItemModel(x)).ToList();
				}

				var keywords = q.ToLower().Trim().Split(' ');
				var results = new List<ContentItemModel>();

				foreach (var k in keywords)
				{
					results.AddRange(db.ContentItems.Where(x => (x.Title.ToLower().Contains(k) ||
																 x.Description.ToLower().Contains(k) ||
																 x.Authors.ToLower().Contains(k))).ToList().Select(x => new ContentItemModel(x)).ToList());
				}

				var agg = results.GroupBy(x => x.Id).OrderByDescending(x => x.Count()).ToList().Select(x => x.First()).ToList();
				return agg;
			}
		}

		public static List<ContentItemModel> Filter(ContentItemModel model)
		{

			var list = Search(model.Title, true);

			return list;
		}


		//--------------------------------------------------------

		public static bool Submit(ContentItemModel model, out string guid)
		{
			guid = Guid.NewGuid().ToString("N");
			model.ContentItemGuid = guid;  // for use later

			try
			{
				var userId = UserModel.GetUserId();

				using (var db = new ConnexienEntities())
				{

					var profile = db.Profiles.FirstOrDefault(x => x.Id == model.ProfileId && x.UserId == userId && x.Deleted == null);
					if (profile == null || model.File == null || model.File.ContentLength == 0)
					{
						return false;
					}

					#region Upload File

					var ext = Path.GetExtension(model.File.FileName);

					var contentItem = new ContentItem
					{
						Created = model.Created,
						Updated = model.Updated,
						Guid = model.ContentItemGuid,
						Title = $"{model.Sector}-{model.Title}",
						Authors = model.Authors,
						Description = model.Description,
						Sector = model.Sector,
						Price = model.Price,
						Version = model.Version,
						Pages = model.Pages,
						MimeType = Utilities.MimeTypeMap.GetMimeType(ext)
					};

					contentItem.Filename = $"{contentItem.Guid}{ext}";
					model.Filename = contentItem.Filename;

					ConnexienStorageBlob.UploadFileToStorageBlob(model.File, ConnexienStorageContainers.StorageContainer.ContentItems, contentItem.Filename);


					profile.ContentItems.Add(contentItem);
					db.SaveChanges();
					#endregion

					ICommunication<IEmailParameters> communication = new MailGunEmail<IEmailParameters>();

					IEmailParameters emailParameters = new ContentPartnerEmailParameters(profile);
					IEmailParameters backendEmailParameters = new ConnexienBackEndEmailParameters(
						bodyText: $"A new Content Item has been submitted to the site by {profile.User.FirstName} {profile.User.LastName} : {profile.Email} {GetStringDetails(model)}",
						subject: "Connexien Contributing Partner Program : New Item"
					//attachments: new Dictionary<string, string>() { { contentItem.Filename, ConnexienStorageBlob.GetPathOfBlob(contentItem.Filename,
					//                                                                                                           ConnexienStorageContainers.StorageContainer.ContentItems) } }
					);

					communication.SendMessage(emailParameters);
					communication.SendMessage(backendEmailParameters);

					return true;
				}

			}
			catch (Exception ex)
			{

				var ip = UserModel.GetIp();
				Exceptions.Log(ex, ip, "ContentItemModel/Submit");
				return false;
			}
		}

		public static bool Update(ContentItemModel model)
		{

			try
			{
				var userId = UserModel.GetUserId();

				using (var db = new ConnexienEntities())
				{

					var a = db.ContentItems.FirstOrDefault(x => x.Id == model.Id && x.Profile.UserId == userId && x.Profile.Deleted == null);
					List<string> sectors = db.SectorTypes.Select(x => x.Id).ToList();

					if (a == null)
					{
						return false;
					}

					string title = string.Empty;

					if (!string.IsNullOrWhiteSpace(model.Title))
					{
						if (sectors.Contains(model.Title.Substring(0, 4)))
						{
							int length = model.Title.Length - 4;
							title = $"{model.Sector}{model.Title.Substring(4, length)}";
						}
						else
						{
							title = $"{model.Sector}-{model.Title}";
						}
					}

					a.Updated = model.Updated;
					a.Title = title;
					a.Authors = string.IsNullOrWhiteSpace(model.Authors) ? a.Authors : model.Authors;
					a.Description = model.Description;
					a.Sector = model.Sector;
					a.Price = model.Price;
					a.Version = model.Version;
					a.Pages = model.Pages;

					#region Upload File

					var file = model.File;

					if (file != null)
					{
						var ext = Path.GetExtension(file.FileName);
						a.MimeType = Utilities.MimeTypeMap.GetMimeType(ext);
						ConnexienStorageBlob.UploadFileToStorageBlob(file, ConnexienStorageContainers.StorageContainer.ContentItems, a.Filename);
					}

					db.SaveChanges();


					#endregion

					var profile = db.Profiles.FirstOrDefault(x => x.Id == model.ProfileId && x.UserId == userId && x.Deleted == null);

					ICommunication<IEmailParameters> communication = new MailGunEmail<IEmailParameters>();

					IEmailParameters emailParameters = new ContentPartnerEmailParameters(profile);
					IEmailParameters backendEmailParameters = new ConnexienBackEndEmailParameters(
						bodyText: $"An updated Content Item has been submitted to the site by {profile.User.FirstName} {profile.User.LastName} : {profile.Email} {GetStringDetails(model)}",
						subject: "Connexien Contributing Partner Program : Updated Item"
					//attachments: new Dictionary<string, string>() { { a.Filename, $@"{Utilities.GetSiteRootDirectory()}{Utilities.ContentPartnerDirectory}" } }
					);

					communication.SendMessage(emailParameters);
					communication.SendMessage(backendEmailParameters);

					return true;
				}

			}
			catch (Exception ex)
			{

				var ip = UserModel.GetIp();
				Exceptions.Log(ex, ip, "ContentItemModel/Update");
				return false;
			}
		}

		public static string GetStringDetails(ContentItemModel contentItemModel)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine();
			sb.AppendLine();
			sb.Append("Sector: " + contentItemModel.Sector);
			sb.AppendLine();
			sb.Append("Title: " + contentItemModel.Title);
			sb.AppendLine();
			sb.Append("Description: " + contentItemModel.Description);
			sb.AppendLine();
			sb.AppendLine();
			sb.Append("Authors: " + contentItemModel.Authors);
			sb.AppendLine();
			sb.Append("Version: " + contentItemModel.Version);
			sb.AppendLine();
			sb.Append("Price: " + contentItemModel.Price.ToString("C"));
			sb.AppendLine();
			sb.Append("Pages: " + contentItemModel.Pages);
			sb.AppendLine();
			sb.Append("Created: " + contentItemModel.Created.ToShortDateString());
			sb.AppendLine();
			sb.Append("Updated: " + contentItemModel.Updated.ToShortDateString());
			sb.AppendLine();
			sb.Append("SKU: " + contentItemModel.ContentItemGuid);
			sb.AppendLine();
			sb.Append("Attachment: " + ConnexienStorageBlob.GetPathOfBlob(contentItemModel.Filename, ConnexienStorageContainers.StorageContainer.ContentItems));
			sb.AppendLine();
			return sb.ToString();
		}
	}
}
