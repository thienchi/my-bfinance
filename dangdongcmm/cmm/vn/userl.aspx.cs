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
    public partial class userl : BasePage
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
        private UserInfo PARENT = null;

        #region private methods
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
                    vlreturn += " >> <a href='userl.aspx?pid=" + info.Id + "'>" + info.Name + "</a>";
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
        private void Init_State()
        {
            int pid = PARENT == null ? -1 : PARENT.Id;
            string parentpath = this.Generate_Parentpath(pid);
            if (!CFunctions.IsNullOrEmpty(parentpath))
            {
                string notice = CCommon.Get_Definephrase(Definephrase.Display_havesub);
                notice = notice.Replace(Queryparam.Varstring.Depth, (PARENT == null ? 1 : PARENT.Depth + 1).ToString());
                notice = notice.Replace(Queryparam.Varstring.Path, parentpath);
                lblError.Text = notice;
            }

            cmdAdd.Attributes.Add("onclick", "CC_gotoUrl('useru.aspx?pid=" + pid + "')");
            this.BindDDL_Group(ddlGroup, 0, "");
            if (ddlGroup.Items.FindByValue(pid.ToString()) != null)
                ddlGroup.SelectedValue = pid.ToString();
        }

        private List<UserInfo> Search(out int numResults)
        {
            numResults = 0;
            int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
            string keywords = txtKeyword.Text.Trim();
            List<UserInfo> list = (new CUser()).Wcmm_Search(pid, keywords, Get_ListOptions(), out numResults);
            return list;
        }
        private List<UserInfo> Load_List(out int numResults)
        {
            numResults = 0;
            int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
            List<UserInfo> list = (new CUser()).Wcmm_Getlist(pid, Get_ListOptions(), out numResults);
            return list;
        }
        public override void Bind_grdView()
        {
            int numResults = 0;
            List<UserInfo> list = CFunctions.IsNullOrEmpty(txtKeyword.Text.Trim()) ? this.Load_List(out numResults) : this.Search(out numResults);
            (new GenericList<UserInfo>(PageIndex, PageSize, SortExp, SortDir)).Bind_GridView(grdView, pagBuilder, list, numResults);
            return;
        }

        #endregion

        #region events
        protected void grdView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                GridView gridView = (GridView)sender;
                BindData_Sorting(gridView, e);

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    LinkButton cmdDeleteone = (LinkButton)e.Row.FindControl("cmdDeleteone");
                    HiddenField txtId = (HiddenField)e.Row.FindControl("txtId");
                    if (txtId.Value == "2")
                        cmdDeleteone.Visible = false;
                    else
                        if (cmdDeleteone != null)
                        {
                            cmdDeleteone.OnClientClick = "javascript:return doDelo('" + (txtId.Value) + "')";
                        }
                }
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

                string vlreturn = Delete(Webcmm.Id.User, iid);
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

                string vlreturn = Delete(Webcmm.Id.User, iid);
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

        protected void ddlAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (ddlAction.SelectedValue)
                {
                    case Commandparam.Delete:
                        this.cmdDelete_Click(null, null);
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
