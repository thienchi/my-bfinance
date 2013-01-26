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

using dangdongcmm.dal;
using dangdongcmm.model;
using dangdongcmm.utilities;

namespace dangdongcmm.cmm
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Auto_Login();

                this.BindDDL_Lang(ddlLang);
                this.Build_Interface(CCommon.LANG);
                txtUsername.Focus();
            }
            CCommon.LANG = ddlLang.SelectedValue;
        }
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "processing", "close_processing();", true);
            }
        }

        #region private methods
        private void BindDDL_Lang(DropDownList cbo)
        {
            try
            {
                int lang_num = Convert.ToInt32(ConfigurationSettings.AppSettings["LANG_NUM"]);
                string lang_str = ConfigurationSettings.AppSettings["LANG_TXT"];
                string[] lang_arr = lang_str.Split(Convert.ToChar("$"));
                if (lang_arr == null || lang_arr.Length == 0 || lang_arr.Length != lang_num) return;

                for (int i = 0; i < lang_num; i++)
                {
                    string lang_val = ConfigurationSettings.AppSettings["LANG_" + i];
                    string lang_txt = lang_arr[i];
                    ListItem item = new ListItem("-- " + lang_txt, lang_val);
                    cbo.Items.Add(item);
                }
                ListItem itemc = cbo.Items.FindByValue(CCommon.LANG);
                if (itemc != null)
                    cbo.SelectedValue = itemc.Value;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Build_Interface(string lang)
        {
            try
            {
                lblTitle.Text = lnkLogin.Text = CCommon.Get_Definephrase(Definephrase.Interface_login_title);
                lblUsername.Text = gp_lblUsername.Text = CCommon.Get_Definephrase(Definephrase.Interface_login_username);
                lblPassword.Text = CCommon.Get_Definephrase(Definephrase.Interface_login_password);
                cmdLogin.Text = CCommon.Get_Definephrase(Definephrase.Interface_login_submit);
                chkRememberlogin.Text = CCommon.Get_Definephrase(Definephrase.Interface_login_remember);
                lnkGetpassword.Text = cmdGetpassword.Text = CCommon.Get_Definephrase(Definephrase.Interface_login_getpassword);
                lnkWebsite.Text = lnkWebsite.NavigateUrl = CConstants.WEBSITE;
                lblCopyright.Text = CCommon.Get_Definephrase(Definephrase.Interface_login_copyright);

                string problem = Request.QueryString[Queryparam.Problem] == null ? "" : Request.QueryString[Queryparam.Problem].ToString();
                switch (problem)
                {
                    case "restricted":
                        lblError.Text = CCommon.Get_Definephrase(Definephrase.Login_restricted);
                        break;
                    case "expired":
                        lblError.Text = CCommon.Get_Definephrase(Definephrase.Login_expired);
                        break;
                }

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Write_Cookies(UserInfo logger)
        {
            if (chkRememberlogin.Checked)
            {
                Response.Cookies[CConstants.WEBSITE][":cmm:username"] = Server.HtmlDecode(logger.Username);
                Response.Cookies[CConstants.WEBSITE][":cmm:password"] = Server.HtmlDecode(logger.Password);
                Response.Cookies[CConstants.WEBSITE][":cmm:remember"] = "1";
                Response.Cookies[CConstants.WEBSITE][":cmm:lang"] = Server.HtmlDecode(ddlLang.SelectedValue);
                Response.Cookies[CConstants.WEBSITE].Expires.AddDays(7);
            }
            else
            {
                Response.Cookies.Remove(CConstants.WEBSITE);
            }
        }
        private UserInfo Read_Cookies()
        {
            if (Request.Cookies[CConstants.WEBSITE] == null) return null;

            UserInfo logger = new UserInfo();
            logger.Username = Server.HtmlEncode(Request.Cookies[CConstants.WEBSITE][":cmm:username"]);
            logger.Password = Server.HtmlEncode(Request.Cookies[CConstants.WEBSITE][":cmm:password"]);
            if (Request.Cookies[CConstants.WEBSITE][":cmm:remember"] != null)
                logger.Logincache = int.Parse(Server.HtmlEncode(Request.Cookies[CConstants.WEBSITE][":cmm:remember"]));
            CCommon.LANG = Server.HtmlEncode(Request.Cookies[CConstants.WEBSITE][":cmm:lang"]);
            return logger;
        }
        private void Auto_Login()
        {
            string problem = Request.QueryString[Queryparam.Problem] == null ? "" : Request.QueryString[Queryparam.Problem].ToString();
            if (problem == null || problem == "" || problem == "nothing")
            {
                UserInfo info = this.Read_Cookies();
                if (info == null || info.Logincache != 1) return;

                UserInfo logger = (new CUser()).Login(info);
                if (logger == null)
                {
                    lblError.Text = CCommon.Get_Definephrase(Definephrase.Interface_login_invalid);
                }
                else
                {
                    if (logger.iRight == null)
                        logger.iRight = (new CUserright()).Getinfo(logger.Id);
                    CCommon.Session_Set(Sessionparam.USERLOGIN, logger);
                    Response.Redirect(CCommon.LANG + "/" + "dashboard.aspx");
                    return;
                }
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

        #endregion

        #region events
        protected void ddlLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CCommon.LANG = ddlLang.SelectedValue;
                this.Build_Interface(CCommon.LANG);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected void lnkGetpassword_Click(object sender, EventArgs e)
        {
            pnlLogin.Visible = false;
            pnlGetpassword.Visible = !pnlLogin.Visible;
            gp_txtUsername.Focus();
        }
        protected void lnkLogin_Click(object sender, EventArgs e)
        {
            pnlLogin.Visible = true;
            pnlGetpassword.Visible = !pnlLogin.Visible;
        }

        protected void cmdLogin_Click(object sender, EventArgs e)
        {
            try
            {
                UserInfo info = new UserInfo();
                info.Username = txtUsername.Text.Trim();
                info.Password = CFunctions.MBEncrypt(txtPassword.Text);

                UserInfo logger = (new CUser()).Login(info);
                if (logger == null)
                {
                    lblError.Text = CCommon.Get_Definephrase(Definephrase.Interface_login_invalid);
                }
                else
                {
                    if (logger.iRight == null)
                        logger.iRight = (new CUserright()).Getinfo(logger.Id);
                    CCommon.Session_Set(Sessionparam.USERLOGIN, logger);
                    this.Write_Cookies(logger);

                    if (Session[Sessionparam.PREVIOUSURL] != null)
                        Response.Redirect(Session[Sessionparam.PREVIOUSURL].ToString());
                    else
                        Response.Redirect(CCommon.LANG + "/" + "dashboard.aspx");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected void cmdGetpassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(gp_txtUsername.Text) || !CFunctions.IsAlpha(gp_txtUsername.Text))
                {
                    gp_lblError.Text = CCommon.Get_Definephrase(Definephrase.Invalid_username);
                    return;
                }
                if (CFunctions.IsNullOrEmpty(gp_txtEmail.Text) || !CFunctions.IsEmail(gp_txtEmail.Text))
                {
                    gp_lblError.Text = CCommon.Get_Definephrase(Definephrase.Invalid_email);
                    return;
                }

                CUser DALUser = new CUser();
                UserInfo info = DALUser.Getinfo(gp_txtUsername.Text.Trim());
                if (info == null)
                {
                    gp_lblError.Text = CCommon.Get_Definephrase(Definephrase.Invalid_username_notexist);
                    return;
                }
                else
                {
                    if (info.Email != gp_txtEmail.Text.Trim())
                    {
                        gp_lblError.Text = CCommon.Get_Definephrase(Definephrase.Invalid_username_email);
                        return;
                    }

                    string passwordold = info.Password;
                    string passwordnew = CFunctions.Randomstr(6);
                    if (DALUser.Updatestr(info.Id.ToString(), "password", CFunctions.MBEncrypt(passwordnew)))
                    {
                        info.Password = passwordnew;
                        CCommon.Session_Set(Sessionparam.USERREGISTER, info);

                        pnlGetpassword.Controls.Clear();
                        Label lbl = new Label();
                        lbl.Text = CCommon.Get_Definephrase(Definephrase.Getpassword_done);
                        lbl.CssClass = "control";
                        pnlGetpassword.Controls.Add(lbl);

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Createuser", "Createuser();", true);
                    }
                    else
                    {
                        DALUser.Updatestr(info.Id.ToString(), "password", passwordold);
                        gp_lblError.Text = CCommon.Get_Definephrase(Definephrase.Getpassword_error);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
