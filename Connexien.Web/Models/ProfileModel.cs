using Connexien.Lib;
using Connexien.Lib.ProfileImages;
using Microsoft.Security.Application;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;

namespace Connexien.Web.Models
{
#pragma warning disable CA1819 // Properties should not return arrays
    public class ProfileModel
    {

        #region Properties

        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime Created { get; set; }
        public string Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Vanity { get; set; }
        public string ProfileGuid { get; set; }

        public List<PublicationModel> Articles { get; set; }
        public List<ContentItemModel> ContentItems { get; set; }

        public int ViewsToday { get; set; }
        public int ViewsTotal { get; set; }
        public int Reviews { get; set; }
        public decimal Rating { get; set; }
        public int AbsRating { get; set; }
        public bool DisplayReview { get; set; }

        public string AvatarPic { get; set; }

        public string CoverPic { get; set; }

        //----------------------------------------

        [Required]
        public string Title { get; set; }
        public string PersonalPhone { get; set; }
        public string PersonalFax { get; set; }
        [Required]
        public string PersonalEmail { get; set; }
        public string PersonalWebsite { get; set; }
        public string PersonalLinkedIn { get; set; }
        public string PersonalFacebook { get; set; }
        public string PersonalTwitter { get; set; }
        public string PersonalYelp { get; set; }

        public string CompanySize { get; set; }

