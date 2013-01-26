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
    public partial class usergroupu : BasePage
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

            this.Bind_dtlListRPages();
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
                    vlreturn += " >> <a href='usergroupu.aspx?pid=" + info.Id + "'>" + info.Name + "</a>";
                }
            }
            return vlreturn;
        }
        
        private bool Validata()
        {
            try
            {
                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(txtName.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_name, "", txtName);
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
                if (iid == 0)
                    info.Username = CFunctions.remove_blank(info.Name);
                info.Password = CFunctions.MBEncrypt("DFTY$FDSSDYE$#%");
                info.Email = info.Username + "@dangdong.vn";
                info.Pis = info.Pis == 0 ? 1 : info.Pis;
                if (PARENT != null)
                {
                    info.Pid = PARENT.Id;
                    info.Depth = PARENT.Depth + 1;
                }
                else
                {
                    info.Depth = info.Pid != 0 ? info.Depth : 1;
                }
                info.Status = CCommon.GetStatus_upt();
                info.Timeupdate = DateTime.Now;

                UserrightInfo rinfo = new UserrightInfo();
                rinfo.Id = info.Id;
                
                string RPages = "";
                if (dtlListRPages.Items.Count > 0)
                {
                    foreach (DataListItem row in dtlListRPages.Items)
                    {
                        string Navigateurl = dtlListRPages.DataKeys[row.ItemIndex].ToString();
                        CheckBox RPages_typeof = (CheckBox)row.FindControl("RPages_typeof");
                        ListBox RPages_cid = (ListBox)row.FindControl("RPages_cid");

                        if (RPages_typeof.Checked)
                        {
                            string page = Navigateurl.Replace("l.aspx", "");
                            RPages += page + "#";
                            foreach (ListItem item in RPages_cid.Items)
                            {
                                if (item.Selected)
                                    RPages += page + item.Value + "#";
                            }
                        }
                    }
                }
                rinfo.R_new = rinfo.R_upt = rinfo.R_del = "#" + RPages;

                rinfo.R_sys = "";
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
                return (new CUser()).Save(info);
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
                
                if (info.iRight == null)
                    info.iRight = (new CUserright()).Getinfo(info.Id);
                if (info.iRight != null)
                {
                    UserrightInfo rinfo = info.iRight;
                    chkr_sys.Checked = rinfo.R_sys == "" ? false : true;

                    foreach (DataListItem row in dtlListRPages.Items)
                    {
                        string Navigateurl = dtlListRPages.DataKeys[row.ItemIndex].ToString();
                        CheckBox RPages_typeof = (CheckBox)row.FindControl("RPages_typeof");
                        ListBox RPages_cid = (ListBox)row.FindControl("RPages_cid");
                        Panel divRPages = (Panel)row.FindControl("divRPages");

                        string page = Navigateurl.Replace("l.aspx", "");
                        RPages_typeof.Checked = rinfo.R_new.IndexOf(page + "#") != -1;
                        foreach (ListItem item in RPages_cid.Items)
                            item.Selected = rinfo.R_new.IndexOf(page + item.Value + "#") != -1;
                        divRPages.Attributes.Add("style", RPages_typeof.Checked ? "" : "display:none");

                    }
                }
                else
                {
                    chkr_sys.Checked = false;
                    foreach (DataListItem row in dtlListRPages.Items)
                    {
                        CheckBox RPages_typeof = (CheckBox)row.FindControl("RPages_typeof");
                        ListBox RPages_cid = (ListBox)row.FindControl("RPages_cid");
                        Panel divRPages = (Panel)row.FindControl("divRPages");

                        RPages_typeof.Checked = false;
                        foreach (ListItem item in RPages_cid.Items)
                            item.Selected = false;
                        divRPages.Attributes.Add("style", "display:none");

                    }
                }

                pnlForm.Enabled = info.Id != 1;
                chkr_sys.Visible = info.Id == 1;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Bind_dtlListRPages()
        {
            int numResults = 0;
            ListOptions options = Get_ListOptionsNoPaging();
            options.SortExp = Queryparam.Sqlcolumn.Orderd;
            options.GetAll = true;
            List<MenuInfo> list = (new CMenu(CCommon.LANG)).Getlist(1, 2, options, out numResults);

            (new GenericList<MenuInfo>()).Bind_DataList(dtlListRPages, null, list, numResults);
            dtlListRPages.Visible = numResults > 0;
            return;
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
                if (vlExist == CConstants.State.Existed.Name)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Exist_username, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                if (this.Save(info))
                {
                    // update permittion for all user in group
                    CUser VDAL = new CUser();
                    List<UserInfo> list = VDAL.Wcmm_Getlistuser(info.Id);
                    if (list != null && list.Count > 0)
                    {
                        foreach (UserInfo _info in list)
                        {
                            _info.iRight = info.iRight.copy();
                            VDAL.Save(_info);
                        }
                    }

                    lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Save_completed, "", null);
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

        protected void dtlListRPages_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            try
            {
                string Navigateurl = ((DataList)sender).DataKeys[e.Item.ItemIndex].ToString();

                CheckBox RPages_typeof = (CheckBox)e.Item.FindControl("RPages_typeof");
                ListBox RPages_cid = (ListBox)e.Item.FindControl("RPages_cid");
                Panel divRPages = (Panel)e.Item.FindControl("divRPages");
                if (Navigateurl == "productl.aspx" || Navigateurl == "servicel.aspx" || Navigateurl == "librariesl.aspx" || Navigateurl == "newsl.aspx")
                {
                    if (RPages_typeof != null)
                        RPages_typeof.Attributes.Add("onclick", "javascript:toggleRPages('" + divRPages.ClientID + "');");
                    
                    if (RPages_cid != null)
                    {
                        int cid = CFunctions.Get_Definecat(Navigateurl.Replace("l.aspx", ".aspx"));
                        BindDDL_Cid_LST(RPages_cid, cid, 0, "");
                        Insert_FirstitemLST(RPages_cid);
                    }
                }
                else
                {
                    if (RPages_cid != null)
                        RPages_cid.Visible = false;
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
