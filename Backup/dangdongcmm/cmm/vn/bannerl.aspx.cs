﻿using System;
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
    public partial class bannerl : BasePage
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
        private void Init_State()
        {
            cmdAdd.Attributes.Add("onclick", "CC_gotoUrl('banneru.aspx')");
        }

        private List<BannerInfo> Search(out int numResults)
        {
            numResults = 0;
            string keywords = txtKeyword.Text.Trim();
            List<BannerInfo> list = (new CBanner(CCommon.LANG)).Wcmm_Search(keywords, Get_ListOptions(), out numResults);
            return list;
        }
        private List<BannerInfo> Load_List(out int numResults)
        {
            numResults = 0;
            List<BannerInfo> list = (new CBanner(CCommon.LANG)).Wcmm_Getlist(Get_ListOptions(), out numResults);
            return list;
        }
        public override void Bind_grdView()
        {
            int numResults = 0;
            List<BannerInfo> list = CFunctions.IsNullOrEmpty(txtKeyword.Text.Trim()) ? this.Load_List(out numResults) : this.Search(out numResults);
            (new GenericList<BannerInfo>(PageIndex, PageSize, SortExp, SortDir)).Bind_GridView(grdView, pagBuilder, list, numResults);
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

                string vlreturn = Delete(Webcmm.Id.Banner, iid);
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
                        BannerInfo info = (new CBanner(CCommon.LANG)).Wcmm_Getinfo(iid);
                        if (info != null)
                            Setupattribute.Show_Dialog(Webcmm.Id.Banner, iid.ToString(), Queryparam.Defstring.Nosymbol, info.Status, Queryparam.Defstring.None);
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

                string vlreturn = Delete(Webcmm.Id.Symbol, iid);
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
        protected void cmdSetupattribute_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string iid = CCommon.Get_FormString(Queryparam.Chkcheck);
                if (CFunctions.IsNullOrEmpty(iid)) return;

                if (iid.IndexOf(',') == -1)
                {
                    BannerInfo info = (new CBanner(CCommon.LANG)).Wcmm_Getinfo(int.Parse(iid));
                    if (info != null)
                        Setupattribute.Show_Dialog(Webcmm.Id.Banner, iid, Queryparam.Defstring.Nosymbol, info.Status, Queryparam.Defstring.None);
                }
                else
                    Setupattribute.Show_Dialog(Webcmm.Id.Banner, iid, Queryparam.Defstring.Nosymbol, Queryparam.Defstring.Nospecifyint, Queryparam.Defstring.None);
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
