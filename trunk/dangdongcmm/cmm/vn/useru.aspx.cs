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
    public partial class useru : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PARENT = this.Get_Parent();
            if (!Page.IsPostBack)
            {
                this.Init_State();
                this.Load_Info(CCommon.Get_QueryNumber(Queryparam.Iid));
            }
        }
        private UserInfo PARENT = null;

        #region private methods
        private void Init_State()
        {
            int pid = PARENT == null ? 0 : PARENT.Id;
            string parentpath = this.Generate_Parentpath(pid);
            if (!CFunctions.IsNullOrEmpty(parentpath))
            {
                string notice = CCommon.Get_Definephrase(Definephrase.Display_havesub);
                notice = notice.Replace(Queryparam.Varstring.Depth, (PARENT == null ? 1 : PARENT.Depth + 1).ToString());
                notice = notice.Replace(Queryparam.Varstring.Path, parentpath);
                lblError.Text = notice;
            }

            this.BindDDL_Group(ddlGroup, 0, "");
            CCommon.Insert_FirstitemDDL(ddlGroup);
        }
        private UserInfo Get_Parent()
        {
            int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
            UserInfo parent = (new CUser()).Wcmm_Getinfo(pid);
            return parent;
        }
        private string Generate_Parentpath(int pid)
        {
            string vlreturn = "";
            List<UserInfo> list_parent = (new CUser()).Getlist_parent(pid);
            if (list_parent != null)
            {
                for (int i = 0; i < list_parent.Count; i++)
                {
                    UserInfo info = list_parent[i];
                    vlreturn += " >> <a href='useru.aspx?pid=" + info.Id + "'>" + info.Name + "</a>";
                }
            }
            return vlreturn;
        }
        private void BindDDL_Group(DropDownList ddl, int pid, string separator)
        {
            List<UserInfo> list = (new CUser()).Wcmm_Getlistgroup(pid);
            if (list == null) return;

            foreach (UserInfo info in list)
            {
                string sep = pid == 0 ? "" : separator + "---";
                ListItem item = new ListItem(sep + info.Name, info.Id.ToString());
                ddl.Items.Add(item);
                this.BindDDL_Group(ddl, info.Id, sep);
            }
        }
        
        private bool Validata()
        {
            try
            {
                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(txtName.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_name, "", txtName);
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
        private UserInfo Take()
        {
            try
            {
                int iid = 0;
                int.TryParse(txtId.Value, out iid);
                UserInfo info = (new CUser()).Wcmm_Getinfo(iid);
                if (info == null)
                    info = new UserInfo();
                info.Id = iid;
                info.Name = txtName.Text.Trim();
                info.Username = txtUsername.Text.Trim();
                info.Password = CFunctions.MBEncrypt(txtPassword.Text);
                info.Email = txtEmail.Text.Trim();
                info.Pis = 0;
                info.Pid = int.Parse(ddlGroup.SelectedValue);
                info.Depth = PARENT == null ? 1 : PARENT.Depth + 1;
                info.Status = Displaysetting.Get_Status();
                info.Timeupdate = DateTime.Now;

                UserrightInfo rinfo = (new CUserright()).Getinfo(info.Pid);
                info.iRight = rinfo;
                
                return info;
            }
            catch
            {
                return null;
            }
        }
        private bool Save(UserInfo info)
        {
            try
            {
                if (info == null) return false;
                int iid = info.Id;
                if ((new CUser()).Save(info))
                    if (PARENT != null && iid == 0)
                        (new CUser()).Updatenum(PARENT.Id.ToString(), Queryparam.Sqlcolumn.Pis, CConstants.NUM_INCREASE);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool Load_Info(int iid)
        {
            try
            {
                UserInfo info = null;
                if (iid != 0)
                {
                    info = (new CUser()).Wcmm_Getinfo(iid);
                    if (info != null)
                    {
                        lstError = new List<Errorobject>();
                        lstError = Form_GetError(lstError, Errortype.Notice, Definephrase.Save_notice, "[" + info.Id + "] " + info.Name, null);
                        Master.Form_ShowError(lstError);
                    }
                }
                if (info == null)
                    info = new UserInfo();
                chkSaveoption_golist.Checked = info.Id != 0;
                
                txtId.Value = info.Id.ToString();
                txtName.Text = info.Name;
                txtUsername.Text = info.Username;
                txtPassword.Text = CFunctions.MBDecrypt(info.Password);
                txtEmail.Text = info.Email;
                ddlGroup.SelectedValue = info.Id == 0 ? (PARENT == null ? "0" : PARENT.Id.ToString()) : info.Pid.ToString();
                if (info.Id == 2)
                {
                    txtUsername.Enabled = false;
                    ddlGroup.Enabled = false;
                }
                Displaysetting.Set("", info.Status, 0);
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
                UserInfo info = this.Take();
                if (info == null)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Takeinfo_error, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                CConstants.State.Existed vlExist = (new CUser()).Exist(info);
                if (vlExist != CConstants.State.Existed.None)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, vlExist == CConstants.State.Existed.Name ? Definephrase.Exist_username : Definephrase.Exist_email, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                if (this.Save(info))
                {
                    info.Password = txtPassword.Text;
                    CCommon.Session_Set(Sessionparam.USERREGISTER, info);

                    lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Save_completed, "", null);
                    Master.Form_ShowError(lstError);
                    if (chkSaveoption_golist.Checked)
                        this.Load_Info(info.Id);
                    Form_SaveOption(!chkSaveoption_golist.Checked);

                    if (info.Status == (int)CConstants.State.Status.Actived)
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Createuser", "Createuser();", true);
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
