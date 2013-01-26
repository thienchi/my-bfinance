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
    public partial class feedbackl : BasePage
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
        private FeedbackInfo PARENT = null;

        #region private methods
        private FeedbackInfo Get_Parent()
        {
            int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
            FeedbackInfo parent = (new CFeedback()).Wcmm_Getinfo(pid);
            return parent;
        }
        private string Generate_Parentpath(int pid)
        {
            string vlreturn = "";
            List<FeedbackInfo> list_parent = (new CFeedback()).Getlist_parent(pid);
            if (list_parent != null)
            {
                for (int i = 0; i < list_parent.Count; i++)
                {
                    FeedbackInfo info = list_parent[i];
                    vlreturn += " >> <a href='feedbackl.aspx?pid=" + info.Id + "'>" + info.Name + "</a>";
                }
            }
            return vlreturn;
        }
        private void Init_State()
        {
            int pid = PARENT == null ? 0 : PARENT.Id;
            string parentpath = this.Generate_Parentpath(pid);
            if (!CFunctions.IsNullOrEmpty(parentpath))
            {
                string notice = CCommon.Get_Definephrase(Definephrase.Display_havesub_feedback);
                notice = notice.Replace(Queryparam.Varstring.Depth, (PARENT == null ? 1 : PARENT.Depth + 1).ToString());
                notice = notice.Replace(Queryparam.Varstring.Path, parentpath);
                lblError.Text = notice;
            }
        }

        private List<FeedbackInfo> Search(out int numResults)
        {
            numResults = 0;
            string keywords = txtKeyword.Text.Trim();
            List<FeedbackInfo> list = (new CFeedback()).Wcmm_Search(keywords, Get_ListOptions(), out numResults);
            return list;
        }
        private List<FeedbackInfo> Load_List(out int numResults)
        {
            numResults = 0;
            int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
            List<FeedbackInfo> list = (new CFeedback()).Wcmm_Getlist(pid, Get_ListOptions(), out numResults);

            List<FeedbackInfo> listfolder = (new CFeedback()).Wcmm_Getlistfolder(pid);
            if (listfolder != null && listfolder.Count > 0)
            {
                if (list == null)
                    list = new List<FeedbackInfo>();
                list.InsertRange(0, listfolder);
            }
            return list;
        }
        public override void Bind_grdView()
        {
            int numResults = 0;
            List<FeedbackInfo> list = CFunctions.IsNullOrEmpty(txtKeyword.Text.Trim()) ? this.Load_List(out numResults) : this.Search(out numResults);
            (new GenericList<FeedbackInfo>(PageIndex, PageSize, SortExp, SortDir)).Bind_GridView(grdView, pagBuilder, list, numResults);
            return;
        }

        private void BindDDL_Pid(DropDownList ddl, int pid, string separator)
        {
            List<FeedbackInfo> list = (new CFeedback()).Wcmm_Getlistfolder(pid);
            if (list == null) return;

            foreach (FeedbackInfo info in list)
            {
                string sep = pid == 0 ? "" : separator + "---";
                ListItem item = new ListItem(sep + info.Name, info.Id.ToString());
                ddl.Items.Add(item);
                this.BindDDL_Pid(ddl, info.Id, sep);
            }
        }

        private void Move_Info(FeedbackInfo source_info, int dest_pid, int dest_depth, bool getchild, CFeedback BLL)
        {
            if (source_info == null) return;
            int offset_depth = source_info.Depth - (dest_depth + 1);

            FeedbackInfo info_copy = source_info.copy();
            info_copy.Pid = dest_pid;
            info_copy.Depth = dest_depth + 1;
            info_copy.Status = CCommon.GetStatus_upt();
            info_copy.Timeupdate = DateTime.Now;
            if (BLL.Save(info_copy))
            {
                List<FeedbackInfo> listin = null;
                List<FeedbackInfo> listsub = BLL.Getlist_sub(source_info.Id, listin);
                if (listsub != null && listsub.Count > 0)
                {
                    foreach (FeedbackInfo info_sub in listsub)
                    {
                        info_sub.Pid = getchild ? info_sub.Pid : (info_sub.Pid == source_info.Id ? source_info.Pid : info_sub.Pid);
                        info_sub.Depth = getchild ? info_sub.Depth + offset_depth : info_sub.Depth - 1;
                        info_sub.Status = CCommon.GetStatus_upt();
                        info_sub.Timeupdate = DateTime.Now;
                        BLL.Save(info_sub);
                    }
                }
            }
        }
        private string Delete(string iid)
        {
            try
            {
                if (!CCommon.Right_del(CFunctions.Get_Definecatrelate(Webcmm.Id.Feedback, Queryparam.Defstring.Page)))
                    return Definephrase.Invalid_right;

                CFeedback DAL = new CFeedback();
                if (DAL.Delete(iid))
                {
                    int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
                    if (pid != 0)
                        DAL.Updatepis(pid.ToString(), CConstants.NUM_DECREASE, iid.Split(',').Length);
                    return Definephrase.Remove_completed;
                }
                else
                    return Definephrase.Remove_error;
            }
            catch (Exception ex)
            {
                throw ex;
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

                string vlreturn = this.Delete(iid);
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
                    case "Viewinfo":
                        (new CFeedback()).Updatenum(iid.ToString(), Queryparam.Sqlcolumn.Viewcounter, CConstants.NUM_INCREASE);
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

                string vlreturn = this.Delete(iid);
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

                List<FeedbackInfo> list = (new CFeedback()).Wcmm_Getlist(iid);
                if (list == null) return;

                grdMove.DataSource = list;
                grdMove.DataBind();
                pnlList.Visible = false;
                pnlMove.Visible = !pnlList.Visible;
                this.BindDDL_Pid(ddlMovepid, 0, "");
                CCommon.Insert_FirstitemDDL(ddlMovepid);

                string iidstr = "";
                foreach (FeedbackInfo info in list)
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
        protected void cmdMoveCancel_Click(object sender, EventArgs e)
        {
            try
            {
                pnlList.Visible = true;
                pnlMove.Visible = !pnlList.Visible;
                Master.Form_ClearError();
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
                int dest_pid = int.Parse(ddlMovepid.SelectedValue);
                bool getchild = chkMoveoption_getchild.Checked;

                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(iidstr))
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Move_error, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                CFeedback BLL = new CFeedback();
                FeedbackInfo parent_info = BLL.Wcmm_Getinfo(dest_pid);
                int dest_depth = parent_info == null ? 1 : parent_info.Depth;
                string[] iidarr = iidstr.Split(',');
                bool isDup = false;
                for (int i = 0; i < iidarr.Length; i++)
                {
                    FeedbackInfo info = BLL.Wcmm_Getinfo(int.Parse(iidarr[i]));
                    if (info.Id != dest_pid)
                    {
                        if (info.Pid != 0)
                            BLL.Updatenum(info.Pid.ToString(), Queryparam.Sqlcolumn.Pis, CConstants.NUM_DECREASE);
                        this.Move_Info(info, dest_pid, dest_depth, getchild, BLL);
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

        protected void cmdFolder_Click(object sender, EventArgs e)
        {
            try
            {
                cmdCreateFolder.Visible = txtFolderName.Visible = true;
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdCreateFolder_Click(object sender, EventArgs e)
        {
            try
            {
                FeedbackInfo info = new FeedbackInfo();
                info.Sender_Name = info.Sender_Email = info.Sender_Address = info.Sender_Phone = info.Description = "";
                info.Name = txtFolderName.Text.Trim();
                info.Timeupdate = DateTime.Now;
                info.Pis = 1;
                if ((new CFeedback()).Save(info))
                {
                    cmdCreateFolder.Visible = txtFolderName.Visible = false;

                    txtKeyword.Text = "";
                    this.Bind_grdView();
                }
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
                    case Commandparam.Move:
                        this.cmdMove_Click(null, null);
                        break;
                    case Commandparam.Updatedisplayorder:
                        this.cmdFolder_Click(null, null);
                        break;
                }
                ddlAction.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }

        protected void ddlMovepid_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int pid = int.Parse(ddlMovepid.SelectedValue);
                cmdMoveOk.Enabled = pid != 0;
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        #endregion
    }
}
