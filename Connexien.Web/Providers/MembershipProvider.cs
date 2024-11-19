using System;
using System.Collections.Specialized;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using Connexien.Lib;
using Connexien.Web.Models;


namespace Connexien.Web.Providers
{

    public sealed class ConnexienMembershipProvider : MembershipProvider
    {

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            try
            {
                Exceptions.Info($"username: {username} | password: {password} | email: {email} | passwordQuestion: {passwordQuestion} | passwordAnswer: {passwordAnswer}");

                using (var db = new ConnexienEntities())
                {
                    if (db.Users.Any(x => x.Email.ToLower() == email))
                    {
                        status = MembershipCreateStatus.DuplicateEmail;
                    }
                    else
                    {
                        var user = new User
                        {
                            Created = DateTime.UtcNow,
                            FirstName = passwordQuestion,
                            LastName = passwordAnswer,
                            Email = email,
                            Password = Encryption.EncryptText(password),
                            Ip = username
                        };
                        db.Users.Add(user);
                        db.SaveChanges();

                        status = MembershipCreateStatus.Success;
                    }

                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Exceptions.Log(e, username, string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));

                    foreach (var ve in eve.ValidationErrors)
                    {
                        Exceptions.Log(e, username, string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage));
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                status = MembershipCreateStatus.ProviderError;
                Exceptions.Log(ex, username, "Membership.CreateUser: " + passwordAnswer + " " + passwordQuestion + " " + email);
            }

            return null;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                using (var db = new ConnexienEntities())
                {
                    var pwd = Encryption.EncryptText(newPassword);

                    User user = db.Users.FirstOrDefault(x => x.Email.ToLower() == username.ToLower());

                    if (user == null)
                    {
                        return false;
                    }

                    user.Password = pwd;
                    db.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, UserModel.GetIp(), "Membership.ChangePassword: " + username);
            }

            return false;
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string email, string password)
        {
            try
            {
                using (var db = new ConnexienEntities())
                {
                    var pwd = Encryption.EncryptText(password);
                    return db.Users.Any(x => x.Email.ToLower() == email.ToLower() && x.Password == pwd);
                }
            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, UserModel.GetIp(), "Membership.ValidateUser: " + email + " " + password);
            }

            return false;
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            try
            {
                using (var db = new ConnexienEntities())
                {
                    var user = db.Users.FirstOrDefault(x => x.Email.ToLower() == username.ToLower());

                    if (user != null)
                    {
                        return new MembershipUser("ConnexienMembershipProvider",
                                                  user.Email,
                                                  user.Id,
                                                  user.Email,
                                                  String.Empty,
                                                  String.Empty,
                                                  true,
                                                  false,
                                                  user.Created,
                                                  DateTime.Now,
                                                  DateTime.Now,
                                                  DateTime.Now,
                                                  DateTime.Now);
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, UserModel.GetIp(), "Membership.GetUser: " + username);
            }

            return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            try
            {
                using (var db = new ConnexienEntities())
                {
                    return db.Users.Where(x => x.Email.ToLower() == email.ToLower())
                                   .Select(x => x.Email.ToLower()).DefaultIfEmpty(String.Empty).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, UserModel.GetIp(), "Membership.GetUserNameByEmail: " + email);
            }

            return String.Empty;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override string ApplicationName { get; set; }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        #region Encryption

        public class Encryption
        {

            // Encrypt the text
            public static string EncryptText(string strText)
            {
                return Encrypt(strText, "C%o*0&N@N,X#I3N?:");
            }

            //Decrypt the text
            public static string DecryptText(string strText)
            {
                return Decrypt(strText, "C%o*0&N@N,X#I3N?:");
            }

            //The function used to encrypt the text
            private static string Encrypt(string strText, string strEncrKey)
            {
                byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };

                try
                {
                    var byKey = Encoding.UTF8.GetBytes(strEncrKey.Substring(0, 8));

                    var des = new DESCryptoServiceProvider();
                    var inputByteArray = Encoding.UTF8.GetBytes(strText);
                    var ms = new MemoryStream();
                    var cs = new CryptoStream(ms, des.CreateEncryptor(byKey, iv), CryptoStreamMode.Write);

                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();

                    return Convert.ToBase64String(ms.ToArray());
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

            }

            //The function used to decrypt the text
            private static string Decrypt(string strText, string sDecrKey)
            {
                byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };

                try
                {
                    var byKey = Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));
                    var des = new DESCryptoServiceProvider();
                    var inputByteArray = Convert.FromBase64String(strText);

                    var ms = new MemoryStream();
                    var cs = new CryptoStream(ms, des.CreateDecryptor(byKey, iv), CryptoStreamMode.Write);

                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();

                    var encoding = Encoding.UTF8;

                    return encoding.GetString(ms.ToArray());
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

        }

        #endregion
    }
}
