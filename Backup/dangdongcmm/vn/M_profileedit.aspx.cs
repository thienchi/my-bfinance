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
    public partial class M_profileedit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Loadinfo();
            }
        }

        public string oldEmail, oldPhone;

        #region private methods
        private void Loadinfo()
        {
            MemberInfo member = CCommon.CurrentMember;
            if (member == null) return;

            lblFullname.Text = member.Username;
            txtFullname.Text = member.Fullname;
            txtEmail.Text = member.Email;
            txtAddress.Text = member.iProfile.Address;
            txtPhone.Text = member.iProfile.Phone;
            txtSkype.Text = CFunctions.IsNullOrEmpty(member.iProfile.Skype) ? "" : member.iProfile.Skypetext;
            txtYahoo.Text = CFunctions.IsNullOrEmpty(member.iProfile.Yahoo) ? "" : member.iProfile.Yahootext;

            radSkypesh.SelectedValue = member.iProfile.Skypesh;
            radYahoosh.SelectedValue = member.iProfile.Yahoosh;
        }

        private MemberInfo Takeinfo()
        {
            try
            {
                MemberInfo member = CCommon.CurrentMember.copy();
                oldEmail = member.Email;
                oldPhone = member.iProfile.Phone;

                string Fullname = txtFullname.Text.Trim();
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
                member.Email = txtEmail.Text.Trim();
                member.iProfile.Address = txtAddress.Text.Trim();
                member.iProfile.Phone = txtPhone.Text.Trim();
                member.iProfile.Skype = radSkypesh.SelectedValue + txtSkype.Text.Trim();
                member.iProfile.Yahoo = radYahoosh.SelectedValue + txtYahoo.Text.Trim();
                
                return member;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region events
        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                lstError = new List<Errorobject>();
                MemberInfo member = this.Takeinfo();
                if (member == null)
                {
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Takeinfo_error, "", null);
                    CCommon.Form_ShowError(lstError, lblError);
                    return;
                }
                string passwordold = CFunctions.MBEncrypt(txtPassword.Text);
                if (passwordold != member.Password)
                {
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_password, "", txtPassword);
                    CCommon.Form_ShowError(lstError, lblError);
                    return;
                }

                CMember DAL = new CMember(CCommon.LANG);
                CConstants.State.Existed exist = DAL.Exist(member);
                if ((exist == CConstants.State.Existed.Phone && !CFunctions.IsNullOrEmpty(txtPhone.Text)) || (exist == CConstants.State.Existed.Mail && !CFunctions.IsNullOrEmpty(txtEmail.Text)))
                {
                    switch (exist)
                    {
                        case CConstants.State.Existed.Phone:
                            lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Exist_phone, "", txtPhone);
                            break;
                        case CConstants.State.Existed.Mail:
                            lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Exist_email, "", txtEmail);
                            break;
                    }
                    CCommon.Form_ShowError(lstError, lblError);
                    return;
                }
                
                if (DAL.Save(member))
                {
                    new CMeProfile(CCommon.LANG).Save(member.iProfile);
                    CCommon.Session_Set(Sessionparam.WEBUSERLOGIN, member);

                    if (oldEmail != member.Email || oldPhone != member.iProfile.Phone)
                    {
                        member.Filepreview = member.Username + " thay đổi " + (oldEmail != member.Email ? "email, " : "") + (oldPhone != member.iProfile.Phone ? "phone, " : "");
                        member.Filepreview = member.Filepreview.Remove(member.Filepreview.LastIndexOf(", "));
                        CCommon.Session_Set(Sessionparam.WEBUSERREGISTER, member);
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Register", "Registerchanged();", true);

                    pnlForm.Visible = false;
                    lstError = CCommon.Form_GetError(lstError, Errortype.Notice, Definephrase.Account_Update_done, "", null);
                    CCommon.Form_ShowError(lstError, lblError);
                    return;
                }
                else
                {
                    pnlForm.Visible = false;
                    lstError = CCommon.Form_GetError(lstError, Errortype.Notice, Definephrase.Save_error, "", null);
                    CCommon.Form_ShowError(lstError, lblError);
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }

        #endregion
    }
}
