using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Connexien.Lib.Auth
{
    public class LinkedIn
    {
        public string Action { get; set; }
        public string SocialId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string code { get; set; }
        public string state { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }

        //---------------------------------

        private const string LiClientId = "75bv1fxbfw6qle";
        private const string LiSecret = "dyEYcI6CfGiljGu3";
        private const string LiScope = "r_basicprofile%20r_emailaddress"; //"r_fullprofile%20r_emailaddress%20r_contactinfo";

        //---------------------------------

        public static string GetAuthUrl(string liState, string action, string liRedirect)
        {
            return "https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id=" + LiClientId + "&redirect_uri=" + liRedirect + action + "&state=" + liState + "&scope=" + LiScope;
        }

        public static LinkedIn GetAuthToken(LinkedIn model, string liRedirect)
        {

            try
            {
                var request = WebRequest.Create("https://www.linkedin.com/uas/oauth2/accessToken");
                request.Method = "POST";
                var postData = "grant_type=authorization_code&code=" + model.code + "&redirect_uri=" + liRedirect + model.Action + "&client_id=" + LiClientId + "&client_secret=" + LiSecret;
                var byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;

                var dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                var response = request.GetResponse();
                dataStream = response.GetResponseStream();
                var reader = new StreamReader(dataStream);

                var reply = reader.ReadToEnd();

                reader.Close();
                dataStream.Close();
                response.Close();

                var o = JObject.Parse(reply);
                model.access_token = (string)o["access_token"];
                model.expires_in = (int)o["expires_in"];

                return model;

            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, "LinkedIn GetAuthToken");
                return model;
            }
        }

        public static LinkedIn GetIdentity(LinkedIn model)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + model.access_token);
                    var url = "https://api.linkedin.com/v1/people/~:(id,email-address,first-name,last-name)?format=json";
                    var html = client.DownloadString(url);

                    var o = JObject.Parse(html);
                    model.SocialId = (string)o["id"];
                    model.Email = (string)o["emailAddress"];
                    model.FirstName = (string)o["firstName"];
                    model.LastName = (string)o["lastName"];

                    return model;
                }
            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, "LinkedIn Get Identity");
                return model;
            }

        }

        public static string GetBasicProfile(LinkedIn model)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + model.access_token);
                    var url = "https://api.linkedin.com/v1/people/~:(id,picture-urls::(original),first-name,last-name,email-address,location,industry,headline,summary,specialties,positions,public-profile-url)?format=json";
                    var html = client.DownloadString(url);

                    return html;
                }
            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, "LinkedIn GetBasicProfile");
                return string.Empty;
            }

        }

        //---------------------------------

        public static string GetPic(long userId)
        {

            try
            {
                using (var db = new ConnexienEntities())
                {
                    var social = db.SocialLogins.FirstOrDefault(x => x.UserId == userId && x.SocialName == "linkedin");

                    if (social == null) return string.Empty;

                    using (var client = new WebClient())
                    {
                        client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + social.Token);
                        var url = "https://api.linkedin.com/v1/people/~:(picture-urls::(original))?format=json";
                        var html = client.DownloadString(url);

                        var o = JObject.Parse(html);
                        if (o["pictureUrls"] != null && (int)o["pictureUrls"]["_total"] > 0)
                            return (string)o["pictureUrls"]["values"][0];
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, "LinkedIn Get Pic");
            }

            return string.Empty;

        }

        public static bool SetupProfile(long userId)
        {
            try
            {
                using (var db = new ConnexienEntities())
                {
                    var social = db.SocialLogins.FirstOrDefault(x => x.SocialName == "linkedin" && x.UserId == userId && x.Expires > DateTime.UtcNow);
                    if (social == null) return false;

                    var o = JObject.Parse(GetBasicProfile(new LinkedIn { access_token = social.Token }));
                    /*
                    #region Profile

                    var profile = db.tblProfiles.FirstOrDefault(x => x.UserId == userId && x.Deleted == null);

                    if (profile == null)
                    {
                        profile = new tblProfile
                        {
                            UserId = userId,
                            Created = DateTime.UtcNow,
                            FirstName = (string)o["firstName"],
                            LastName = (string)o["lastName"],
                            Email = (string)o["emailAddress"],
                            Title = (string)o["headline"],
                            Searchable = true,
                            Relocate = false
                        };

                        var ind = (string)o["industry"];
                        var industry = db.tblIndustries.FirstOrDefault(x => x.Description.ToLower() == ind.ToLower());
                        if (industry != null) profile.IndustryId = industry.Id;

                        if (o["location"] != null)
                        {
                            var loc = (string)o["location"]["name"];
                            var country = (string)o["location"]["country"]["code"];

                            profile.Country = country;

                            var location = db.tblLocations.FirstOrDefault(x => loc.ToLower().Contains(x.City.ToLower()));
                            if (location != null)
                            {
                                profile.LocationId = location.Id;
                            }
                        }

                        db.tblProfiles.AddObject(profile);
                        db.SaveChanges();

                        profile.Vanity = profile.Id.ToString(CultureInfo.InvariantCulture);
                    }

                    db.ExecuteStoreCommand("DELETE FROM tblWebLinks WHERE ProfileId=" + profile.Id + " AND Url LIKE '%linkedin%';");
                    db.tblWebLinks.AddObject(new tblWebLink { ProfileId = profile.Id, Url = (string)o["publicProfileUrl"] });
                    db.SaveChanges();

                    #endregion

                    #region Experience

                    if (o["positions"] != null && (int)o["positions"]["_total"] > 0)
                    {
                        for (var i = 0; i < (int)o["positions"]["_total"]; i++)
                        {
                            var p = o["positions"]["values"][i];
                            db.tblExperiences.AddObject(new tblExperience
                            {
                                ProfileId = profile.Id,
                                Title = (string)p["title"],
                                Company = (string)p["company"]["name"],
                                Started = (int)p["startDate"]["month"] + "/" + (int)p["startDate"]["year"],
                                Ended = (bool)p["isCurrent"] ? "Now" : (int)p["endDate"]["month"] + "/" + (int)p["endDate"]["year"],
                                Current = (bool)p["isCurrent"],
                                Description = (string)p["summary"]
                            });
                        }

                        db.SaveChanges();
                    }

                    #endregion

                    #region Picture

                    if (o["pictureUrls"] != null && (int)o["pictureUrls"]["_total"] > 0)
                    {
                        var url = (string)o["pictureUrls"]["values"][0];

                        var pic = db.tblPics.FirstOrDefault(x => x.ProfileId == profile.Id && x.Deleted == null);

                        if (!string.IsNullOrWhiteSpace(url))
                        {

                            if (pic == null)
                            {
                                pic = new tblPic { ProfileId = profile.Id, Guid = Guid.NewGuid(), Created = DateTime.UtcNow };
                                db.tblPics.AddObject(pic);
                                db.SaveChanges();
                            }
                            else
                            {
                                Wistia.Data.DeleteMedia(pic.WistiaId);
                            }

                            var webClient = new WebClient();
                            var imageBytes = webClient.DownloadData(url);
                            webClient.Dispose();

                            var upload = new Wistia.Upload(new MemoryStream(imageBytes), "LinkedInPic.jpg", pic.Guid.ToString(), "pic");
                            pic.WistiaId = upload.HashedId;
                            pic.Url = upload.Thumbnail;
                            db.SaveChanges();

                            if (profile.tblVids.Any(x => x.Deleted == null))
                            {
                                var vid = profile.tblVids.First(x => x.Deleted == null);
                                vid.PicId = pic.Id;
                                Wistia.Data.UpdateThumbnail(vid.WistiaId, pic.WistiaId);
                            }
                        }

                    }

                    #endregion

                    #region Notes

                    db.tblNotes.AddObject(new tblNote { ProfileId = profile.Id, Note = (string)o["summary"] });

                    #endregion

                    db.SaveChanges();
                    */
                    return true;
                }
            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, "LinkedIn SetupProfile for UserId: " + userId);
                return false;
            }
        }

    }

}
