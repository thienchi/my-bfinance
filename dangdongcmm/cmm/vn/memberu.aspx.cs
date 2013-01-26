using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm.cmm
{
    public partial class memberu : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Init_State();
                this.Load_Info(CCommon.Get_QueryNumber(Queryparam.Iid));
            }
        }
        
        public bool haveApplication = false;
        public string oldPassword;
        public int oldId;

        #region private methods
        private void Init_State()
        {
            //this.BindDDL_National();
            this.Bind_ddlDistrict();
            this.Bind_ddlYearLine(ddlYear);
            this.Bind_ddlDayLine(ddlDay);
        }
        private void Bind_ddlYearLine(DropDownList ddl)
        {
            ddl.Items.Clear();
            for (int i = 0; i <= 100; i++)
            {
                int year = 1911 + i;
                ListItem item = new ListItem(year.ToString());
                ddl.Items.Add(item);
            }
            ddl.SelectedIndex = 0;
        }
        private void Bind_ddlDayLine(DropDownList ddl)
        {
            ddl.Items.Clear();
            for (int i = 1; i <= 31; i++)
            {
                int day = i;
                ListItem item = new ListItem(day.ToString());
                ddl.Items.Add(item);
            }
            ddl.SelectedIndex = 0;
        }
        private void Bind_ddlDistrict()
        {
            ddlDistrict.Items.Clear();
            BindDDL_Cid(ddlDistrict, Webcmm.Id.District, 0, "");
            CCommon.Insert_FirstitemDDL(ddlDistrict);
        }
        private void BindDDL_National()
        {
            BindDDL_Cid(ddlNational, Webcmm.Id.National, 0, "");
            CCommon.Insert_FirstitemDDL(ddlNational);
        }
        private void BindDDL_City(int countryid)
        {
            ddlCity.Items.Clear();
            BindDDL_Cid(ddlCity, Webcmm.Id.National, countryid, "");
            CCommon.Insert_FirstitemDDL(ddlCity);
        }
        
        private bool Validata()
        {
            try
            {
                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(txtUsername.Text) || !CFunctions.IsAlpha(txtUsername.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_username, "", txtUsername);
                if (CFunctions.IsNullOrEmpty(txtPassword.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_password, "", txtPassword);
                if (CFunctions.IsNullOrEmpty(txtEmail.Text) || !CFunctions.IsEmail(txtEmail.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_email, "", txtEmail);
                return Master.Form_ShowError(lstError);
            }
            catch
            {
                return false;
            }
        }
        private MemberInfo Take()
        {
            try
            {
                int iid = 0;
                int.TryParse(txtId.Value, out iid);
                MemberInfo info = (new CMember(CCommon.LANG)).Wcmm_Getinfo(iid);
                if (info == null)
                    info = new MemberInfo();
                oldId = info.Id;
                oldPassword = info.Password;
                info.Id = iid;
                info.Username = txtUsername.Text.Trim();
                info.Password = CFunctions.IsNullOrEmpty(txtPassword.Text) ? info.Password : CFunctions.MBEncrypt(txtPassword.Text);
                info.Firstname = txtFirstname.Text.Trim();
                info.Lastname = txtLastname.Text.Trim();
                info.Email = txtEmail.Text.Trim();
                info.Grouptype = ddlGrouptype.SelectedIndex;
                //info.Filepreview = Filepreview.Get();
                info.Status = Displaysetting.Get_Status();
                if (info.Id == 0)
                    info.Timecreate = DateTime.Now;
                info.Timeupdate = DateTime.Now;

                if (info.iProfile == null)
                    info.iProfile = new MeProfileInfo();
                info.iProfile.Phone = txtPhone.Text.Trim();
                info.iProfile.Address = txtAddress.Text.Trim();
                info.iProfile.Districtname = txtDistrict.Text.Trim();
                info.iProfile.Cityname = txtCity.Text.Trim();
                info.iProfile.Zipcode = txtZipcode.Text.Trim();
                //info.iProfile.Nationalid = int.Parse(ddlNational.SelectedValue);
                //info.iProfile.Cityid = int.Parse(ddlCity.SelectedValue);
                info.iProfile.Districtid = int.Parse(ddlDistrict.SelectedValue);
                info.iProfile.Districtname = ddlDistrict.SelectedIndex == 0 ? "" : ddlDistrict.SelectedItem.Text;
                info.iProfile.Birthday = new DateTime(int.Parse(ddlYear.SelectedValue), int.Parse(ddlMonth.SelectedValue), int.Parse(ddlDay.SelectedValue));
                info.iProfile.About = txtAbout.Text.Trim();
                info.iProfile.Blog = radBlogsh.SelectedValue + (txtBlog.Text.Trim() == "http://" ? "" : txtBlog.Text.Trim());
                info.iProfile.Homepage = radHomepagesh.SelectedValue + (txtHomepage.Text.Trim() == "http://" ? "" : txtHomepage.Text.Trim());
                info.iProfile.Facebook = radFacebooksh.SelectedValue + (txtFacebook.Text.Trim() == "http://www.facebook.com/" ? "" : txtFacebook.Text.Trim());
                info.iProfile.Twitter = radTwittersh.SelectedValue + (txtTwitter.Text.Trim() == "http://www.twitter.com/" ? "" : txtTwitter.Text.Trim());
                info.iProfile.Youtube = radYoutubesh.SelectedValue + (txtYoutube.Text.Trim() == "http://www.youtube.com/" ? "" : txtYoutube.Text.Trim());
                info.iProfile.Flickr = radFlickrsh.SelectedValue + (txtFlickr.Text.Trim() == "http://www.flickr.com/" ? "" : txtFlickr.Text.Trim());
                info.iProfile.Skype = radSkypesh.SelectedValue + txtSkype.Text.Trim();
                info.iProfile.Yahoo = radYahoosh.SelectedValue + txtYahoo.Text.Trim();

                return info;
            }
            catch
            {
                return null;
            }
        }
        private bool Load_Info(int iid)
        {
            try
            {
                MemberInfo info = null;
                if (iid != 0)
                {
                    info = (new CMember(CCommon.LANG)).Wcmm_Getinfo(iid);
                    if (info != null)
                    {
                        lstError = new List<Errorobject>();
                        lstError = Form_GetError(lstError, Errortype.Notice, Definephrase.Save_notice, "[" + info.Id + "] " + info.Fullname, null);
                        Master.Form_ShowError(lstError);
                    }
                }
                if (info == null)
                    info = new MemberInfo();
                chkSaveoption_golist.Checked = info.Id != 0;
                
                txtId.Value = info.Id.ToString();
                txtUsername.Text = info.Username;
                txtPassword.Text = CFunctions.MBDecrypt(info.Password);
                txtFirstname.Text = info.Firstname;
                txtLastname.Text = info.Lastname;
                txtEmail.Text = info.Email;
                ddlGrouptype.SelectedIndex = info.Grouptype;
                //Filepreview.Set(info.Filepreview);
                Displaysetting.Set("", info.Status, 0);
                
                if (info.iProfile == null)
                    info.iProfile = new MeProfileInfo();
                txtPhone.Text = info.iProfile.Phone;
                txtAddress.Text = info.iProfile.Address;
                txtDistrict.Text = info.iProfile.Districtname;
                txtCity.Text = info.iProfile.Cityname;
                txtZipcode.Text = info.iProfile.Zipcode;
                //ddlNational.SelectedValue = info.iProfile.Nationalid.ToString();
                //ddlCity.SelectedValue = info.iProfile.Cityid.ToString();
                ddlDistrict.SelectedValue = info.iProfile.Districtid.ToString();
                ddlYear.SelectedValue = info.iProfile.Birthday.Year.ToString();
                ddlMonth.SelectedValue = info.iProfile.Birthday.Month.ToString();
                ddlDay.SelectedValue = info.iProfile.Birthday.Day.ToString();
                txtAbout.Text = info.iProfile.About;
                txtBlog.Text = CFunctions.IsNullOrEmpty(info.iProfile.Blog) ? "http://" : info.iProfile.Blogtext;
                txtHomepage.Text = CFunctions.IsNullOrEmpty(info.iProfile.Homepage) ? "http://" : info.iProfile.Homepagetext;
                txtFacebook.Text = CFunctions.IsNullOrEmpty(info.iProfile.Facebook) ? "http://www.facebook.com/" : info.iProfile.Facebooktext;
                txtTwitter.Text = CFunctions.IsNullOrEmpty(info.iProfile.Twitter) ? "http://www.twitter.com/" : info.iProfile.Twittertext;
                txtYoutube.Text = CFunctions.IsNullOrEmpty(info.iProfile.Youtube) ? "http://www.youtube.com/" : info.iProfile.Youtubetext;
                txtFlickr.Text = CFunctions.IsNullOrEmpty(info.iProfile.Flickr) ? "http://www.flickr.com/" : info.iProfile.Flickrtext;
                txtSkype.Text = CFunctions.IsNullOrEmpty(info.iProfile.Skype) ? "" : info.iProfile.Skypetext;
                txtYahoo.Text = CFunctions.IsNullOrEmpty(info.iProfile.Yahoo) ? "" : info.iProfile.Yahootext;
                radBlogsh.SelectedValue = info.iProfile.Blogsh;
                radHomepagesh.SelectedValue = info.iProfile.Homepagesh;
                radFacebooksh.SelectedValue = info.iProfile.Facebooksh;
                radTwittersh.SelectedValue = info.iProfile.Twittersh;
                radYoutubesh.SelectedValue = info.iProfile.Youtubesh;
                radFlickrsh.SelectedValue = info.iProfile.Flickrsh;
                radSkypesh.SelectedValue = info.iProfile.Skypesh;
                radYahoosh.SelectedValue = info.iProfile.Yahoosh;

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region events
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.Validata()) return;

                lstError = new List<Errorobject>();
                MemberInfo info = this.Take();
                if (info == null)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Takeinfo_error, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                CConstants.State.Existed vlExist = (new CMember(CCommon.LANG)).Exist(info);
                if (vlExist != CConstants.State.Existed.None)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, vlExist == CConstants.State.Existed.Name ? Definephrase.Exist_username : Definephrase.Exist_email, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                if (new CMember(CCommon.LANG).Save(info))
                {
                    info.iProfile.Id = info.Id;
                    new CMeProfile(CCommon.LANG).Save(info.iProfile);
                    
                    lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Save_completed, "", null);
                    Master.Form_ShowError(lstError);
                    if (chkSaveoption_golist.Checked)
                        this.Load_Info(info.Id);
                    Form_SaveOption(!chkSaveoption_golist.Checked);

                    if (oldId == 0 || oldPassword != CFunctions.MBEncrypt(txtPassword.Text))
                    {
                        info.Password = txtPassword.Text;
                        CCommon.Session_Set(Sessionparam.WEBUSERREGISTER, info);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Register", "Register();", true);
                    }
                }
                else
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Save_error, "", null);
                    Master.Form_ShowError(lstError);
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdClear_Click(object sender, EventArgs e)
        {
            try
            {
                this.Load_Info(0);
                Master.Form_ClearError();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }

        protected void ddlNational_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int countryid = int.Parse(ddlNational.SelectedValue);
                this.BindDDL_City(countryid);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }

        protected void cmdCreatepassword_Click(object sender, EventArgs e)
        {
            try
            {
                txtPassword.Text = CFunctions.Randomstr(6);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "gottabs", "gottabs(1);", true);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        #endregion
    }
}
