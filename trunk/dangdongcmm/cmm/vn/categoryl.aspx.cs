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
    public partial class categoryl : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PARENT = this.Get_Parent();
            if (!Page.IsPostBack)
            {
                this.Init_State();
                this.Bind_grdView();
            }
        }
        private CategoryInfo PARENT = null;

        #region private methods
        private CategoryInfo Get_Parent()
        {
            int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
            CategoryInfo parent = (new CCategory(CCommon.LANG)).Wcmm_Getinfo(pid);
            return parent;
        }
        private string Generate_Parentpath(int pid, int cid)
        {
            string vlreturn = "";
            List<CategoryInfo> listparent = (new CCategory(CCommon.LANG)).Wcmm_Getlist_parent(pid);
            if (listparent != null && listparent.Count > 0)
                foreach (CategoryInfo parent in listparent)
                    vlreturn += " >> <a href='categoryl.aspx?cid=" + parent.Cid + "&pid=" + parent.Id + "'>" + parent.Name + "</a>";

            CategorytypeofInfo infotypeof = (new CCategorytypeof(CCommon.LANG)).Wcmm_Getinfo(cid);
            vlreturn = (infotypeof == null ? "" : " <a href='categoryl.aspx?cid=" + infotypeof.Id + "'>" + infotypeof.Name + "</a>") + vlreturn;
            return vlreturn;
        }
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
                lblPath.Text = notice;
            }

            cmdAdd.Attributes.Add("onclick", "CC_gotoUrl('categoryu.aspx?cid=" + cid + "&pid=" + pid + "')");
            this.BindDDL_Cid(ddlCid);
            if (ddlCid.Items.FindByValue(cid.ToString()) != null)
                ddlCid.SelectedValue = cid.ToString();
        }

        private List<CategoryInfo> Search(out int numResults)
        {
            numResults = 0;
            int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
            string keywords = txtKeyword.Text.Trim();
            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Wcmm_Search(cid, keywords, Get_ListOptions(), out numResults);
            return list;
        }
        private List<CategoryInfo> Load_List(out int numResults)
        {
            numResults = 0;
            int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
            int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Wcmm_Getlist(cid, pid, Get_ListOptions(), out numResults);
            return list;
        }
        public override void Bind_grdView()
        {
            try
            {
                int numResults = 0;
                List<CategoryInfo> list = CFunctions.IsNullOrEmpty(txtKeyword.Text.Trim()) ? this.Load_List(out numResults) : this.Search(out numResults);
                (new GenericList<CategoryInfo>(PageIndex, PageSize, SortExp, SortDir)).Bind_GridView(grdView, pagBuilder, list, numResults);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
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
        private void BindDDL_Pid(DropDownList ddl, int cid, int pid, string separator)
        {
            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Wcmm_Getlist(cid, pid, Get_ListOptionsNoPaging());
            if (list == null) return;

            foreach (CategoryInfo info in list)
            {
                string sep = pid == 0 ? "" : separator + "---";
                ListItem item = new ListItem(sep + info.Name, info.Id.ToString());
                ddl.Items.Add(item);
                this.BindDDL_Pid(ddl, cid, info.Id, sep);
            }
        }

        private void Copy_Info(CategoryInfo source_info, int dest_cid, int dest_pid, int dest_depth, bool getchild, CCategory BLL)
        {
            if (source_info == null) return;

            CategoryInfo info_copy = source_info.copy();
            info_copy.Id = 0;
            info_copy.Cid = dest_cid;
            info_copy.Pid = dest_pid;
            info_copy.Depth = dest_depth + 1;
            info_copy.Status = CCommon.GetStatus_upt();
            info_copy.Username = CCommon.Get_CurrentUsername();
            info_copy.Timeupdate = DateTime.Now;
            info_copy.Orderd = 0;
            info_copy.Pis = getchild ? info_copy.Pis : (info_copy.Pis > 0 ? 1 : 0);
            if (BLL.Save(info_copy))
            {
                //BLL.Updatenum(dest_pid.ToString(), Queryparam.Sqlcolumn.Pis, 1);
                if (getchild)
                {
                    List<CategoryInfo> listsub = BLL.Wcmm_Getlist(source_info.Cid, source_info.Id, Get_ListOptionsNoPaging());
                    if (listsub == null || listsub.Count == 0) return;
                    foreach (CategoryInfo info_sub in listsub)
                    {
                        this.Copy_Info(info_sub, dest_cid, info_copy.Id, info_copy.Depth, getchild, BLL);
                    }
                }
            }
        }
        private void Move_Info(CategoryInfo source_info, int dest_cid, int dest_pid, int dest_depth, bool getchild, CCategory BLL)
        {
            if (source_info == null) return;
            int offset_depth = source_info.Depth - (dest_depth + 1);

            CategoryInfo info_copy = source_info.copy();
            info_copy.Cid = dest_cid;
            info_copy.Pid = dest_pid;
            info_copy.Depth = dest_depth + 1;
            info_copy.Status = CCommon.GetStatus_upt();
            info_copy.Username = CCommon.Get_CurrentUsername();
            info_copy.Timeupdate = DateTime.Now;
            if (BLL.Save(info_copy))
            {
                //BLL.Updatenum(dest_pid.ToString(), Queryparam.Sqlcolumn.Pis, 1);

                List<CategoryInfo> listin = null;
                List<CategoryInfo> listsub = BLL.Wcmm_Getlist_sub(source_info.Id, listin);
                if (listsub != null && listsub.Count > 0)
                {
                    foreach (CategoryInfo info_sub in listsub)
                    {
                        info_sub.Cid = getchild ? dest_cid : source_info.Cid;
                        info_sub.Pid = getchild ? info_sub.Pid : (info_sub.Pid == source_info.Id ? source_info.Pid : info_sub.Pid);
                        info_sub.Depth = getchild ? info_sub.Depth + offset_depth : info_sub.Depth - 1;
                        info_sub.Status = CCommon.GetStatus_upt();
                        info_sub.Username = CCommon.Get_CurrentUsername();
                        info_sub.Timeupdate = DateTime.Now;
                        BLL.Save(info_sub);
                    }
                }
            }
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

                string vlreturn = Delete(Webcmm.Id.Category, iid);
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
                        CategoryInfo info = (new CCategory(CCommon.LANG)).Wcmm_Getinfo(iid);
                        if (info != null)
                            Setupattribute.Show_Dialog(Webcmm.Id.Category, iid.ToString(), info.Iconex, info.Status, info.Markas);
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
        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string iid = CCommon.Get_FormString(Queryparam.Chkcheck);
                if (CFunctions.IsNullOrEmpty(iid)) return;

                string vlreturn = Delete(Webcmm.Id.Category, iid);
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
        protected void cmdCopy_Click(object sender, EventArgs e)
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

                List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Wcmm_Getlist(iid);
                if (list == null) return;

                grdCopy.DataSource = list;
                grdCopy.DataBind();
                pnlList.Visible = false;
                pnlCopy.Visible = !pnlList.Visible;
                if (ddlCopycid.Items.Count == 0)
                    this.BindDDL_Cid(ddlCopycid);

                string iidstr = "";
                foreach (CategoryInfo info in list)
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
                int dest_pid = int.Parse(ddlCopypid.SelectedValue);
                bool getchild = chkCopyoption_getchild.Checked;

                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(iidstr) || dest_cid == 0)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Copy_error, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                CCategory BLL = new CCategory(CCommon.LANG);
                CategoryInfo parent_info = BLL.Wcmm_Getinfo(dest_pid);
                int dest_depth = parent_info == null ? 1 : parent_info.Depth;
                string[] iidarr = iidstr.Split(',');
                bool isDup = false;
                for (int i = 0; i < iidarr.Length; i++)
                {
                    CategoryInfo info = BLL.Wcmm_Getinfo(int.Parse(iidarr[i]));
                    if (info.Id != dest_pid)
                        this.Copy_Info(info, dest_cid, dest_pid, dest_depth, getchild, BLL);
                    else
                        isDup = true;
                }
                if (parent_info != null)
                {
                    int pis = parent_info.Pis == 0 ? iidarr.Length + 1 : iidarr.Length + parent_info.Pis;
                    if (isDup) pis--;
                    BLL.Updatenum(dest_pid.ToString(), Queryparam.Sqlcolumn.Pis, pis);
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
        protected void cmdMove_Click(object sender, EventArgs e)
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

                List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Wcmm_Getlist(iid);
                if (list == null) return;

                grdMove.DataSource = list;
                grdMove.DataBind();
                pnlList.Visible = false;
                pnlMove.Visible = !pnlList.Visible;
                if (ddlMovecid.Items.Count == 0) 
                    this.BindDDL_Cid(ddlMovecid);

                string iidstr = "";
                foreach (CategoryInfo info in list)
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
                int dest_pid = int.Parse(ddlMovepid.SelectedValue);
                bool getchild = chkMoveoption_getchild.Checked;

                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(iidstr) || dest_cid == 0)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Move_error, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                CCategory BLL = new CCategory(CCommon.LANG);
                CategoryInfo parent_info = BLL.Wcmm_Getinfo(dest_pid);
                int dest_depth = parent_info == null ? 1 : parent_info.Depth;
                string[] iidarr = iidstr.Split(',');
                bool isDup = false;
                for (int i = 0; i < iidarr.Length; i++)
                {
                    CategoryInfo info = BLL.Wcmm_Getinfo(int.Parse(iidarr[i]));
                    if (info.Id != dest_pid)
                    {
                        if (info.Pid != 0)
                            BLL.Updatenum(info.Pid.ToString(), Queryparam.Sqlcolumn.Pis, CConstants.NUM_DECREASE);
                        this.Move_Info(info, dest_cid, dest_pid, dest_depth, getchild, BLL);
                    }
                    else
                        isDup = true;
                }
                if (parent_info != null)
                {
                    int pis = parent_info.Pis == 0 ? iidarr.Length + 1 : iidarr.Length + parent_info.Pis;
                    if (isDup) pis--;
                    BLL.Updatenum(dest_pid.ToString(), Queryparam.Sqlcolumn.Pis, pis);
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
        protected void cmdUpdateorder_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdView.Rows.Count == 0) return;

                CCategory BLL = new CCategory(CCommon.LANG);
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
                    CategoryInfo info = (new CCategory(CCommon.LANG)).Wcmm_Getinfo(int.Parse(iid));
                    if (info != null)
                        Setupattribute.Show_Dialog(Webcmm.Id.Category, iid, info.Iconex, info.Status, info.Markas);
                }
                else
                    Setupattribute.Show_Dialog(Webcmm.Id.Category, iid, Queryparam.Defstring.Nospecifystr, Queryparam.Defstring.Nospecifyint, Queryparam.Defstring.Nospecifyint);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }

        protected void ddlCopycid_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int cid = int.Parse(ddlCopycid.SelectedValue);
                ddlCopypid.Items.Clear();
                cmdCopyOk.Enabled = cid != 0;
                if (cid == 0) return;

                this.BindDDL_Pid(ddlCopypid, cid, 0, "");
                CCommon.Insert_FirstitemDDL(ddlCopypid);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void ddlMovecid_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int cid = int.Parse(ddlMovecid.SelectedValue);
                ddlMovepid.Items.Clear();
                cmdMoveOk.Enabled = cid != 0;
                if (cid == 0) return;

                this.BindDDL_Pid(ddlMovepid, cid, 0, "");
                CCommon.Insert_FirstitemDDL(ddlMovepid);
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
