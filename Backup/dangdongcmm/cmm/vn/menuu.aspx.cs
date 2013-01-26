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
    public partial class menuu : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PARENT = this.Get_Parent();
            if (!Page.IsPostBack)
            {
                this.Init_State();
                this.BindDDL_Cid();
                this.BindDDL_Cataloguetypeof();
                this.Load_Info(CCommon.Get_QueryNumber(Queryparam.Iid));
            }
        }
        private MenuInfo PARENT = null;
    
        #region private methods
        private void Init_State()
        {
            int pid = PARENT == null ? 0 : PARENT.Id;
            int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
            string parentpath = this.Generate_Parentpath(pid, cid);
            if (!CFunctions.IsNullOrEmpty(parentpath))
            {
                string notice = CCommon.Get_Definephrase(Definephrase.Display_havesub);
                notice = notice.Replace(Queryparam.Varstring.Depth, (PARENT == null ? 1 : PARENT.Depth + 1).ToString());
                notice = notice.Replace(Queryparam.Varstring.Path, parentpath);
                lblError.Text = notice;
            }

            ddlCid.Enabled = PARENT == null;
            chkSaveoption_golang.Visible = CFunctions.IsMultiLanguage();
        }
        private MenuInfo Get_Parent()
        {
            int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
            MenuInfo parent = (new CMenu(CCommon.LANG)).Wcmm_Getinfo(pid);
            return parent;
        }
        private string Generate_Parentpath(int pid, int cid)
        {
            string vlreturn = "";
            List<MenuInfo> list_parent = (new CMenu(CCommon.LANG)).Wcmm_Getlist_parent(pid);
            if (list_parent != null)
            {
                for (int i = 0; i < list_parent.Count; i++)
                {
                    MenuInfo info = list_parent[i];
                    vlreturn += " >> <a href='menuu.aspx?cid=" + info.Cid + "&pid=" + info.Id + "'>" + info.Name + "</a>";
                    cid = info.Cid;
                }
            }
            MenutypeofInfo infotypeof = (new CMenutypeof(CCommon.LANG)).Wcmm_Getinfo(cid);
            vlreturn = (infotypeof == null ? "" : " <a href='menuu.aspx?cid=" + infotypeof.Id + "'>" + infotypeof.Name + "</a>") + vlreturn;
            return vlreturn;
        }
        private void BindDDL_Cid()
        {
            List<MenutypeofInfo> list = (new CMenutypeof(CCommon.LANG)).Wcmm_Getlist((int)CConstants.State.Status.Actived);
            if (list == null || list.Count == 0) return;

            ddlCid.DataSource = list;
            ddlCid.DataTextField = "Name";
            ddlCid.DataValueField = "Id";
            ddlCid.DataBind();
            CCommon.Insert_FirstitemDDL(ddlCid);
        }
        private void BindDDL_Cataloguetypeof()
        {
            List<CategorytypeofInfo> list = (new CCategorytypeof(CCommon.LANG)).Wcmm_Getlist((int)CConstants.State.Status.Actived, (int)CConstants.State.MarkAs.None);
            if (list == null || list.Count == 0) return;

            ddlCataloguetypeof.DataSource = list;
            ddlCataloguetypeof.DataTextField = "Name";
            ddlCataloguetypeof.DataValueField = "Id";
            ddlCataloguetypeof.DataBind();
            CCommon.Insert_FirstitemDDL(ddlCataloguetypeof);
        }
    
        private bool Validata()
        {
            try
            {
                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(ddlCid.SelectedValue) || ddlCid.SelectedValue == "0")
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_catalogue, "", ddlCid);
                //if (CFunctions.IsNullOrEmpty(txtName.Text))
                //    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_name, "", txtName);
                return Master.Form_ShowError(lstError);
            }
            catch
            {
                return false;
            }
        }
        private MenuInfo Take()
        {
            try
            {
                int iid = 0;
                int.TryParse(txtId.Value, out iid);
                MenuInfo info = (new CMenu(CCommon.LANG)).Wcmm_Getinfo(iid);
                if (info == null)
                    info = new MenuInfo();
                info.Id = iid;
                info.Name = txtName.Text.Trim();
                info.Code = "";
                info.Note = txtNote.Text.Trim();
                info.Navigateurl = txtNavigateurl.Text.Trim();
                info.Tooltip = txtTooltip.Text.Trim();
                info.Attributes = txtAttributes.Text.Trim();
                info.ApplyAttributesChild = chkApplyAttributesChild.Checked ? 1 : 0;
                info.Visible = chkVisible.Checked ? 0 : 1;
                info.Cataloguetypeofid = int.Parse(ddlCataloguetypeof.SelectedValue);
                if (ddlCatalogue.Visible)
                    info.Catalogueid = int.Parse(ddlCatalogue.SelectedValue);
                info.Insertcatalogue = chkInsertcatalogue.Checked ? 1 : 0;
                info.Noroot = chkNoroot.Checked ? 1 : 0;
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
                info.Status = Displaysetting.Get_Status();
                info.Orderd = Displaysetting.Get_Orderd();
                info.Username = CCommon.Get_CurrentUsername();
                info.Timeupdate = DateTime.Now;
                return info;
            }
            catch
            {
                return null;
            }
        }
        private bool Save(MenuInfo info)
        {
            try
            {
                if (info == null) return false;
                int iid = info.Id;
                if ((new CMenu(CCommon.LANG)).Save(info))
                    if (PARENT != null && iid == 0)
                        (new CMenu(CCommon.LANG)).Updatenum(PARENT.Id.ToString(), Queryparam.Sqlcolumn.Pis, CConstants.NUM_INCREASE);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool Save_Lang(MenuInfo info)
        {
            try
            {
                if (!CFunctions.IsMultiLanguage() || !chkSaveoption_golang.Checked) return false;

                int lang_num = CConstants.LANG_NUM;
                for (int i = 0; i < lang_num; i++)
                {
                    string lang_val = ConfigurationSettings.AppSettings["LANG_" + i];
                    if (lang_val == CCommon.LANG) continue;

                    MenuInfo lang_info = info.copy();
                    lang_info.Id = 0;
                    lang_info.Status = (int)CConstants.State.Status.Waitactive;
                    (new CMenu(lang_val)).Save(lang_info);
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
                MenuInfo info = null;
                if (iid != 0)
                {
                    info = (new CMenu(CCommon.LANG)).Wcmm_Getinfo(iid);
                    if (info != null)
                    {
                        lstError = new List<Errorobject>();
                        lstError = Form_GetError(lstError, Errortype.Notice, Definephrase.Save_notice, "[" + info.Id + "] " + info.Name, null);
                        Master.Form_ShowError(lstError);
                    }
                }
                if (info == null)
                    info = new MenuInfo();
                chkSaveoption_golist.Checked = info.Id != 0;
                chkSaveoption_golang.Checked = info.Id == 0;

                txtId.Value = info.Id.ToString();
                txtName.Text = info.Name;
                txtNote.Text = info.Note;
                txtNavigateurl.Text = info.Navigateurl;
                txtTooltip.Text = info.Tooltip;
                txtAttributes.Text = info.Attributes;
                chkApplyAttributesChild.Checked = info.ApplyAttributesChild == 1;
                chkVisible.Checked = (info.Visible == 0 ? (info.Id == 0 ? false : true) : false);
                ddlCataloguetypeof.SelectedValue = info.Cataloguetypeofid.ToString();
                if (info.Cataloguetypeofid != 0)
                {
                    this.ddlCataloguetypeof_SelectedIndexChanged(null, null);
                    ddlCatalogue.SelectedValue = info.Catalogueid.ToString();
                }
                chkInsertcatalogue.Checked = info.Insertcatalogue == 1;
                chkNoroot.Checked = info.Noroot == 1;
                ddlCid.SelectedValue = PARENT != null ? PARENT.Cid.ToString() : (info.Cid != 0 ? info.Cid.ToString() : CCommon.Get_QueryNumber(Queryparam.Cid).ToString());
                chkPis.Checked = info.Pis != 0;
                chkPis.ToolTip = info.Pis.ToString();
                chkPis.Enabled = !(info.Pis > 1);
                chkPis.Text = CCommon.Get_Definephrase(Definephrase.Display_pis).Replace(Queryparam.Varstring.Depth, (PARENT == null ? 2 : PARENT.Depth + 2).ToString());
                Displaysetting.Set("", info.Status, info.Orderd);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region events
        protected void ddlCataloguetypeof_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int belongto = int.Parse(ddlCataloguetypeof.SelectedValue);
                ddlCatalogue.Visible = belongto != 0;
                ddlCatalogue.Items.Clear();
                BindDDL_Cid(ddlCatalogue, belongto, 0, "");
                CCommon.Insert_FirstitemDDL(ddlCatalogue);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.Validata()) return;

                lstError = new List<Errorobject>();
                MenuInfo info = this.Take();
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
