using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm
{
    public partial class uclogin : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Auto_Login();
                //LoadCaptcha(imgCaptcha);
            }
        }

        #region private methods

        private void Write_Cookies(MemberInfo logger)
        {
            if (chkRememberlogin.Checked)
            {
                Response.Cookies[CConstants.WEBSITE][":web:username"] = Server.HtmlDecode(logger.Username);
                Response.Cookies[CConstants.WEBSITE][":web:password"] = Server.HtmlDecode(logger.Password);
                Response.Cookies[CConstants.WEBSITE][":web:remember"] = "1";
                Response.Cookies[CConstants.WEBSITE].Expires.AddDays(7);
            }
            else
            {
                Response.Cookies.Remove(CConstants.WEBSITE);
            }
        }
        private MemberInfo Read_Cookies()
        {
            if (Request.Cookies[CConstants.WEBSITE] == null) return null;

            MemberInfo logger = new MemberInfo();
            logger.Username = Server.HtmlEncode(Request.Cookies[CConstants.WEBSITE][":web:username"]);
            logger.Password = Server.HtmlEncode(Request.Cookies[CConstants.WEBSITE][":web:password"]);
            logger.Logincache = Request.Cookies[CConstants.WEBSITE][":web:remember"] == null ? 0 : int.Parse(Server.HtmlEncode(Request.Cookies[CConstants.WEBSITE][":web:remember"]));
            return logger;
        }
        private void Auto_Login()
        {
            MemberInfo current = CCommon.CurrentMember;
            this.Show_Login(current);
            if (current != null) return;

            string problem = Request.QueryString[Queryparam.Problem] == null ? "" : Request.QueryString[Queryparam.Problem].ToString();
            if (problem == null || problem == "" || problem == "nothing")
            {
                MemberInfo info = this.Read_Cookies();
                if (info == null || info.Logincache != 1) return;

                txtUsername.Text = info.Username;
            }
            else
            {
                switch (problem)
                {
                    case "restricted":
                        lblError.Text = CCommon.Get_Definephrase(Definephrase.Login_restricted);
                        break;
                    case "expired":
                        lblError.Text = CCommon.Get_Definephrase(Definephrase.Login_expired);
                        break;
                }
            }
        }
        private void Show_Login(MemberInfo info)
        {
            this.Visible = info == null;


        }
        private bool Validata()
        {
            try
            {
                //lstError = new List<Errorobject>();
                //if (!imgCaptcha.ImageUrl.Contains("/" + CFunctions.MBEncrypt(txtCaptcha.Text) + ".png"))
                //{
                //    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_captcha, "", txtCaptcha);
                //    txtCaptcha.Text = "";
                //}
                return CCommon.Form_ShowError(lstError, lblError);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region events
        protected void cmdLogin_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!this.Validata())
                //{
                //    LoadCaptcha(imgCaptcha);
                //    return;
                //}

                MemberInfo info = new MemberInfo();
                info.Username = txtUsername.Text.Trim();
                info.Password = CFunctions.MBEncrypt(txtPassword.Text);

                MemberInfo logger = (new CMember(CCommon.LANG)).Login(info);
                if (logger == null)
                {
                    lblError.Text = CCommon.Get_Definephrase(Definephrase.Login_invalid);
                }
                else
                {
                    CCommon.Session_Set(Sessionparam.WEBUSERLOGIN, logger);
                    this.Write_Cookies(logger);

                    if (CFunctions.IsNullOrEmpty(CCommon.PreviousUrl) && CFunctions.IsNullOrEmpty(CConstants.PAGE_WELCOMEDEF))
                    {
                        this.Show_Login(logger);
                        Response.Redirect(Request.Url.AbsolutePath + Request.Url.Query, false);
                    }
                    else
                    {
                        Response.Redirect((!CFunctions.IsNullOrEmpty(CCommon.PreviousUrl) ? CCommon.PreviousUrl : CConstants.PAGE_WELCOMEDEF), false);
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdGetpassword_Click(object sender, EventArgs e)
        {
            try
            {
                CMember DAL = new CMember(CCommon.LANG);
                MemberInfo member = DAL.Getinfo(gp_txtUsername.Text.Trim());
                if (member == null)
                {
                    gp_lblError.Text = CCommon.Get_Definephrase(Definephrase.Invalid_username_notexist);
                    pnlGetlogin.Attributes.Add("style", "display:none");
                    pnlGetpassword.Attributes.Add("style", "display:block");
                }
                else
                {
                    if (member.Email != gp_txtEmail.Text.Trim())
                    {
                        gp_lblError.Text = CCommon.Get_Definephrase(Definephrase.Invalid_username_email);
                        pnlGetlogin.Attributes.Add("style", "display:none");
                        pnlGetpassword.Attributes.Add("style", "display:block");
                    }
                    else
                    {
                        string temporarycode = DateTime.Now.Ticks.ToString();
                        if (DAL.Updatestr(member.Id.ToString(), "temporarycode", temporarycode))
                        {
                            member.Temporarycode = temporarycode;
                            CCommon.Session_Set(Sessionparam.WEBUSERGETPASSWORD, member);
                            gp_lblError.Text = CCommon.Get_Definephrase(Definephrase.Getpassword_verify);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "getpassword", "Getpassword();", true);
                        }
                        else
                        {
                            gp_lblError.Text = CCommon.Get_Definephrase(Definephrase.Getpassword_error);
                        }
                        pnlGetlogin.Attributes.Add("style", "display:none");
                        pnlGetpassword.Attributes.Add("style", "display:block");
                        formGetpassword.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        //protected void cmdCaptcha_Click(object sender, ImageClickEventArgs e)
        //{
        //    LoadCaptcha(imgCaptcha);
        //}
        #endregion
    }
}