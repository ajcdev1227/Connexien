using System;
using System.Web;

namespace Connexien.Web.Models
{
	using Connexien.Lib;
	using Connexien.Lib.Communication;
	using Connexien.Lib.Communication.EmailParameters;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.IO;
	using System.Linq;
	using System.Text;

	public class CustomFormRequestModel
	{
		public long ProfileId { get; set; }

		public string ProfileGuid { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		[StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters.")]
		public string Description { get; set; }

		[Required]
		[StringLength(1000, ErrorMessage = "Purpose cannot be longer than 1000 characters.")]
		public string Purpose { get; set; }

		[Required(ErrorMessage = "Please provide a comma separated list of the key fields for the form")]
		public string KeyFields { get; set; }

		[Required(ErrorMessage = "Please select a frequency of use")]
		public string FrequencyOfUse { get; set; }

		[Required(ErrorMessage = "Please select an industry")]
		public string Sector { get; set; }

		[Required(ErrorMessage = "Please selected a preferred format")]
		public string PreferredFormat { get; set; }

		[Required(ErrorMessage = "Please select an intended recipient")]
		public string IntendedRecipients { get; set; }

		public bool NeedsRecipientSignature { get; set; }

		public bool NeedsSupervisorSignature { get; set; }

		public string MimeType { get; set; }

		public string Filename { get; set; }

		public HttpPostedFileBase File { get; set; }

		[Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the Contributing Partner Terms and Conditions.")]
		public bool AcceptedTermsAndConditions { get; set; }

		public CustomFormRequestModel()
		{

		}

		public static bool Submit(CustomFormRequestModel model)
		{

			try
			{
				var userId = UserModel.GetUserId();

				using (var db = new ConnexienEntities())
				{

					var profile = db.Profiles.FirstOrDefault(x => x.Id == model.ProfileId && x.UserId == userId && x.Deleted == null);
					if (profile == null)
					{
						return false;
					}

					ICommunication<IEmailParameters> communication = new MailGunEmail<IEmailParameters>();

					IEmailParameters emailParameters = new CustomFormRequestEmailParameters(profile);
					IEmailParameters backendEmailParameters;

					string bodyText = $"A new Custom Form Request has been submitted by {profile.User.FirstName} {profile.User.LastName} : {profile.Email} {GetStringDetails(model)}";
					string subject = "Connexien Custom Form Request Program";

					if (model.File != null)
					{
						var file = model.File;
						var ext = Path.GetExtension(file.FileName);
						string filePath = null;
						model.Filename = Guid.NewGuid().ToString("N") + ext;
						if (file.ContentLength > 0)
						{
							filePath = Path.Combine($@"{Utilities.GetSiteRootDirectory()}", model.Filename);
							file.SaveAs(filePath);
							ConnexienStorageBlob.UploadFileToStorageBlob(file, ConnexienStorageContainers.StorageContainer.CustomForms, model.Filename);
						}

						bodyText = "Thank you for your submission! " +
							"We’re currently reviewing your request, and we’ll notify you via email within 2-3 business days with a quote for building your form.";
						bodyText += "\n\nIf you have any questions, please contact us at connect@connexien.com." +
							"\r\nSincerely,\r\nConnexien Team\r\n";
						//string filepath = "https://connexien.blob.core.windows.net/site-custom-form-request/" + model.Filename;
						backendEmailParameters = new ConnexienBackEndEmailParameters(
							bodyText: bodyText,
							emailAddress: profile.Email,
							subject: subject,
							attachments: new List<string>() { filePath }
						);
						//communication.SendMessage(emailParameters);
						communication.SendMessage(backendEmailParameters);
						System.IO.File.Delete(filePath);
					}
					else
					{
						backendEmailParameters = new ConnexienBackEndEmailParameters(
							bodyText: bodyText,
							emailAddress: profile.Email,
							subject: subject
						);
						//communication.SendMessage(emailParameters);
						communication.SendMessage(backendEmailParameters);
					}

					//communication.SendMessage(emailParameters);
					

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

		public static string GetStringDetails(CustomFormRequestModel customFormRequestModel)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine();
			sb.AppendLine();
			sb.Append("Sector: " + customFormRequestModel.Sector);
			sb.AppendLine();
			sb.Append("Title: " + customFormRequestModel.Title);
			sb.AppendLine();
			sb.Append("Description: " + customFormRequestModel.Description);
			sb.AppendLine();
			sb.AppendLine();
			sb.Append("Purpose: " + customFormRequestModel.Purpose);
			sb.AppendLine();
			sb.AppendLine();
			sb.Append("Key Fields: " + customFormRequestModel.KeyFields);
			sb.AppendLine();
			sb.Append("Frequency Of Use: " + customFormRequestModel.FrequencyOfUse);
			sb.AppendLine();
			sb.Append("Preferred Format: " + customFormRequestModel.PreferredFormat);
			sb.AppendLine();
			sb.Append("Intended Recipients: " + customFormRequestModel.IntendedRecipients);
			sb.AppendLine();
			sb.Append("Needs Recipient Signature: " + (customFormRequestModel.NeedsRecipientSignature ? "Yes" : "No"));
			sb.AppendLine();
			sb.Append("Needs Supervisor Signature: " + (customFormRequestModel.NeedsSupervisorSignature ? "Yes" : "No"));
			sb.AppendLine();
			if (!string.IsNullOrEmpty(customFormRequestModel.Filename))
			{
				sb.Append("Attachment: " + ConnexienStorageBlob.GetPathOfBlob(customFormRequestModel.Filename, ConnexienStorageContainers.StorageContainer.CustomForms));
				sb.AppendLine();
			}

			return sb.ToString();
		}
	}
}