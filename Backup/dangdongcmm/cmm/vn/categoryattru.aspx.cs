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
    public partial class categoryattru : BasePage
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
        private CategoryattrInfo PARENT = null;
    
        #region private methods
        private void Init_State()
        {
            int pid = PARENT == null ? 0 : PARENT.Id;
            int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
            string parentpath = this.Generate_Parentpath(pid, cid);
            if (!CFunctions.IsNullOrEmpty(parentpath))
            {
                string notice = CCommon.Get_Definephrase(Definephrase.Display_havesub_attr);
                notice = notice.Replace(Queryparam.Varstring.Depth, (PARENT == null ? 1 : PARENT.Depth + 1).ToString());
                notice = notice.Replace(Queryparam.Varstring.Path, parentpath);
                lblError.Text = notice;
            }
            
            ddlCid.Enabled = PARENT == null;
            chkSaveoption_golang.Visible = CFunctions.IsMultiLanguage();

            this.BindDDL_Cid(ddlCid);
        }
        private CategoryattrInfo Get_Parent()
        {
            int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
            CategoryattrInfo parent = (new CCategoryattr(CCommon.LANG)).Wcmm_Getinfo(pid);
            return parent;
        }
        private string Generate_Parentpath(int pid, int cid)
        {
            string vlreturn = "";
            List<CategoryattrInfo> listparent = (new CCategoryattr(CCommon.LANG)).Wcmm_Getlist_parent(pid);
            if (listparent != null && listparent.Count > 0)
                foreach (CategoryattrInfo parent in listparent)
                    vlreturn += " >> <a href='categoryattru.aspx?cid=" + parent.Cid + "&pid=" + parent.Id + "'>" + parent.Name + "</a>";

            CategorytypeofInfo infotypeof = (new CCategorytypeof(CCommon.LANG)).Wcmm_Getinfo(cid);
            vlreturn = (infotypeof == null ? "" : " <a href='categoryattru.aspx?cid=" + infotypeof.Id + "'>" + infotypeof.Name + "</a>") + vlreturn;
            return vlreturn;
        }
        private void BindDDL_Cid(DropDownList ddl)
        {
            List<CategorytypeofInfo> list = (new CCategorytypeof(CCommon.LANG)).Wcmm_Getlist((int)CConstants.State.Status.Actived, (int)CConstants.State.MarkAs.None);
            if (list == null || list.Count == 0) return;

            ddl.DataSource = list;
            ddl.DataTextField = "Name";
            ddl.DataValueField = "Id";
            ddl.DataBind();
            CCommon.Insert_FirstitemDDL(ddl);
        }
        
        private bool Validata()
        {
            try
            {
                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(ddlCid.SelectedValue) || ddlCid.SelectedValue == "0")
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_catalogue, "", ddlCid);
                if (CFunctions.IsNullOrEmpty(txtName.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_name, "", txtName);
                return Master.Form_ShowError(lstError);
            }
            catch
            {
                return false;
            }
        }
        private CategoryattrInfo Take()
        {
            try
            {
                int iid = 0;
                int.TryParse(txtId.Value, out iid);
                CategoryattrInfo info = (new CCategoryattr(CCommon.LANG)).Wcmm_Getinfo(iid);
                if (info == null)
                    info = new CategoryattrInfo();
                info.Id = iid;
                info.Name = txtName.Text.Trim();
                info.Code = txtCode.Text.Trim();
                info.Url = txtUrl.Text.Trim();
                info.Note = txtNote.Text.Trim();
                info.Cid = int.Parse(ddlCid.SelectedValue);
                info.Pis = chkPis.Checked ? (chkPis.ToolTip == "0" ? 1 : int.Parse(chkPis.ToolTip)) : 0;
                if (PARENT != null)
                {
                    info.Pid = PARENT.Id;
                    info.Depth = PARENT.Depth + 1;
                }
                else
                {
                    info.Depth = info.Pid != 0 ? info.Depth : 1;
                }
                info.Filepreview = Filepreview.Get();
                info.Iconex = Displaysetting.Get_Icon();
                info.Status = Displaysetting.Get_Status();
                info.Orderd = Displaysetting.Get_Orderd();
                info.Markas = Displaysetting.Get_Markas();
                info.Username = CCommon.Get_CurrentUsername();
                info.Timeupdate = DateTime.Now;
                return info;
            }
            catch
            {
                return null;
            }
        }
        private bool Save(CategoryattrInfo info)
        {
            try
            {
                if (info == null) return false;
                int iid = info.Id;
                if ((new CCategoryattr(CCommon.LANG)).Save(info))
                    if (PARENT != null && iid == 0)
                        (new CCategoryattr(CCommon.LANG)).Updatenum(PARENT.Id.ToString(), Queryparam.Sqlcolumn.Pis, CConstants.NUM_INCREASE);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool Save_Lang(CategoryattrInfo info)
        {
            try
            {
                if (!CFunctions.IsMultiLanguage() || !chkSaveoption_golang.Checked) return false;

                int lang_num = CConstants.LANG_NUM;
                for (int i = 0; i < lang_num; i++)
                {
                    string lang_val = ConfigurationSettings.AppSettings["LANG_" + i];
                    if (lang_val == CCommon.LANG) continue;

                    CategoryattrInfo lang_info = info.copy();
                    lang_info.Id = 0;
                    lang_info.Status = (int)CConstants.State.Status.Waitactive;
                    (new CCategoryattr(lang_val)).Save(lang_info);
                }
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
                CategoryattrInfo info = null;
                if (iid != 0)
                {
                    info = (new CCategoryattr(CCommon.LANG)).Wcmm_Getinfo(iid);
                    if (info != null)
                    {
                        lstError = new List<Errorobject>();
                        lstError = Form_GetError(lstError, Errortype.Notice, Definephrase.Save_notice, "[" + info.Id + "] " + info.Name, null);
                        Master.Form_ShowError(lstError);
                    }
                }
                if (info == null)
                    info = new CategoryattrInfo();
                chkSaveoption_golist.Checked = info.Id != 0;
                chkSaveoption_golang.Checked = info.Id == 0;

                txtId.Value = info.Id.ToString();
                txtName.Text = info.Name;
                txtCode.Text = info.Code;
                txtUrl.Text = info.Url;
                txtNote.Text = info.Note;
                ddlCid.SelectedValue = PARENT != null ? PARENT.Cid.ToString() : (info.Cid != 0 ? info.Cid.ToString() : CCommon.Get_QueryNumber(Queryparam.Cid).ToString());
                chkPis.Checked = info.Pis != 0;
                chkPis.ToolTip = info.Pis.ToString();
                chkPis.Enabled = !(info.Pis > 1);
                chkPis.Text = CCommon.Get_Definephrase(Definephrase.Display_pis).Replace(Queryparam.Varstring.Depth, (PARENT == null ? 2 : PARENT.Depth + 2).ToString());
                Filepreview.Set(info.Filepreview);
                Displaysetting.Set(info.Iconex, info.Status, info.Orderd, info.Markas);
                
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
                CategoryattrInfo info = this.Take();
                if (info == null)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Takeinfo_error, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                if (this.Save(info))
                {
                    lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Save_completed, "", null);
                    if (this.Save_Lang(info))
                        lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Savemultilang_completed, "", null);
                    Master.Form_ShowError(lstError);
                    if (chkSaveoption_golist.Checked)
                        this.Load_Info(info.Id);
                    Form_SaveOption(!chkSaveoption_golist.Checked);
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
        #endregion
    }
}
