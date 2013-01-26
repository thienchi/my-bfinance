using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Mail;

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm
{
    /// <summary>
    /// Summary description for UtiMailService
    /// </summary>
    [WebService(Namespace = "http://dangdongcmm.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [ScriptService]
    public class UtiMailService : System.Web.Services.WebService
    {
        public string LANG = CCommon.LANG;

        #region private methods
        private string Gettemplate(string code)
        {
            try
            {
                string template = "";
                string filepath = HttpContext.Current.Server.MapPath("/xhtml/" + LANG + "/" + code + ".htm");
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.StreamReader sw = new System.IO.StreamReader(filepath, System.Text.UTF8Encoding.UTF8, false);
                    template = sw.ReadToEnd();
                    sw.Close();
                }
                return template;
            }
            catch
            {
                return "";
            }
        }
        private Settingsite.MailServer Getmailserver()
        {
            try
            {
                Settingsite.MailServer info = (new CMailServer()).Getinfo(1);
                info.UseSSL = (info.SMTPPort == 465 || info.SMTPPort == 587) ? 1 : 0;
                info.Password = CFunctions.MBDecrypt(info.Password);
                return info;
            }
            catch
            {
                return null;
            }
        }
        private bool SendMailToServer(string emailfr, string subject, string content)
        {
            try
            {
                Settingsite.MailServer info = this.Getmailserver();
                MailMessage mm = new MailMessage();
                mm.Subject = subject;
                mm.Body = content;
                mm.BodyEncoding = System.Text.Encoding.UTF8;
                mm.IsBodyHtml = true;
                mm.From = new MailAddress(CFunctions.IsNullOrEmpty(emailfr) ? info.Username : emailfr);
                mm.To.Add(CFunctions.IsNullOrEmpty(info.Receiver) ? info.Username : info.Receiver);
                mm.ReplyTo = new MailAddress(CFunctions.IsNullOrEmpty(emailfr) ? info.Username : emailfr);

                SmtpClient client = new SmtpClient(info.SMTPServer, info.SMTPPort);
                client.Credentials = new System.Net.NetworkCredential(info.Username, info.Password);
                client.EnableSsl = info.UseSSL == 1;
                client.Send(mm);

                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool SendMailToUser(string emailto, string subject, string content)
        {
            try
            {
                Settingsite.MailServer info = this.Getmailserver();
                MailMessage mm = new MailMessage();
                mm.Subject = subject;
                mm.Body = content;
                mm.BodyEncoding = System.Text.Encoding.UTF8;
                mm.IsBodyHtml = true;
                mm.From = new MailAddress(CFunctions.IsNullOrEmpty(info.Receiver) ? info.Username : info.Receiver);
                if (emailto.Split(',').Length > 1)
                    mm.Bcc.Add(emailto);
                else
                    mm.To.Add(emailto);
                mm.ReplyTo = new MailAddress(CFunctions.IsNullOrEmpty(info.Receiver) ? info.Username : info.Receiver);

                SmtpClient client = new SmtpClient(info.SMTPServer, info.SMTPPort);
                client.Credentials = new System.Net.NetworkCredential(info.Username, info.Password);
                client.EnableSsl = info.UseSSL == 1;
                client.Send(mm);

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        [WebMethod]
        public string SayHelloJSON(string notice, string callback)
        {
            return callback + "({ \"say\" : \"Hello " + notice + "\" })";
        }

        [WebMethod]
        public string SayHello(string notice)
        {
            return notice;
        }

        [WebMethod(EnableSession = true)]
        public string Feedback()
        {
            try
            {
                if (Session[Sessionparam.WEBUSERCOMMENT] == null) return "false";
                FeedbackInfo info = (FeedbackInfo)Session[Sessionparam.WEBUSERCOMMENT];

                string content = this.Gettemplate("MailToServer.Feedback");
                if (content == null) return "false";

                string subject = info.Name;
                content = content.Replace("$VAR_TIMEUPDATE$", info.eTimeupdate);
                content = content.Replace("$VAR_SENDERNAME$", info.Sender_Name);
                content = content.Replace("$VAR_SENDERADDRESS$", info.Sender_Address);
                content = content.Replace("$VAR_SENDERPHONE$", info.Sender_Phone);
                content = content.Replace("$VAR_SENDEREMAIL$", info.Sender_Email);
                content = content.Replace("$VAR_NAME$", info.Name);
                content = content.Replace("$VAR_DESCRIPTION$", info.Description);
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                this.SendMailToServer(info.Sender_Email, subject, content);

                Session.Remove(Sessionparam.WEBUSERCOMMENT);
                return "true";
            }
            catch
            {
                return "false";
            }
        }

        [WebMethod(EnableSession = true)]
        public string Comment()
        {
            try
            {
                if (Session[Sessionparam.WEBUSERCOMMENT] == null) return "false";
                CommentInfo info = (CommentInfo)Session[Sessionparam.WEBUSERCOMMENT];

                string content = this.Gettemplate("MailToServer.Comment");
                if (content == null) return "false";

                string subject = info.Name;
                content = content.Replace("$VAR_TIMEUPDATE$", info.eTimeupdate);
                content = content.Replace("$VAR_SENDERNAME$", info.Sender_Name);
                content = content.Replace("$VAR_SENDERADDRESS$", info.Sender_Address);
                content = content.Replace("$VAR_SENDERPHONE$", info.Sender_Phone);
                content = content.Replace("$VAR_SENDEREMAIL$", info.Sender_Email);
                content = content.Replace("$VAR_NAME$", info.Name);
                content = content.Replace("$VAR_DESCRIPTION$", info.Description);
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                this.SendMailToServer(info.Sender_Email, subject, content);

                Session.Remove(Sessionparam.WEBUSERCOMMENT);
                return "true";
            }
            catch
            {
                return "false";
            }
        }
        
        [WebMethod(EnableSession = true)]
        public string Getpassword()
        {
            try
            {
                if (Session[Sessionparam.WEBUSERGETPASSWORD] == null) return "false";
                MemberInfo member = (MemberInfo)Session[Sessionparam.WEBUSERGETPASSWORD];

                string content = this.Gettemplate("MailToUser.Verifychangepassword");
                if (content == null) return "false";

                string subject = "Yêu cầu mật khẩu tại " + CConstants.WEBSITE;
                content = content.Replace("$VAR_NAME$", member.Fullname);
                content = content.Replace("$VAR_USERNAME$", CFunctions.MBEncrypt(member.Username));
                content = content.Replace("$VAR_TEMPORARYCODE$", CFunctions.MBEncrypt(member.Temporarycode));
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                this.SendMailToUser(member.Email, subject, content);
                
                Session.Remove(Sessionparam.WEBUSERGETPASSWORD);
                return "true";
            }
            catch
            {
                return "false";
            }
        }

        [WebMethod(EnableSession = true)]
        public string Resetpassword()
        {
            try
            {
                if (Session[Sessionparam.WEBUSERGETPASSWORD] == null) return "false";
                MemberInfo member = (MemberInfo)Session[Sessionparam.WEBUSERGETPASSWORD];

                string content = this.Gettemplate("MailToUser.Resetpassword");
                if (content == null) return "false";

                string subject = "Yêu cầu mật khẩu tại " + CConstants.WEBSITE;
                content = content.Replace("$VAR_NAME$", member.Fullname);
                content = content.Replace("$VAR_USERNAME$", member.Username);
                content = content.Replace("$VAR_PASSWORD$", member.Password);
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                this.SendMailToUser(member.Email, subject, content);

                Session.Remove(Sessionparam.WEBUSERGETPASSWORD);
                return "true";
            }
            catch
            {
                return "false";
            }
        }

        [WebMethod(EnableSession = true)]
        public string Register()
        {
            try
            {
                if (Session[Sessionparam.WEBUSERREGISTER] == null) return "false";
                MemberInfo member = (MemberInfo)Session[Sessionparam.WEBUSERREGISTER];

                this.Register_MailToUser(member);
                this.Register_MailToServer(member);
                Session.Remove(Sessionparam.WEBUSERREGISTER);
                return "true";
            }
            catch
            {
                return "false";
            }
        }
        [WebMethod]
        private bool Register_MailToServer(MemberInfo member)
        {
            try
            {
                string content = this.Gettemplate("MailToServer.Register");
                if (content == null) return false;

                string subject = "New member " + CConstants.WEBSITE;
                content = content.Replace("$VAR_NAME$", member.Fullname);
                content = content.Replace("$VAR_USERNAME$", member.Username);
                content = content.Replace("$VAR_PASSWORD$", member.Password);
                content = content.Replace("$VAR_EMAIL$", member.Email);
                content = content.Replace("$VAR_PHONE$", member.iProfile.Phone);
                content = content.Replace("$VAR_YAHOO$", member.iProfile.Yahootext);
                content = content.Replace("$VAR_SKYPE$", member.iProfile.Skypetext);
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                this.SendMailToServer(member.Email, subject, content);
                return true;
            }
            catch
            {
                return false;
            }
        }
        [WebMethod]
        private bool Register_MailToUser(MemberInfo member)
        {
            try
            {
                string content = this.Gettemplate("MailToUser.Register");
                if (content == null) return false;

                string subject = "Chào mừng đến với " + CConstants.WEBSITE;
                content = content.Replace("$VAR_NAME$", member.Fullname);
                content = content.Replace("$VAR_USERNAME$", member.Username);
                content = content.Replace("$VAR_PASSWORD$", member.Password);
                content = content.Replace("$VAR_TEMPORARYCODE$", member.Temporarycode);
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                this.SendMailToUser(member.Email, subject, content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod(EnableSession = true)]
        public string Registerchanged()
        {
            try
            {
                if (Session[Sessionparam.WEBUSERREGISTER] == null) return "false";
                MemberInfo member = (MemberInfo)Session[Sessionparam.WEBUSERREGISTER];

                if (!CFunctions.IsNullOrEmpty(member.Email))
                    this.Registerchanged_MailToUser(member);
                this.Registerchanged_MailToServer(member);
                Session.Remove(Sessionparam.WEBUSERREGISTER);
                return "true";
            }
            catch
            {
                return "false";
            }
        }
        [WebMethod]
        private bool Registerchanged_MailToServer(MemberInfo member)
        {
            try
            {
                string content = this.Gettemplate("MailToServer.Registerchanged");
                if (content == null) return false;

                string subject = member.Filepreview;
                content = content.Replace("$VAR_NAME$", member.Fullname);
                content = content.Replace("$VAR_USERNAME$", member.Username);
                content = content.Replace("$VAR_PASSWORD$", CFunctions.MBDecrypt(member.Password));
                content = content.Replace("$VAR_PIN$", CFunctions.MBDecrypt(member.PIN));
                content = content.Replace("$VAR_EMAIL$", member.Email);
                content = content.Replace("$VAR_ADDRESS$", member.iProfile.Address);
                content = content.Replace("$VAR_PHONE$", member.iProfile.Phone);
                content = content.Replace("$VAR_YAHOO$", member.iProfile.Yahootext);
                content = content.Replace("$VAR_SKYPE$", member.iProfile.Skypetext);
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                this.SendMailToServer(member.Email, subject, content);
                return true;
            }
            catch
            {
                return false;
            }
        }
        [WebMethod]
        private bool Registerchanged_MailToUser(MemberInfo member)
        {
            try
            {
                string content = this.Gettemplate("MailToUser.Registerchanged");
                if (content == null) return false;

                string subject = "Thành viên thay đổi thông tin " + member.Username;
                content = content.Replace("$VAR_NAME$", member.Fullname);
                content = content.Replace("$VAR_USERNAME$", member.Username);
                content = content.Replace("$VAR_PASSWORD$", CFunctions.MBDecrypt(member.Password));
                content = content.Replace("$VAR_PIN$", CFunctions.MBDecrypt(member.PIN));
                content = content.Replace("$VAR_EMAIL$", member.Email);
                content = content.Replace("$VAR_ADDRESS$", member.iProfile.Address);
                content = content.Replace("$VAR_PHONE$", member.iProfile.Phone);
                content = content.Replace("$VAR_YAHOO$", member.iProfile.Yahootext);
                content = content.Replace("$VAR_SKYPE$", member.iProfile.Skypetext);
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                content = content.Replace("$VAR_TEMPORARYCODE$", member.Temporarycode);
                this.SendMailToUser(member.Email, subject, content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod(EnableSession = true)]
        public string Passwordchanged()
        {
            try
            {
                if (Session[Sessionparam.WEBUSERREGISTER] == null) return "false";
                MemberInfo member = (MemberInfo)Session[Sessionparam.WEBUSERREGISTER];

                if (!CFunctions.IsNullOrEmpty(member.Email))
                    this.Passwordchanged_MailToUser(member);
                Session.Remove(Sessionparam.WEBUSERREGISTER);
                return "true";
            }
            catch
            {
                return "false";
            }
        }
        [WebMethod]
        private bool Passwordchanged_MailToUser(MemberInfo member)
        {
            try
            {
                string content = this.Gettemplate("MailToUser.Passwordchanged");
                if (content == null) return false;

                string subject = "VN-Ibet888: thay đổi mật khẩu " + member.Username;
                content = content.Replace("$VAR_NAME$", member.Fullname);
                content = content.Replace("$VAR_USERNAME$", member.Username);
                content = content.Replace("$VAR_PASSWORD$", CFunctions.MBDecrypt(member.Password));
                content = content.Replace("$VAR_PIN$", CFunctions.MBDecrypt(member.PIN));
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                content = content.Replace("$VAR_TEMPORARYCODE$", member.Temporarycode);
                this.SendMailToUser(member.Email, subject, content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod(EnableSession = true)]
        public string Createuser()
        {
            try
            {
                if (Session[Sessionparam.USERREGISTER] == null) return "false";
                UserInfo user = (UserInfo)Session[Sessionparam.USERREGISTER];

                this.Createuser_MailToUser(user);
                Session.Remove(Sessionparam.USERREGISTER);
                return "true";
            }
            catch
            {
                return "false";
            }
        }
        [WebMethod]
        private bool Createuser_MailToUser(UserInfo user)
        {
            try
            {
                string content = this.Gettemplate("MailToUser.Createuser");
                if (content == null) return false;

                string subject = "Tài khoản quản trị web " + CConstants.WEBSITE;
                content = content.Replace("$VAR_NAME$", user.Name);
                content = content.Replace("$VAR_USERNAME$", user.Username);
                content = content.Replace("$VAR_PASSWORD$", user.Password);
                content = content.Replace("$VAR_EMAIL$", user.Email);
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                this.SendMailToUser(user.Email, subject, content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod(EnableSession = true)]
        public string Orderconfirm()
        {
            try
            {
                if (Session[Sessionparam.CART] == null) return "false";
                CartInfo CARTINFO = (CartInfo)Session[Sessionparam.CART];

                this.Order_MailToServer(CARTINFO);
                this.Order_MailToUser(CARTINFO);
                Session.Remove(Sessionparam.CART);
                return "true";
            }
            catch
            {
                return "false";
            }
        }
        [WebMethod]
        private bool Order_MailToServer(CartInfo CARTINFO)
        {
            try
            {
                string content = this.Gettemplate("MailToServer.Order");
                if (content == null) return false;

                string subject = "Order " + CARTINFO.Name;
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                content = content.Replace("$VAR_TIMEUPDATE$", CARTINFO.eTimeupdate);
                string cartlist = "";
                string cartlisttemplate = CCommon.Get_Definephrase(Definephrase.Cart_listtemplate);
                foreach (CartitemInfo cartitem in CARTINFO.lCartitem)
                {
                    string item = cartlisttemplate.Replace("$VAR_PRODUCTNAME$", cartitem.Productname);
                    item = item.Replace("$VAR_QUANTITY$", cartitem.Quantity.ToString());
                    item = item.Replace("$VAR_AMOUNT$", cartitem.eAmount);
                    cartlist += item;
                }

                content = content.Replace("$VAR_CARTLIST$", cartlist);
                content = content.Replace("$VAR_AMOUNTTOTAL$", CARTINFO.eAmount);
                content = content.Replace("$VAR_NAME$", CARTINFO.iMember.Fullname);
                content = content.Replace("$VAR_ADDRESS$", CARTINFO.iMember.iProfile.Address + " - " + CARTINFO.iMember.iProfile.Cityname + " - " + CARTINFO.iMember.iProfile.Nationalname);
                content = content.Replace("$VAR_PHONE$", CARTINFO.iMember.iProfile.Phone);
                content = content.Replace("$VAR_USERNAME$", CARTINFO.iMember.Username);
                content = content.Replace("$VAR_SHIPPING_ADDRESS$", CARTINFO.Shipping_Address + " - " + CARTINFO.Shipping_City + " - " + CARTINFO.Shipping_Nationalname);
                content = content.Replace("$VAR_SHIPPING_NAME$", CARTINFO.Shipping_Name);
                content = content.Replace("$VAR_SHIPPING_PHONE$", CARTINFO.Shipping_Phone);
                content = content.Replace("$VAR_PAYMENTNAME$", CARTINFO.iPayment.Name);
                content = content.Replace("$VAR_PAYMENTNOTE$", CARTINFO.iPayment.Note);
                content = content.Replace("$VAR_NOTE$", CARTINFO.Note);
                content = content.Replace("$VAR_LANG$", LANG);
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                this.SendMailToServer(CARTINFO.iMember.Username, subject, content);
                return true;
            }
            catch
            {
                return false;
            }
        }
        [WebMethod]
        private bool Order_MailToUser(CartInfo CARTINFO)
        {
            try
            {
                string content = this.Gettemplate("MailToUser.Order");
                if (content == null) return false;

                string subject = "Order " + CARTINFO.Name;
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                content = content.Replace("$VAR_TIMEUPDATE$", CARTINFO.eTimeupdate);
                string cartlist = "";
                string cartlisttemplate = CCommon.Get_Definephrase(Definephrase.Cart_listtemplate);
                foreach (CartitemInfo cartitem in CARTINFO.lCartitem)
                {
                    string item = cartlisttemplate.Replace("$VAR_PRODUCTNAME$", cartitem.Productname);
                    item = item.Replace("$VAR_QUANTITY$", cartitem.Quantity.ToString());
                    item = item.Replace("$VAR_AMOUNT$", cartitem.eAmount);
                    cartlist += item;
                }

                content = content.Replace("$VAR_CARTLIST$", cartlist);
                content = content.Replace("$VAR_AMOUNTTOTAL$", CARTINFO.eAmount);
                content = content.Replace("$VAR_NAME$", CARTINFO.iMember.Fullname);
                content = content.Replace("$VAR_ADDRESS$", CARTINFO.iMember.iProfile.Address + " - " + CARTINFO.iMember.iProfile.Cityname + " - " + CARTINFO.iMember.iProfile.Nationalname);
                content = content.Replace("$VAR_PHONE$", CARTINFO.iMember.iProfile.Phone);
                content = content.Replace("$VAR_USERNAME$", CARTINFO.iMember.Username);
                content = content.Replace("$VAR_SHIPPING_ADDRESS$", CARTINFO.Shipping_Address + " - " + CARTINFO.Shipping_City + " - " + CARTINFO.Shipping_Nationalname);
                content = content.Replace("$VAR_SHIPPING_NAME$", CARTINFO.Shipping_Name);
                content = content.Replace("$VAR_SHIPPING_PHONE$", CARTINFO.Shipping_Phone);
                content = content.Replace("$VAR_PAYMENTNAME$", CARTINFO.iPayment.Name);
                content = content.Replace("$VAR_PAYMENTNOTE$", CARTINFO.iPayment.Note);
                content = content.Replace("$VAR_NOTE$", CARTINFO.Note);
                content = content.Replace("$VAR_LANG$", LANG);
                content = content.Replace("$VAR_WEBSITE$", CConstants.WEBSITE);
                this.SendMailToUser(CARTINFO.iMember.Username, subject, content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod(EnableSession = true)]
        public string Cart_Remove(int index)
        {
            try
            {
                (new CartHandler(CCommon.LANG)).RemovefromCart(index);
                return "true";
            }
            catch
            {
                return "false";
            }
        }

        [WebMethod(EnableSession = true)]
        public string SwitchLang(string lang)
        {
            CCommon.LANG = lang;
            return lang;
        }

    }
}
