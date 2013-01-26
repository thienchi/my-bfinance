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

namespace dangdongcmm
{
    public partial class register : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CCommon.LoadCaptcha(imgCaptcha);
                this.BindDDL_Region(ddlNational, 0);
                this.ddlNational_SelectedIndexChanged(null, null);
            }
        }

        #region private methods
        private void BindDDL_Region(DropDownList ddl, int pid)
        {
            if (!ddl.Visible) return;

            ListOptions options = Get_ListOptionsNoPaging();
            options.SortExp = Queryparam.Sqlcolumn.Orderd;
            options.GetAll = true;
            int numResults = 0;
            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Getlist(Webcmm.Id.National, pid, options, out numResults);
            if (list == null) return;

            ddl.DataSource = list;
            ddl.DataValueField = "Id";
            ddl.DataTextField = "Name";
            ddl.DataBind();

            CCommon.Insert_FirstitemDDL(ddl);
            if (ddl.Items.Count > 0) ddl.SelectedIndex = 1;
        }
        
        private bool Validata()
        {
            try
            {
                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(txtUsername.Text) || !CFunctions.IsEmail(txtUsername.Text))
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_username, "", txtUsername);
                if (CFunctions.IsNullOrEmpty(txtPassword.Text))
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_password, "", txtPassword);
                if (CFunctions.IsNullOrEmpty(txtName.Text))
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Require_name, "", txtName);
                if (ddlNational.Visible && ddlNational.SelectedIndex == 0)
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Require_national, "", ddlNational);
                if (ddlCity.Visible && ddlCity.SelectedIndex == 0)
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Require_city, "", ddlCity);

                return CCommon.Form_ShowError(lstError, lblError);
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
                MemberInfo member = new MemberInfo();
                member.Username = txtUsername.Text.Trim().ToLower();
                member.Password = CFunctions.MBEncrypt(txtPassword.Text);
                member.Email = member.Username;
                member.PIN = CFunctions.MBEncrypt(CFunctions.Randomnum(6));
                member.Status = CConstants.REGISTERCONFIRM;
                member.Timeupdate = DateTime.Now;
                string Fullname = txtName.Text.Trim();
                if (!CFunctions.IsNullOrEmpty(Fullname))
                {
                    int index = Fullname.IndexOf(" ");
                    if (index != -1)
                    {
                        member.Firstname = Fullname.Substring(0, index).Trim();
                        member.Lastname = Fullname.Substring(index).Trim();
                    }
                    else
                    {
                        member.Firstname = Fullname;
                    }
                }

                if (member.iProfile == null)
                    member.iProfile = new MeProfileInfo();
                member.iProfile.Phone = txtPhone.Text.Trim();
                member.iProfile.Address = txtAddress.Text.Trim();
                if (ddlNational.Visible && ddlNational.Items.Count > 0 && ddlNational.SelectedIndex != 0)
                {
                    member.iProfile.Nationalid = int.Parse(ddlNational.SelectedValue);
                    member.iProfile.Nationalname = ddlNational.SelectedItem.Text;
                }
                if (ddlCity.Visible && ddlCity.Items.Count > 0 && ddlCity.SelectedIndex != 0)
                {
                    member.iProfile.Cityid = int.Parse(ddlCity.SelectedValue);
                    member.iProfile.Cityname = ddlCity.SelectedItem.Text;
                }

                return member;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region events
        protected void cmdRegister_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!this.Validata()) return;

                lstError = new List<Errorobject>();
                MemberInfo member = this.Take();
                if (member == null)
                {
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Takeinfo_error, "", null);
                    CCommon.Form_ShowError(lstError, lblError);
                    return;
                }

                CMember DAL = new CMember();
                MemberInfo existmember = DAL.Getinfo(txtUsername.Text.Trim());
                if (existmember != null && existmember.Autosave == 0)
                {
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Exist_username, "", txtUsername);
                    CCommon.Form_ShowError(lstError, lblError);
                    goto gotoExit;
                } 
                else if (existmember != null && existmember.Autosave == 1)
                {
                    member.Id = existmember.Id;
                    member.Autosave = 0;
                }

                if (DAL.Save(member))
                {
                    member.iProfile.Id = member.Id;
                    new CMeProfile(CCommon.LANG).Save(member.iProfile);
                    
                    pnlForm.Visible = false;
                    pnlNotice.Visible = true;

                    member.Password = txtPassword.Text;
                    CCommon.Session_Set(Sessionparam.WEBUSERREGISTER, member);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "register", "Register();", true);
                }
                else
                {
                    pnlForm.Visible = false;
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Createaccount_error, "", null);
                    CCommon.Form_ShowError(lstError, lblError);
                }
                return;

            gotoExit:
                {
                    CCommon.LoadCaptcha(imgCaptcha);
                    txtCaptcha.Text = "";
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdCaptcha_Click(object sender, ImageClickEventArgs e)
        {
            CCommon.LoadCaptcha(imgCaptcha);
        }
        protected void ddlNational_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!ddlNational.Visible) return;

                int national = int.Parse(ddlNational.SelectedValue);
                ddlCity.Items.Clear();
                if (national == 0) return;

                this.BindDDL_Region(ddlCity, national);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        #endregion
    }
}