        [Required]
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        public string Zip { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyFax { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyLinkedIn { get; set; }
        public string CompanyFacebook { get; set; }
        public string CompanyTwitter { get; set; }
        public string CompanyYelp { get; set; }

        [Required]
        public string Experience { get; set; }
        [Required]
        public string Engagements { get; set; }
        public string EdLevel { get; set; }
        public string SchoolName { get; set; }
        public string DegreeMajor { get; set; }
        public int YearGrad { get; set; }
        public bool LicenseBit { get; set; }
        public long[] Licenses { get; set; }
        public string[] LicenseTitles { get; set; }
        //public bool NasaaBit { get; set; }
        //public long[] Licenses2 { get; set; }
        public bool DesigBit { get; set; }
        public long[] Designations { get; set; }
        public string[] DesignationTitles { get; set; }
        public bool HistoryBit { get; set; }

        [Required]
        public string[] Focuses { get; set; }

        [Required]
        public string Availability { get; set; }
        public string CommunicationPref { get; set; }
        public bool AllStatesBit { get; set; }
        public string[] ServiceStates { get; set; }
        public string FeeType { get; set; }
        public string FeeTypeTitle { get; set; }
        public bool MinFee { get; set; }
        public bool FeeNegot { get; set; }
        public int? HourlyRate { get; set; }
        public string HourlyRateTitle { get; set; }

        public string ListedBy { get; set; }
        public long[] ServiceTypes { get; set; }
        public string[] ServiceTitles { get; set; }
        public long[] ProductTypes { get; set; }
        public string[] ProductTitles { get; set; }
        public long[] RulesRegs { get; set; }

        public string PaymentAccount { get; set; }
        public string PaymentAccountRefreshToken { get; set; }

        public string Summary { get; set; }
        #endregion

        public ProfileModel()
        {

        }

		public static string Extension(string mimeType)
		{
			var key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
			var value = key?.GetValue("Extension", null);

			return value?.ToString();
		}

		public ProfileModel(Profile profile)
        {

            #region Profile

            Id = profile.Id;
            UserId = profile.UserId;
            Vanity = profile.Vanity;
            ProfileGuid = profile.Guid;
            Type = profile.Type;
            Created = profile.Created;

            Articles = profile.ProfileArticles.Select(x => new PublicationModel(x)).ToList();
            ContentItems = profile.ContentItems.Select(x => new ContentItemModel(x)).ToList();

            FirstName = profile.User.FirstName;
            LastName = profile.User.LastName;
            Email = profile.User.Email;

            ViewsToday = profile.ProfileViews.Count(x => x.Created > DateTime.Today && x.Type == "profile");
            ViewsTotal = profile.ProfileViews.Count(x => x.Type == "profile");
            Reviews = profile.ProfileRatings.Count();
            Rating = Reviews != 0 ? profile.ProfileRatings.Average(x => x.Rating) : 0;
            AbsRating = (int)Math.Round(Rating, 0);
            DisplayReview = profile.UserId != UserModel.GetUserId() && !HasReview(profile.Id, UserModel.GetUserId(), UserModel.GetIp());

            var av = profile.ProfileImages.FirstOrDefault(x => x.Type == "avatar");
            AvatarPic = av == null ? "default.jpg" : av.Guid + Extension(av.MimeType);// == "image/png" ? ".png" :".jpg");

            var cov = profile.ProfileImages.FirstOrDefault(x => x.Type == "cover");
            CoverPic = cov == null ? "default-cover.jpg" : cov.Guid + Extension(cov.MimeType);//(av.MimeType == "image/png" ? ".png" : ".jpg");

			Summary = Sanitizer.GetSafeHtmlFragment(profile.Summary);
            #endregion

            #region Personal

            Title = profile.Title;
            PersonalPhone = profile.Phone;
            PersonalFax = profile.Fax;
            PersonalEmail = profile.Email;

            foreach (var link in profile.ProfileLinks)
            {
                switch (link.Type.ToLower())
                {
                    case "website":
                        PersonalWebsite = link.Url;
                        break;
                    case "linkedin":
                        PersonalLinkedIn = link.Url;
                        break;
                    case "facebook":
                        PersonalFacebook = link.Url;
                        break;
                    case "twitter":
                        PersonalTwitter = link.Url;
                        break;
                    case "yelp":
                        PersonalYelp = link.Url;
                        break;

                }
            }

            #endregion

            #region Company

            CompanySize = profile.Company.Size;
            CompanyName = profile.Company.Name;
            Address1 = profile.Company.Address1;
            Address2 = profile.Company.Address2;
            City = profile.Company.City;
            State = profile.Company.State;
            Zip = profile.Company.Zip;
            CompanyPhone = profile.Company.Phone;
            CompanyFax = profile.Company.Fax;

            foreach (var link in profile.Company.CompanyLinks)
            {
                switch (link.Type.ToLower())
                {
                    case "website":
                        CompanyWebsite = link.Url;
                        break;
                    case "linkedin":
                        CompanyLinkedIn = link.Url;
                        break;
                    case "facebook":
                        CompanyFacebook = link.Url;
                        break;
                    case "twitter":
                        CompanyTwitter = link.Url;
                        break;
                    case "yelp":
                        CompanyYelp = link.Url;
                        break;

                }
            }

            #endregion

            PaymentAccount = profile.PaymentAccount;
            PaymentAccountRefreshToken = profile.PaymentAccountRefreshToken;

            if (profile.Type != "provider")
            {
                return;
            }

            #region Experience

            Experience = profile.YearsExp;
            Engagements = profile.Engagements;
            EdLevel = profile.EdLevel;
            SchoolName = profile.SchoolName;
            DegreeMajor = profile.DegreeMajor;
            YearGrad = profile.YearGrad ?? 0;
            LicenseBit = profile.ProfileLicenses.Any();
            Licenses = profile.ProfileLicenses.Select(x => x.LicenseId).ToArray();
            LicenseTitles = profile.ProfileLicenses.Select(x => x.License.Name).ToArray();
            DesigBit = profile.ProfileDesignations.Any();
            Designations = profile.ProfileDesignations.Select(x => x.DesignationId).ToArray();
            DesignationTitles = profile.ProfileDesignations.Select(x => x.Designation.Name).ToArray();
            HistoryBit = profile.DisciplinaryHistory;

            #endregion

            #region Industry

            Focuses = profile.ServiceFocus.Split(',').Select(x => x.Trim()).ToArray();
            Availability = profile.Availability;
            CommunicationPref = profile.CommunicationPref;
            AllStatesBit = profile.ServiceStates.ToLower() == "all";
            ServiceStates = profile.ServiceStates.ToLower() == "all"
                                ? new string[0]
                                : profile.ServiceStates.Split(',').Select(x => x.Trim()).ToArray();
            FeeType = profile.FeeType;
            FeeTypeTitle = profile.FeeType == "hourly"
                               ? "Hourly Rate"
                               : (profile.FeeType == "flat" ? "Flat Rate" : "Hourly or Flat Rate");
            FeeNegot = profile.FeeNegot;
            MinFee = profile.MinFee;
            HourlyRate = profile.HourlyRate;

            switch (profile.HourlyRate)
            {
                case 25:
                    HourlyRateTitle = "$25‐$50 per hour";
                    break;
                case 50:
                    HourlyRateTitle = "$50‐$100 per hour";
                    break;
                case 100:
                    HourlyRateTitle = "$100‐$200 per hour";
                    break;
                case 200:
                    HourlyRateTitle = "$200‐$300 per hour";
                    break;
                case 300:
                    HourlyRateTitle = "$300‐$400 per hour";
                    break;
                case 400:
                    HourlyRateTitle = "$400‐$450 per hour";
                    break;
                case 450:
                    HourlyRateTitle = "$450‐$500 per hour";
                    break;
                case 500:
                    HourlyRateTitle = "$500+ per hour (state billable rate)";
                    break;
            }

            #endregion

            #region Expertise

            ListedBy = profile.ProfileServiceTypes.Any() && profile.ProfileProductTypes.Any()
                           ? "both"
                           : (profile.ProfileServiceTypes.Any()
                                  ? "service"
                                  : (profile.ProfileProductTypes.Any() ? "product" : "both"));
            ServiceTypes = profile.ProfileServiceTypes.Select(x => x.ServiceTypeId).ToArray();
            ServiceTitles = profile.ProfileServiceTypes.Select(x => x.ServiceType.SubCategory).ToArray();
            ProductTypes = profile.ProfileProductTypes.Select(x => x.ProductTypeId).ToArray();
            ProductTitles = profile.ProfileProductTypes.Select(x => x.ProductType.SubCategory).ToArray();

            #endregion

        }

        public static bool Save(ProfileModel model, ProfileType profileType)
        {
            var userId = UserModel.GetUserId();

            using (var db = new ConnexienEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.Id == userId);

                if (user == null)
                {
                    return false;
                }

                #region Company

                var company = new Company
                {
                    Created = DateTime.UtcNow,
                    Name = model.CompanyName,
                    Size = model.CompanySize,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    City = model.City,
                    State = model.State,
                    Zip = model.Zip,
                    Phone = model.CompanyPhone,
                    Fax = model.CompanyFax
                };

                if (!string.IsNullOrWhiteSpace(model.CompanyWebsite))
                {
                    company.CompanyLinks.Add(new CompanyLink { Created = DateTime.UtcNow, Type = "website", Url = model.CompanyWebsite });
                }

                db.Companies.Add(company);
                db.SaveChanges();

                #endregion



                var profile = new Profile();

                switch (profileType)
                {
                    case ProfileType.Provider:
                        var guid = Guid.NewGuid().ToString();

                        profile = new Profile
                        {
                            UserId = userId,
                            CompanyId = company.Id,
                            Created = DateTime.UtcNow,
                            Title = model.Title,
                            Phone = model.PersonalPhone,
                            Fax = model.PersonalFax,
                            Email = model.PersonalEmail,
                            YearsExp = model.Experience,
                            Engagements = model.Engagements,
                            EdLevel = model.EdLevel,
                            SchoolName = model.SchoolName,
                            DegreeMajor = model.DegreeMajor,
                            YearGrad = model.YearGrad,
                            DisciplinaryHistory = model.HistoryBit,
                            ServiceFocus = string.Join(", ", model.Focuses),
                            Availability = model.Availability,
                            CommunicationPref = model.CommunicationPref,
                            ServiceStates = model.AllStatesBit || model.ServiceStates == null ? "all" : string.Join(", ", model.ServiceStates),
                            FeeType = model.FeeType,
                            FeeNegot = model.FeeNegot,
                            HourlyRate = model.HourlyRate,
                            Type = "provider",
                            Vanity = guid,
                            Guid = guid
                        };

                        #region Licenses

                        if (model.LicenseBit)
                        {
                            foreach (var l in model.Licenses)
                            {
                                profile.ProfileLicenses.Add(new ProfileLicens { ProfileId = model.Id, LicenseId = l, Created = DateTime.UtcNow });
                            }
                        }

                        #endregion

                        #region Designations

                        if (model.DesigBit)
                        {
                            foreach (var d in model.Designations)
                            {
                                profile.ProfileDesignations.Add(new ProfileDesignation { ProfileId = model.Id, DesignationId = d, Created = DateTime.UtcNow });
                            }
                        }

                        #endregion

                        #region Services

                        if (model.ListedBy == "service" || model.ListedBy == "both")
                        {
                            foreach (var s in model.ServiceTypes)
                            {
                                profile.ProfileServiceTypes.Add(new ProfileServiceType { ServiceTypeId = s });
                            }
                        }

                        #endregion

                        #region Products

                        if (model.ListedBy == "product" || model.ListedBy == "both")
                        {
                            foreach (var p in model.ProductTypes)
                            {
                                profile.ProfileProductTypes.Add(new ProfileProductType { ProductTypeId = p });
                            }
                        }

                        #endregion

                        break;
                    case ProfileType.ContentPartner:
                        profile = new Profile
                        {
                            UserId = userId,
                            CompanyId = company.Id,
                            Created = DateTime.UtcNow,
                            Title = model.Title,
                            Phone = model.PersonalPhone,
                            Fax = model.PersonalFax,
                            Email = model.PersonalEmail,
                            Type = "contentpartner",
                            Vanity = Guid.NewGuid().ToString()
                        };

                        break;
                    default:
                    case ProfileType.Member:
                        profile = new Profile
                        {
                            UserId = userId,
                            CompanyId = company.Id,
                            Created = DateTime.UtcNow,
                            Title = model.Title,
                            Phone = model.PersonalPhone,
                            Fax = model.PersonalFax,
                            Email = model.PersonalEmail,
                            Type = "member",
                            Vanity = Guid.NewGuid().ToString()
                        };

                        break;
                }

                if (!string.IsNullOrWhiteSpace(model.PersonalWebsite))
                {
                    profile.ProfileLinks.Add(new ProfileLink { Created = DateTime.UtcNow, Type = "website", Url = model.PersonalWebsite });
                }

                if (!string.IsNullOrWhiteSpace(model.PersonalLinkedIn))
                {
                    profile.ProfileLinks.Add(new ProfileLink { Created = DateTime.UtcNow, Type = "linkedin", Url = model.PersonalLinkedIn });
                }

                profile.PaymentAccount = model.PaymentAccount;
                profile.PaymentAccountRefreshToken = model.PaymentAccountRefreshToken;

                profile.Summary = Sanitizer.GetSafeHtmlFragment(model.Summary);

                user.Profiles.Add(profile);
                db.SaveChanges();
            }

            return true;
        }

        public static bool Update(string section, ProfileModel model)
        {
            var userId = UserModel.GetUserId();

            using (var db = new ConnexienEntities())
            {
                var profile = db.Profiles.FirstOrDefault(x => x.Id == model.Id && x.UserId == userId && x.Deleted == null);
                if (profile == null)
                {
                    return false;
                }

                switch (section)
                {
                    case "Personal":

                        #region Personal

                        profile.Title = model.Title;
                        profile.Phone = model.PersonalPhone;
                        profile.Fax = model.PersonalFax;
                        profile.Email = model.PersonalEmail;

                        var lnk = profile.ProfileLinks.FirstOrDefault(x => x.Type == "linkedin");
                        if (lnk != null)
                        {
                            lnk.Url = model.PersonalLinkedIn;
                        }
                        else
                        {
                            profile.ProfileLinks.Add(new ProfileLink
                            {
                                Created = DateTime.UtcNow,
                                Type = "linkedin",
                                Url = model.PersonalLinkedIn
                            });
                        }

                        profile.Summary = Sanitizer.GetSafeHtmlFragment(model.Summary);
                        #endregion

                        break;
                    case "Company":

                        #region Company

                        profile.Company.Size = model.CompanySize;
                        profile.Company.Name = model.CompanyName;
                        profile.Company.Address1 = model.Address1;
                        profile.Company.Address2 = model.Address2;
                        profile.Company.City = model.City;
                        profile.Company.State = model.State;
                        profile.Company.Zip = model.Zip;
                        profile.Company.Phone = model.CompanyPhone;
                        profile.Company.Fax = model.CompanyFax;

                        var ws = profile.Company.CompanyLinks.FirstOrDefault(x => x.Type == "website");
                        if (ws != null)
                        {
                            ws.Url = model.CompanyWebsite;
                        }
                        else
                        {
                            profile.Company.CompanyLinks.Add(new CompanyLink
                            {
                                Created = DateTime.UtcNow,
                                Type = "website",
                                Url = model.CompanyWebsite
                            });
                        }

                        #endregion

                        break;
                    case "Experience":

                        #region Experience

                        profile.SchoolName = model.SchoolName;
                        profile.YearsExp = model.Experience;
                        profile.Engagements = model.Engagements;
                        profile.EdLevel = model.EdLevel;
                        profile.DegreeMajor = model.DegreeMajor;
                        profile.YearGrad = model.YearGrad;

                        //-----------------------------------

                        db.Database.ExecuteSqlCommand("DELETE FROM ProfileLicenses WHERE ProfileId = {0}", model.Id);

                        if (model.LicenseBit)
                        {
                            foreach (var l in model.Licenses)
                            {
                                profile.ProfileLicenses.Add(new ProfileLicens { ProfileId = model.Id, LicenseId = l, Created = DateTime.UtcNow });
                            }
                        }

                        //-----------------------------------

                        db.Database.ExecuteSqlCommand("DELETE FROM ProfileDesignations WHERE ProfileId = {0}", model.Id);

                        if (model.DesigBit)
                        {
                            foreach (var d in model.Designations)
                            {
                                profile.ProfileDesignations.Add(new ProfileDesignation { ProfileId = model.Id, DesignationId = d, Created = DateTime.UtcNow });
                            }
                        }

                        //-----------------------------------

                        profile.DisciplinaryHistory = model.HistoryBit;

                        #endregion

                        break;
                    case "Industry":

                        #region Industry

                        profile.ServiceFocus = string.Join(", ", model.Focuses);
                        profile.Availability = model.Availability;
                        profile.CommunicationPref = model.CommunicationPref;
                        profile.ServiceStates = model.AllStatesBit ? "all" : string.Join(", ", model.ServiceStates);
                        profile.FeeType = model.FeeType;
                        profile.FeeNegot = model.FeeNegot;
                        profile.HourlyRate = model.HourlyRate;

                        #endregion

                        break;
                    case "Expertise":

                        #region Expertise

                        db.Database.ExecuteSqlCommand("DELETE FROM ProfileServiceTypes WHERE ProfileId = {0}", model.Id);

                        if (model.ListedBy == "service" || model.ListedBy == "both")
                        {
                            foreach (var s in model.ServiceTypes)
                            {
                                profile.ProfileServiceTypes.Add(new ProfileServiceType { ProfileId = model.Id, ServiceTypeId = s });
                            }
                        }

                        //-----------------------------------

                        db.Database.ExecuteSqlCommand("DELETE FROM ProfileProductTypes WHERE ProfileId = {0}", model.Id);

                        if (model.ListedBy == "product" || model.ListedBy == "both")
                        {
                            foreach (var p in model.ProductTypes)
                            {
                                profile.ProfileProductTypes.Add(new ProfileProductType { ProfileId = model.Id, ProductTypeId = p });
                            }
                        }

                        #endregion

                        break;
                    case "Profile":

                        #region Profile

                        profile.Vanity = model.Vanity;

                        #endregion

                        break;
                    case "ContentPartner":
                        #region ContentPartner

                        profile.PaymentAccount = model.PaymentAccount;
                        profile.PaymentAccountRefreshToken = model.PaymentAccountRefreshToken;

                        #endregion
                        break;
                }

                db.SaveChanges();

            }

            return true;
        }

        public static bool SavePic(HttpPostedFileBase file, long id, string type)
        {
            try
            {
                var userId = UserModel.GetUserId();

                using (var db = new ConnexienEntities())
                {

                    var profile = db.Profiles.FirstOrDefault(x => x.Id == id && x.UserId == userId && x.Deleted == null);
                    if (profile == null || file == null || file.ContentLength == 0)
                    {
                        return false;
                    }

                    #region Upload Pic

                    var ext = Path.GetExtension(file.FileName);
                    var img = new ProfileImage
                    {
                        Created = DateTime.UtcNow,
                        Type = type,
                        Guid = Guid.NewGuid().ToString(),
                        MimeType = Utilities.MimeTypeMap.GetMimeType(ext)
                    };

                    img.Filename = $"{img.Guid}{ext}";
                    Dictionary<string, string> serviceSettings = ConfigurationManager.AppSettings.AllKeys.ToDictionary(x => x, x => ConfigurationManager.AppSettings[x]);
                    ConnexienStorageBlob.UploadFileToStorageBlob(file, ConnexienStorageContainers.StorageContainer.ProfileImages, img.Filename, serviceSettings);

                    var profileImage = profile.ProfileImages.FirstOrDefault(x => x.Type == type && x.ProfileId == id);

                    if (profileImage != null)
                    {
                        profileImage.Filename = img.Filename;
                        profileImage.Modified = DateTime.UtcNow;
                        profileImage.Guid = img.Guid;
                        profileImage.MimeType = img.MimeType;
						db.ProfileImages.AddOrUpdate(profileImage);

					}
                    else
                    {
                        profile.ProfileImages.Add(img);
                    }
                    db.SaveChanges();

                    #endregion

                    System.Threading.Thread.Sleep(10000);

                    return true;
                }

            }
            catch (Exception ex)
            {

                var ip = UserModel.GetIp();
                Exceptions.Log(ex, ip, "SavePic");
                return false;
            }
        }

        public static bool RemovePic(string id)
        {
            try
            {
                id = Path.GetFileNameWithoutExtension(id);

                var userId = UserModel.GetUserId();

                using (var db = new ConnexienEntities())
                {

                    var pic = db.ProfileImages.FirstOrDefault(x => x.Guid == id && x.Profile.UserId == userId);
                    if (pic == null)
                    {
                        return false;
                    }

                    Dictionary<string, string> serviceSettings = ConfigurationManager.AppSettings.AllKeys.ToDictionary(x => x, x => ConfigurationManager.AppSettings[x]);

                    ConnexienStorageBlob.DeleteFileFromStorageBlob(pic.Filename, ConnexienStorageContainers.StorageContainer.ProfileImages, serviceSettings);


                    foreach (var resizedImage in ProfileImageUtilities.GetResizedImageFiles(pic.Filename))
                    {
                        ConnexienStorageBlob.DeleteFileFromStorageBlob(resizedImage.FileName, ConnexienStorageContainers.StorageContainer.ProfileImagesResized, serviceSettings);
                    }

                    db.ProfileImages.Remove(pic);
                    db.SaveChanges();

                    return true;
                }

            }
            catch (Exception ex)
            {

                var ip = UserModel.GetIp();
                Exceptions.Log(ex, ip, "RemovePic");
                return false;
            }
        }

        //-------------------------------------------------------------

        public static bool Rate(string guid, int rating)
        {

            try
            {

                var userId = UserModel.GetUserId();
                var userIp = UserModel.GetIp();

                using (var db = new ConnexienEntities())
                {
                    var profile = db.Profiles.FirstOrDefault(x => x.Guid == guid && x.Deleted == null);
                    if (profile == null)
                    {
                        return false;
                    }

                    var rec = new ProfileRating
                    {
                        ProfileId = profile.Id,
                        Created = DateTime.UtcNow,
                        Rating = rating,
                        IP = userIp
                    };

                    if (userId != 0)
                    {
                        rec.UserId = userId;
                    }

                    db.ProfileRatings.Add(rec);
                    db.SaveChanges();

                    return true;
                }

            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, UserModel.GetIp(), "Profile Rate");
                return false;
            }
        }

        private static bool HasReview(long id, long userId, string ip)
        {
            using (var db = new ConnexienEntities())
            {
                return db.ProfileRatings.Any(x => x.ProfileId == id && (x.UserId == userId || (x.UserId == null && x.IP == ip)));
            }
        }

        //-------------------------------------------------------------

        public static bool VanityCheck(string vanity, long id)
        {
            using (var db = new ConnexienEntities())
            {
                return db.Profiles.Any(x => x.Id != id && x.Vanity.ToLower() == vanity.ToLower());
            }
        }

    }

    public enum ProfileType
    {
        Member = 0,
        Provider = 1,
        ContentPartner = 2
    }
#pragma warning restore CA1819 // Properties should not return arrays
}
