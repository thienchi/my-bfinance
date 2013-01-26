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
    public partial class librariesl : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Init_State();
                this.Bind_grdView();
            }
        }
        
        #region private methods
        private string Generate_Parentpath(int cid)
        {
            string vlreturn = "";
            List<CategoryInfo> listparent = (new CCategory(CCommon.LANG)).Wcmm_Getlist_parent(cid);
            if (listparent != null && listparent.Count > 0)
                foreach (CategoryInfo parent in listparent)
                    vlreturn += (CFunctions.IsNullOrEmpty(vlreturn) ? "" : " >> ") + "<a href='librariesl.aspx?cid=" + parent.Id + "'>" + parent.Name + "</a>";

            if (CFunctions.IsNullOrEmpty(vlreturn))
            {
                CategorytypeofInfo infotypeof = (new CCategorytypeof(CCommon.LANG)).Wcmm_Getinfo(Webcmm.Id.Libraries);
                vlreturn += (infotypeof == null ? "" : " <a href='librariesl.aspx?cid=0'>" + infotypeof.Name + "</a>");
            }
            return vlreturn;
        }
        private void Init_State()
        {
            int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
            lblError.Text = CCommon.Get_Definephrase(Definephrase.Display_havesub_news).Replace(Queryparam.Varstring.Path, this.Generate_Parentpath(cid));

            cmdAdd.Attributes.Add("onclick", "CC_gotoUrl('librariesu.aspx?cid=" + cid + "')");
            this.BindDDL_Cid(ddlCid);
            if (ddlCid.Items.FindByValue(cid.ToString()) != null)
                ddlCid.SelectedValue = cid.ToString();
        }

        private List<LibrariesInfo> Search(out int numResults)
        {
            numResults = 0;
            int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
            string keywords = txtKeyword.Text.Trim();
            List<LibrariesInfo> list = (new CLibraries(CCommon.LANG)).Wcmm_Search(cid, keywords, Get_ListOptions(), out numResults);
            return list;
        }
        private List<LibrariesInfo> Load_List(out int numResults)
        {
            numResults = 0;
            int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
            List<LibrariesInfo> list = (new CLibraries(CCommon.LANG)).Wcmm_Getlist(cid, Get_ListOptions(), out numResults);
            return list;
        }
        public override void Bind_grdView()
        {
            int numResults = 0;
            List<LibrariesInfo> list = CFunctions.IsNullOrEmpty(txtKeyword.Text.Trim()) ? this.Load_List(out numResults) : this.Search(out numResults);
            (new GenericList<LibrariesInfo>(PageIndex, PageSize, SortExp, SortDir)).Bind_GridView(grdView, pagBuilder, list, numResults);
            return;
        }

        private void BindDDL_Cid(DropDownList ddl)
        {
            this.BindDDL_Cid(ddl, 0, "");
            CCommon.Insert_FirstitemDDL(ddl);
        }
        private void BindDDL_Cid(DropDownList ddl, int pid, string separator)
        {
            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Wcmm_Getlist(Webcmm.Id.Libraries, pid, Get_ListOptionsNoPaging());
            if (list == null) return;

            foreach (CategoryInfo info in list)
            {
                string sep = pid == 0 ? "" : separator + "---";
                ListItem item = new ListItem(sep + info.Name, info.Id.ToString());
                ddl.Items.Add(item);
                this.BindDDL_Cid(ddl, info.Id, sep);
            }
        }

        private void Copy_Info(LibrariesInfo source_info, int dest_cid, CLibraries BLL)
        {
            if (source_info == null) return;

            LibrariesInfo info_copy = source_info.copy();
            info_copy.Id = 0;
            info_copy.Cid = dest_cid;
            info_copy.Status = CCommon.GetStatus_upt();
            info_copy.Username = CCommon.Get_CurrentUsername();
            info_copy.Timeupdate = DateTime.Now;
            info_copy.Allowcomment = source_info.Allowcomment > 0 ? 1 : 0;
            info_copy.Orderd = 0;
            BLL.Save(info_copy);
        }
        private void Move_Info(LibrariesInfo source_info, int dest_cid, CLibraries BLL)
        {
            if (source_info == null) return;

            LibrariesInfo info_copy = source_info.copy();
            info_copy.Cid = dest_cid;
            info_copy.Status = CCommon.GetStatus_upt();
            info_copy.Username = CCommon.Get_CurrentUsername();
            info_copy.Timeupdate = DateTime.Now;
            BLL.Save(info_copy);
        }
        #endregion

        #region events
        protected void grdView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                GridView gridView = (GridView)sender;
                BindData_Sorting(gridView, e);
                BuildCommand_Delete(e);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void grdView_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                SortExp = e.SortExpression;
                SortDir = SortDir == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
                this.Bind_grdView();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void grdView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                string iid = grdView.DataKeys[e.RowIndex].Value.ToString();
                if (CFunctions.IsNullOrEmpty(iid)) return;

                string vlreturn = Delete(Webcmm.Id.Libraries, iid);
                lstError = new List<Errorobject>();
                lstError = Form_GetError(lstError, vlreturn == Definephrase.Remove_completed ? Errortype.Completed : Errortype.Error, vlreturn, "", null);
                Master.Form_ShowError(lstError);
                this.Bind_grdView();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void grdView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int iid = 0;
                int.TryParse(e.CommandArgument.ToString(), out iid);
                switch (e.CommandName)
                {
                    case Commandparam.Setupattribute:
                        LibrariesInfo info = (new CLibraries(CCommon.LANG)).Wcmm_Getinfo(iid);
                        if (info != null)
                            Setupattribute.Show_Dialog(Webcmm.Id.Libraries, iid.ToString(), info.Iconex, info.Status, info.Markas);
                        break;
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            try
            {
                PageIndex = 1;
                this.Bind_grdView();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string iid = CCommon.Get_FormString(Queryparam.Chkcheck);
                if (CFunctions.IsNullOrEmpty(iid)) return;

                string vlreturn = Delete(Webcmm.Id.Libraries, iid);
                lstError = new List<Errorobject>();
                lstError = Form_GetError(lstError, vlreturn == Definephrase.Remove_completed ? Errortype.Completed : Errortype.Error, vlreturn, "", null);
                Master.Form_ShowError(lstError);
                this.Bind_grdView();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdCopy_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string iid = CCommon.Get_FormString(Queryparam.Chkcheck);
                if (CFunctions.IsNullOrEmpty(iid))
                {
                    lstError = new List<Errorobject>();
                    lstError = Form_GetError(lstError, Errortype.Warning, Definephrase.Notice_noselecteditem, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }
                Master.Form_ClearError();

                List<LibrariesInfo> list = (new CLibraries(CCommon.LANG)).Wcmm_Getlist(iid);
                if (list == null) return;

                grdCopy.DataSource = list;
                grdCopy.DataBind();
                pnlList.Visible = false;
                pnlCopy.Visible = !pnlList.Visible;
                if (ddlCopycid.Items.Count == 0) 
                    this.BindDDL_Cid(ddlCopycid);

                string iidstr = "";
                foreach (LibrariesInfo info in list)
                {
                    iidstr += info.Id + ",";
                }
                txtIidstr.Value = iidstr.Remove(iidstr.Length - 1);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdCopyOk_Click(object sender, EventArgs e)
        {
            try
            {
                string iidstr = txtIidstr.Value;
                int dest_cid = int.Parse(ddlCopycid.SelectedValue);

                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(iidstr) || dest_cid == 0)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Copy_error, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                CLibraries BLL = new CLibraries(CCommon.LANG);
                string[] iidarr = iidstr.Split(',');
                for (int i = 0; i < iidarr.Length; i++)
                {
                    LibrariesInfo info = BLL.Wcmm_Getinfo(int.Parse(iidarr[i]));
                    this.Copy_Info(info, dest_cid, BLL);
                }

                cmdCopyOk.Enabled = false;
                lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Copy_completed, "", null);
                Master.Form_ShowError(lstError);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdMove_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string iid = CCommon.Get_FormString(Queryparam.Chkcheck);
                if (CFunctions.IsNullOrEmpty(iid))
                {
                    lstError = new List<Errorobject>();
                    lstError = Form_GetError(lstError, Errortype.Warning, Definephrase.Notice_noselecteditem, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }
                Master.Form_ClearError();

                List<LibrariesInfo> list = (new CLibraries(CCommon.LANG)).Wcmm_Getlist(iid);
                if (list == null) return;

                grdMove.DataSource = list;
                grdMove.DataBind();
                pnlList.Visible = false;
                pnlMove.Visible = !pnlList.Visible;
                if (ddlMovecid.Items.Count == 0) 
                    this.BindDDL_Cid(ddlMovecid);

                string iidstr = "";
                foreach (LibrariesInfo info in list)
                {
                    iidstr += info.Id + ",";
                }
                txtIidstr.Value = iidstr.Remove(iidstr.Length - 1);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdMoveOk_Click(object sender, EventArgs e)
        {
            try
            {
                string iidstr = txtIidstr.Value;
                int dest_cid = int.Parse(ddlMovecid.SelectedValue);
                
                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(iidstr) || dest_cid == 0)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Move_error, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                CLibraries BLL = new CLibraries(CCommon.LANG);
                string[] iidarr = iidstr.Split(',');
                for (int i = 0; i < iidarr.Length; i++)
                {
                    LibrariesInfo info = BLL.Wcmm_Getinfo(int.Parse(iidarr[i]));
                    this.Move_Info(info, dest_cid, BLL);
                }

                cmdMoveOk.Enabled = false;
                lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Move_completed, "", null);
                Master.Form_ShowError(lstError);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdUpdateorder_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (grdView.Rows.Count == 0) return;

                CLibraries BLL = new CLibraries(CCommon.LANG);
                foreach (GridViewRow row in grdView.Rows)
                {
                    HiddenField txtId = (HiddenField)row.FindControl("txtId");
                    TextBox txtOrderd = (TextBox)row.FindControl("txtOrderd");
                    if (txtId != null && txtOrderd != null)
                    {
                        int id = int.Parse(txtId.Value);
                        int orderd = 0;
                        int.TryParse(txtOrderd.Text, out orderd);
                        BLL.Updatenum(id.ToString(), Queryparam.Sqlcolumn.Orderd, orderd);
                    }
                    lstError = new List<Errorobject>();
                    lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Saveorderd_completed, "", null);
                    Master.Form_ShowError(lstError);
                }
            }
            catch
            {
                lstError = new List<Errorobject>();
                lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Saveorderd_error, "", null);
                Master.Form_ShowError(lstError); 
            }
        }
        protected void cmdSetupattribute_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string iid = CCommon.Get_FormString(Queryparam.Chkcheck);
                if (CFunctions.IsNullOrEmpty(iid)) return;

                if (iid.IndexOf(',') == -1)
                {
                    LibrariesInfo info = (new CLibraries(CCommon.LANG)).Wcmm_Getinfo(int.Parse(iid));
                    if (info != null)
                        Setupattribute.Show_Dialog(Webcmm.Id.Libraries, iid, info.Iconex, info.Status, info.Markas);
                }
                else
                    Setupattribute.Show_Dialog(Webcmm.Id.Libraries, iid, Queryparam.Defstring.Nospecifystr, Queryparam.Defstring.Nospecifyint, Queryparam.Defstring.Nospecifyint);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        
        protected void ddlAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (ddlAction.SelectedValue)
                {
                    case Commandparam.Delete:
                        this.cmdDelete_Click(null, null);
                        break;
                    case Commandparam.Copy:
                        this.cmdCopy_Click(null, null);
                        break;
                    case Commandparam.Move:
                        this.cmdMove_Click(null, null);
                        break;
                    case Commandparam.Updatedisplayorder:
                        this.cmdUpdateorder_Click(null, null);
                        break;
                    case Commandparam.Setupattribute:
                        this.cmdSetupattribute_Click(null, null);
                        break;
                }
                ddlAction.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        #endregion
    }
}
