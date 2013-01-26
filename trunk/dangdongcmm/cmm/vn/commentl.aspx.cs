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
using System.IO;
using OfficeOpenXml;

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm.cmm
{
    public partial class commentl : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Init_State();
                this.Bind_grdView();
            }
        }

        #region properties
        public int BELONGTO
        {
            get
            {
                return ViewState["BELONGTO"] == null ? 0 : int.Parse(ViewState["BELONGTO"].ToString());
            }
            set
            {
                ViewState["BELONGTO"] = value;
            }
        }
        public int CID
        {
            get
            {
                return ViewState["CID"] == null ? 0 : int.Parse(ViewState["CID"].ToString());
            }
            set
            {
                ViewState["CID"] = value;
            }
        }
        public int PID
        {
            get
            {
                return ViewState["PID"] == null ? 0 : int.Parse(ViewState["PID"].ToString());
            }
            set
            {
                ViewState["PID"] = value;
            }
        }
        #endregion

        #region private methods
        private void Init_State()
        {
            int belongto = CCommon.Get_QueryNumber(Queryparam.Belongto);
            int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
            int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
            this.BELONGTO = belongto;
            this.CID = cid;
            this.PID = pid;

            if (belongto != 0)
            {
                this.BindDDL_Cid(ddlCid, belongto);
                if (ddlCid.Items.Count > 0)
                {
                    ddlCid.SelectedValue = ddlCid.Items.FindByValue(cid.ToString()) == null ? "0" : cid.ToString();
                    
                    this.ddlCid_SelectedIndexChanged(null, null);
                    if (ddlName.Items.Count > 0)
                    {
                        ddlName.SelectedValue = ddlName.Items.FindByValue(pid.ToString()) == null ? "0" : pid.ToString();
                        if (pid != 0)
                            this.ddlName_SelectedIndexChanged(null, null);
                    }
                }
            }

        }
        private void BindCMD_New(int belongto, int cid, int pid)
        {
            cmdAdd.Attributes.Add("onclick", "CC_gotoUrl('commentu.aspx?blto=" + belongto + "&cid=" + cid + "&pid=" + pid + "')");
        }

        private void BindDDL_Cid(DropDownList ddl, int belongto)
        {
            BindDDL_Cid(ddl, belongto, 0, "");
            CCommon.Insert_FirstitemDDL(ddl);
        }
        private void BindDDL_Name(int belongto, DropDownList ddl, int cid, string separator)
        {
            List<GeneralInfo> list = (new CGeneral(CCommon.LANG, belongto)).Wcmm_Getlist_cid(cid, Get_ListOptionsNoPaging());
            if (list == null) return;

            foreach (GeneralInfo info in list)
            {
                string sep = "";
                ListItem item = new ListItem(sep + info.Name, info.Id.ToString());
                ddl.Items.Add(item);
            }
        }

        private List<CommentInfo> Search(out int numResults)
        {
            numResults = 0;
            string keywords = txtKeyword.Text.Trim();
            List<CommentInfo> list = (new CComment(CCommon.LANG)).Wcmm_Search(this.BELONGTO, this.PID, keywords, Get_ListOptions(), out numResults);
            return list;
        }
        private List<CommentInfo> Load_List(out int numResults)
        {
            numResults = 0;
            List<CommentInfo> list = (new CComment(CCommon.LANG)).Wcmm_Getlist(this.BELONGTO, this.PID, Get_ListOptions(), out numResults);
            return list;
        }
        public override void Bind_grdView()
        {
            this.BindCMD_New(BELONGTO, CID, PID);

            int numResults = 0;
            List<CommentInfo> list = CFunctions.IsNullOrEmpty(txtKeyword.Text.Trim()) ? this.Load_List(out numResults) : this.Search(out numResults);
            (new GenericList<CommentInfo>(PageIndex, PageSize, SortExp, SortDir)).Bind_GridView(grdView, pagBuilder, list, numResults);
            return;
        }
        #endregion

        #region events
        protected void ddlCid_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int belongto = this.BELONGTO;
                int cid = int.Parse(ddlCid.SelectedValue);
                ddlName.Items.Clear();
                this.CID = cid;
                this.PID = 0;
                this.Bind_grdView();
                if (belongto == 0) return;

                this.BindDDL_Name(belongto, ddlName, cid, "");
                CCommon.Insert_FirstitemDDL(ddlName);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void ddlName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int belongto = this.BELONGTO;
                int pid = int.Parse(ddlName.SelectedValue);
                //if (pid == 0) pid = -1;
                this.PID = pid;
                
                this.Bind_grdView();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }

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
                List<CommentInfo> list = (new CComment(CCommon.LANG)).Wcmm_Getlist(iid);

                string vlreturn = Delete(Webcmm.Id.Comment, iid);
                lstError = new List<Errorobject>();
                lstError = Form_GetError(lstError, vlreturn == Definephrase.Remove_completed ? Errortype.Completed : Errortype.Error, vlreturn, "", null);
                Master.Form_ShowError(lstError);
                if (vlreturn == Definephrase.Remove_completed)
                {
                    //List<CommentInfo> list = (new CComment(CCommon.LANG)).Wcmm_Getlist(iid);
                    if (list != null && list.Count > 0)
                    {
                        foreach (CommentInfo info in list)
                        {
                            (new CGeneral(CCommon.LANG, info.Belongto)).Updatenum(info.Iid.ToString(), Queryparam.Sqlcolumn.Allowcomment, CConstants.NUM_DECREASE);
                        }
                    }
                }
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
                        CommentInfo info = (new CComment(CCommon.LANG)).Wcmm_Getinfo(iid);
                        if (info != null)
                            Setupattribute.Show_Dialog(Webcmm.Id.Comment, iid.ToString(), info.Iconex, info.Status, info.Markas);
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
                List<CommentInfo> list = (new CComment(CCommon.LANG)).Wcmm_Getlist(iid);

                string vlreturn = Delete(Webcmm.Id.Comment, iid);
                lstError = new List<Errorobject>();
                lstError = Form_GetError(lstError, vlreturn == Definephrase.Remove_completed ? Errortype.Completed : Errortype.Error, vlreturn, "", null);
                Master.Form_ShowError(lstError);
                if (vlreturn == Definephrase.Remove_completed)
                {
                    //List<CommentInfo> list = (new CComment(CCommon.LANG)).Wcmm_Getlist(iid);
                    if (list != null && list.Count > 0)
                    {
                        foreach (CommentInfo info in list)
                        {
                            (new CGeneral(CCommon.LANG, info.Belongto)).Updatenum(info.Iid.ToString(), Queryparam.Sqlcolumn.Allowcomment, CConstants.NUM_DECREASE);
                        }
                    }
                }
                this.Bind_grdView();
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

                CComment BLL = new CComment(CCommon.LANG);
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
                    CommentInfo info = (new CComment(CCommon.LANG)).Wcmm_Getinfo(int.Parse(iid));
                    if (info != null)
                        Setupattribute.Show_Dialog(Webcmm.Id.Comment, iid, info.Iconex, info.Status, info.Markas);
                }
                else
                    Setupattribute.Show_Dialog(Webcmm.Id.Comment, iid, Queryparam.Defstring.Nospecifystr, Queryparam.Defstring.Nospecifyint, Queryparam.Defstring.Nospecifyint);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdExportexcel_Click(object sender, EventArgs e)
        {
            try
            {
                int numResults = 0;
                List<CommentInfo> list = null;
                if (CFunctions.IsNullOrEmpty(txtKeyword.Text.Trim()))
                {
                    list = (new CComment(CCommon.LANG)).Wcmm_Getlist(this.BELONGTO, this.PID, Get_ListOptionsNoPaging(), out numResults);
                }
                else
                {
                    list = (new CComment(CCommon.LANG)).Wcmm_Search(this.BELONGTO, this.PID, txtKeyword.Text.Trim(), Get_ListOptionsNoPaging(), out numResults);
                }

                if (list != null && list.Count > 0)
                {
                    FileInfo exTemplate = new FileInfo(Server.MapPath(@"~/xhtml/" + CCommon.LANG + "/ReportComment.xlsx"));
                    string filePath = @"~/commup/manual/ReportComment_" + DateTime.Now.Ticks.ToString() + ".xlsx";
                    FileInfo exNewfile = new FileInfo(Server.MapPath(filePath));
                    using (ExcelPackage exPackage = new ExcelPackage(exNewfile, exTemplate))
                    {
                        ExcelWorksheet exWorksheet = exPackage.Workbook.Worksheets[1];

                        for (int i = 0; i < list.Count; i++)
                        {
                            CommentInfo info = (CommentInfo)list[i];
                            exWorksheet.Cell(i + 2, 1).Value = (i + 1).ToString();
                            exWorksheet.Cell(i + 2, 2).Value = info.Sender_Name;
                            exWorksheet.Cell(i + 2, 3).Value = info.Sender_Address;
                            exWorksheet.Cell(i + 2, 4).Value = info.Sender_Email;
                            exWorksheet.Cell(i + 2, 5).Value = info.Sender_Phone;
                            exWorksheet.Cell(i + 2, 6).Value = info.Rating.ToString();
                            exWorksheet.Cell(i + 2, 7).Value = info.Description;
                        }

                        exPackage.Workbook.Properties.Title = "Report Comment";
                        exPackage.Save();
                    }

                    lnkExport.Visible = true;
                    lnkExport.Text = "Download";
                    lnkExport.NavigateUrl = filePath;
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
                    case Commandparam.Updatedisplayorder:
                        this.cmdUpdateorder_Click(null, null);
                        break;
                    case Commandparam.Setupattribute:
                        this.cmdSetupattribute_Click(null, null);
                        break;
                    case Commandparam.Exportexcel:
                        this.cmdExportexcel_Click(null, null);
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
