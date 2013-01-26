using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;
using System.Drawing;

using dangdongcmm.dal;
using dangdongcmm.model;
using dangdongcmm.utilities;

namespace dangdongcmm.cmm
{
    public class BasePage : System.Web.UI.Page
    {
        public BasePage()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            CCommon.PreviousUrl = Request.Url.PathAndQuery;
            PageIndex = CCommon.Get_QueryNumber(Queryparam.Pageindex);
            if (ConfigurationSettings.AppSettings["WCMM"] != "free")
            {
                UserInfo info = CCommon.Get_CurrentUser();
                if (info == null)
                {
                    Response.Redirect("../login.aspx?problem=expired");
                    return;
                }
                else
                {
                    UserrightInfo rinfo = info.iRight;
                    if (CFunctions.IsNullOrEmpty(rinfo.R_sys))
                    {
                        string r_page = rinfo.R_new;
                        string page = Page.ToString().Replace("ASP.cmm_" + CCommon.LANG + "_", "");
                        if (page != "dashboard_aspx" && page != "changepassword_aspx" && page != "albummanagement_aspx")
                        {
                            if (!this.CheckRPages(r_page, page))
                            {
                                Response.Redirect(CFunctions.IsNullOrEmpty(CCommon.PreviousUrl) ? "dashboard.aspx" : CCommon.PreviousUrl);
                                return;
                            }
                        }
                    }
                }
            }
        }
        private bool CheckRPages(string r_page, string page)
        {
            page = page.IndexOf("u_aspx") != -1 ? page.Replace("u_aspx", "") : page.Replace("l_aspx", "");
            if (r_page.IndexOf("#" + page) != -1)
            {
                int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
                if (r_page.IndexOf("#" + page + (cid == 0 ? "" : cid.ToString()) + "#") != -1)
                    return true;
            }
            return false;
        }
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "processing", "close_processing();", true);
            }
        }

        virtual public void Bind_grdView()
        {
        }
        virtual public void Bind_dtlList(int belongto, int iid)
        {
        }

        #region properties
        public int PageSize = CConstants.PAGESIZE;
        public int PageIndex
        {
            get
            {
                return ViewState["PageIndex"] == null ? 1 : int.Parse(ViewState["PageIndex"].ToString());
            }
            set
            {
                ViewState["PageIndex"] = value == 0 ? 1 : value;
            }
        }
        public string SortExp
        {
            get
            {
                return ViewState["SortExp"] == null ? "Id" : ViewState["SortExp"].ToString();
            }
            set
            {
                ViewState["SortExp"] = value;
            }
        }
        public SortDirection SortDir
        {
            get
            {
                if (ViewState["SortDir"] == null)
                    ViewState["SortDir"] = SortDirection.Descending;

                return (SortDirection)ViewState["SortDir"];
            }
            set
            {
                ViewState["SortDir"] = value;
            }
        }
        public List<Errorobject> lstError = null;
        
        public ListOptions Get_ListOptions()
        {
            ListOptions options = new ListOptions();
            options.PageIndex = PageIndex;
            options.PageSize = PageSize;
            options.SortExp = SortExp;
            options.SortDir = SortDir.ToString();

            UserInfo user = CCommon.Get_CurrentUser();
            if (user != null)
            {
                options.Username = user.Username;
                options.GetAll = CCommon.Right_sys();
            }
            return options;
        }
        public ListOptions Get_ListOptionsNoPaging()
        {
            ListOptions options = new ListOptions();
            options.SortExp = SortExp;
            options.SortDir = SortDir.ToString();

            UserInfo user = CCommon.Get_CurrentUser();
            if (user != null)
            {
                options.Username = user.Username;
                options.GetAll = CCommon.Right_sys();
            }
            return options;
        }
        #endregion

        public List<Errorobject> Form_GetError(List<Errorobject> lstError, string errortype, string tag, string message, object txt)
        {
            if (lstError == null)
                lstError = new List<Errorobject>();
            Errorobject error = new Errorobject(txt, CCommon.Get_Definephrase(tag) + message, errortype);
            lstError.Add(error);
            return lstError;
        }
        public bool Form_SaveOption(bool golist)
        {
            if (!golist) return false;

            string url_l = Page.Request.Url.PathAndQuery.Replace("u.aspx", "l.aspx");
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "gotoUrl", "CC_gotoUrlcountdown('" + url_l + "');", true);
            return true;
        }

        #region for u
        public void BindDDL_Cid(DropDownList ddl, int cid, int pid, string separator)
        {
            this.BindDDL_Cid_DDL(ddl, cid, pid, separator);

            UserInfo userinfo = CCommon.Get_CurrentUser();
            UserrightInfo rinfo = userinfo == null ? null : userinfo.iRight;
            bool r_sys = rinfo == null ? false : !CFunctions.IsNullOrEmpty(rinfo.R_sys);
            if (!r_sys)
            {
                string r_page = rinfo == null ? "" : rinfo.R_new;
                string page = CFunctions.Get_Definecatrelate(cid, Queryparam.Defstring.Page).Replace(".aspx", "");

                string list_parent = "#";
                for (int i = ddl.Items.Count - 1; i >= 0; i--)
                {
                    ListItem item = ddl.Items[i];

                    if (list_parent.IndexOf("#" + item.Value + "#") == -1)
                    {
                        if (r_page.IndexOf("#" + page + item.Value + "#") == -1)
                        {
                            ddl.Items.Remove(item);
                        }
                        else
                        {
                            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Getlist_parent(int.Parse(item.Value));
                            if (list != null && list.Count > 0)
                                foreach (CategoryInfo info in list)
                                    list_parent += info.Id + "#";
                        }
                    }
                }
            }
        }
        public void BindDDL_Cid_DDL(DropDownList ddl, int cid, int pid, string separator)
        {
            CCommon.BindDDL_Cid_DDL(Get_ListOptionsNoPaging(), ddl, cid, pid, separator);
        }
        public void BindDDL_Cid(ListBox lst, int cid, int pid, string separator)
        {
            this.BindDDL_Cid_LST(lst, cid, pid, separator);

            UserInfo userinfo = CCommon.Get_CurrentUser();
            UserrightInfo rinfo = userinfo == null ? null : userinfo.iRight;
            bool r_sys = rinfo == null ? false : !CFunctions.IsNullOrEmpty(rinfo.R_sys);
            if (!r_sys)
            {
                string r_page = rinfo == null ? "" : rinfo.R_new;
                string page = Page.ToString().Replace("ASP.cmm_" + CCommon.LANG + "_", "");
                page = page.IndexOf("u_aspx") != -1 ? page.Replace("u_aspx", "") : page.Replace("l_aspx", "");

                string list_parent = "#";
                for (int i = lst.Items.Count - 1; i >= 0; i--)
                {
                    ListItem item = lst.Items[i];

                    if (list_parent.IndexOf("#" + item.Value + "#") == -1)
                    {
                        if (r_page.IndexOf("#" + page + item.Value + "#") == -1)
                        {
                            lst.Items.Remove(item);
                        }
                        else
                        {
                            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Getlist_parent(int.Parse(item.Value));
                            if (list != null && list.Count > 0)
                                foreach (CategoryInfo info in list)
                                    list_parent += info.Id + "#";
                        }
                    }
                }
            }
        }
        public void BindDDL_Cid_LST(ListBox lst, int cid, int pid, string separator)
        {
            ListOptions options = Get_ListOptionsNoPaging();
            options.SortExp = Queryparam.Sqlcolumn.Orderd;
            options.GetAll = true;

            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Wcmm_Getlist(cid, pid, options);
            if (list == null) return;

            foreach (CategoryInfo info in list)
            {
                string sep = pid == 0 ? "" : separator + "---";
                ListItem item = new ListItem(sep + info.Name, info.Id.ToString());
                lst.Items.Add(item);
                this.BindDDL_Cid_LST(lst, cid, info.Id, sep);
            }
        }
        public void Insert_FirstitemLST(ListBox lst)
        {
            ListItem item = new ListItem(CCommon.Get_Definephrase(Definephrase.Firstitem_ddl), "0");
            item.Attributes.Add("class", "textdefndis");
            lst.Items.Insert(0, item);
        }
        #endregion

        #region for l
        public void BindData_Sorting(GridView gridView, GridViewRowEventArgs e)
        {
            if (SortExp.Length > 0)
            {
                int cellIndex = -1;
                foreach (DataControlField field in gridView.Columns)
                {
                    if (field.SortExpression == SortExp)
                    {
                        cellIndex = gridView.Columns.IndexOf(field);
                        break;
                    }
                }

                if (cellIndex > -1)
                {
                    if (e.Row.RowType == DataControlRowType.Header)
                    {
                        e.Row.Cells[cellIndex].CssClass = (SortDir.ToString() == "Ascending" ? "headerasc" : "headerdsc");
                    }
                    else if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        //e.Row.Cells[cellIndex].BackColor = System.Drawing.Color.FromArgb(230, 230, 230);
                        e.Row.Cells[cellIndex].Attributes.Add("style", "background-color:#e6e6e6;");
                        //e.Row.Cells[cellIndex].CssClass = "cellselect";
                    }
                }
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "lEffect_Over(this)");
                e.Row.Attributes.Add("onmouseout", "lEffect_Out(this)");
                e.Row.Attributes.Add("id", gridView.DataKeys[e.Row.RowIndex].Values["Id"].ToString());
            }
        }
        public void BuildCommand_Delete(GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton cmdDeleteone = (LinkButton)e.Row.FindControl("cmdDeleteone");
                if (cmdDeleteone != null)
                {
                    cmdDeleteone.OnClientClick = "javascript:return doDelo('" + (((HiddenField)e.Row.FindControl("txtId")).Value) + "')";
                }
            }
        }
        #endregion

        #region for generic
        public string RemoveFilepreview(int did, int iid)
        {
            try
            {
                if (!CCommon.Right_upt(CFunctions.Get_Definecatrelate(did, Queryparam.Defstring.Page)))
                    return Definephrase.Invalid_right;

                if ((new CGeneral(CCommon.LANG, did)).Updatestr(iid.ToString(), Queryparam.Sqlcolumn.Filepreview, ""))
                    return Definephrase.Removefilepreview_completed;
                else
                    return Definephrase.Removefilepreview_error;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Delete(int did, string iid)
        {
            try
            {
                if (!CCommon.Right_del(CFunctions.Get_Definecatrelate(did, Queryparam.Defstring.Page)))
                    return Definephrase.Invalid_right;

                CGeneral DAL = new CGeneral(CCommon.LANG, did);
                int status = CCommon.GetStatus_del();
                bool recursive = CFunctions.Get_Definecatrelate(did, Queryparam.Defstring.Recursive) == Queryparam.Defstring.Recursive;
                //if (DAL.Updatenum(iid, Queryparam.Sqlcolumn.Status, status))
                if (DAL.Delete(iid, recursive))
                {
                    if ((recursive) && (status == (int)CConstants.State.Status.Deleted))
                        DAL.Updatepis(iid, CConstants.NUM_DECREASE, iid.Split(',').Length);
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
        public string Updatenum(int did, string iid, string sqlcolumn, object value)
        {
            try
            {
                if (!CCommon.Right_upt(CFunctions.Get_Definecatrelate(did, Queryparam.Defstring.Page)))
                    return Definephrase.Invalid_right;

                CGeneral DAL = new CGeneral(CCommon.LANG, did);
                if (DAL.Updatenum(iid, sqlcolumn, value))
                {
                    return Definephrase.Save_completed;
                }
                else
                    return Definephrase.Save_error;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
