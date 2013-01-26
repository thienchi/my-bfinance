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
    public partial class waittingupdate : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Init_State();
            }
        }
        
        #region private methods
        private void Init_State()
        {
            this.Bind_grdView();
        }

        private List<GeneralInfo> Load_List()
        {
            List<GeneralInfo> list = null;
            List<CategorytypeofInfo> listtypeof = (new CCategorytypeof(CCommon.LANG)).Wcmm_Getlist((int)CConstants.State.Status.None, (int)CConstants.State.MarkAs.Focus);
            if (listtypeof == null) return null;

            foreach (CategorytypeofInfo infotypeof in listtypeof)
            {
                string tablename = CFunctions.Get_Definecatrelate(infotypeof.Id, Queryparam.Defstring.Table);
                if (CFunctions.IsNullOrEmpty(tablename) || tablename == CCommon.LANG) continue;

                List<GeneralInfo> listin = (new CGeneral(CCommon.LANG, tablename)).Waitting_Getlist((int)CConstants.State.Status.Waitactive, infotypeof.Id, infotypeof.Name);
                if (listin == null) continue;

                if (list == null) 
                    list = new List<GeneralInfo>();
                list.AddRange(listin);
            }
            return list;
        }
        private List<GeneralInfo> Search()
        {
            string keywords = txtKeyword.Text.Trim();
            List<GeneralInfo> list = null;
            List<CategorytypeofInfo> listtypeof = (new CCategorytypeof(CCommon.LANG)).Wcmm_Getlist((int)CConstants.State.Status.None, (int)CConstants.State.MarkAs.Focus);
            foreach (CategorytypeofInfo infotypeof in listtypeof)
            {
                string tablename = CFunctions.Get_Definecatrelate(infotypeof.Id, Queryparam.Defstring.Table);
                if (CFunctions.IsNullOrEmpty(tablename) || tablename == CCommon.LANG) continue;

                List<GeneralInfo> listin = (new CGeneral(CCommon.LANG, tablename)).Waitting_Search(keywords, (int)CConstants.State.Status.Waitactive, infotypeof.Id, infotypeof.Name);
                if (listin == null) continue;

                if (list == null)
                    list = new List<GeneralInfo>();
                list.AddRange(listin);
            }
            return list;
        }
        public override void Bind_grdView()
        {
            List<GeneralInfo> list = CFunctions.IsNullOrEmpty(txtKeyword.Text.Trim()) ? this.Load_List() : this.Search();
            SortExp = "Timeupdate";
            (new GenericList<GeneralInfo>(PageIndex, PageSize, SortExp, SortDir)).Bind_GridView(grdView, pagBuilder, list);
            return;
        }

        private string Get_pageu(int belongto)
        {
            string page = CFunctions.Get_Definecatrelate(belongto, Queryparam.Defstring.Page);
            string pageu = page.Replace(".aspx", "u.aspx");
            return pageu;
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
                    int iid = int.Parse(grdView.DataKeys[e.Row.RowIndex].Values["Id"].ToString());
                    int belongto = int.Parse(grdView.DataKeys[e.Row.RowIndex].Values["Belongto"].ToString());
                
                    LinkButton lnkEdit = (LinkButton)e.Row.FindControl("lnkEdit");
                    if (lnkEdit != null)
                    {
                        lnkEdit.OnClientClick = "javascript:CC_gotoUrl('" + this.Get_pageu(belongto) + "?iid=" + iid + "'); return false;";
                    }
                    LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                    if (lnkDelete != null)
                    {
                        lnkDelete.OnClientClick = "javascript:return doDelo('" + iid + "')";
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
                string iid = grdView.DataKeys[e.RowIndex].Values["Id"].ToString();
                int belongto = int.Parse(grdView.DataKeys[e.RowIndex].Values["Belongto"].ToString());
                if (CFunctions.IsNullOrEmpty(iid)) return;
                
                string vlreturn = Delete(belongto, iid);
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
        protected void grdView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                string iid = grdView.DataKeys[e.RowIndex].Values["Id"].ToString();
                int belongto = int.Parse(grdView.DataKeys[e.RowIndex].Values["Belongto"].ToString());
                if (CFunctions.IsNullOrEmpty(iid)) return;

                string vlreturn = Updatenum(belongto, iid, Queryparam.Sqlcolumn.Status, (int)CConstants.State.Status.Actived);
                lstError = new List<Errorobject>();
                lstError = Form_GetError(lstError, vlreturn == Definephrase.Save_completed ? Errortype.Completed : Errortype.Error, vlreturn, "", null);
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
        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string querystring = CCommon.Get_FormString(Queryparam.Chkcheck);
                if (CFunctions.IsNullOrEmpty(querystring)) return;

                string vlreturn = "";
                string[] items = querystring.Split(',');
                foreach (string iid_belongto in items)
                {
                    string[] arr = iid_belongto.Split(':');
                    vlreturn = Delete(int.Parse(arr[1]), arr[0]);
                }

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
        protected void cmdAccept_Click(object sender, EventArgs e)
        {
            try
            {
                string querystring = CCommon.Get_FormString(Queryparam.Chkcheck);
                if (CFunctions.IsNullOrEmpty(querystring)) return;

                string vlreturn = "";
                string[] items = querystring.Split(',');
                foreach (string iid_belongto in items)
                {
                    string[] arr = iid_belongto.Split(':');
                    vlreturn = Updatenum(int.Parse(arr[1]), arr[0], Queryparam.Sqlcolumn.Status, (int)CConstants.State.Status.Actived);
                }

                lstError = new List<Errorobject>();
                lstError = Form_GetError(lstError, vlreturn == Definephrase.Save_completed ? Errortype.Completed : Errortype.Error, vlreturn, "", null);
                Master.Form_ShowError(lstError);
                this.Bind_grdView();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        #endregion
    }
}
