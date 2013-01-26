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
using System.IO;

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm
{
    public class BasePage : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (this.IsRestrictedPages())
                this.CheckLogin();
            PageIndex = CCommon.Get_QueryNumber(Queryparam.Pageindex);
        }
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "processing", "close_processing();", true);
            }

            //MemberInfo info = Get_CurrentMember();
            //int logined = CConstants.CHECKLOGINFORDOWNLOAD == 1 && info == null ? 0 : 1;
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "WEBUSERLOGIN", "MB_WEBUSERLOGIN(" + logined + ");", true);
        }

        private bool IsRestrictedPages()
        {
            if (CFunctions.IsNullOrEmpty(Application["RestrictedPages"].ToString()))
            {
                string RestrictedPages = "#";
                List<Settingsite.RestrictedPages> list = (new CRestrictedPages()).Wcmm_Getlist();
                if (list != null && list.Count > 0)
                    foreach (Settingsite.RestrictedPages info in list)
                        RestrictedPages += info.Name + "#";
                Application["RestrictedPages"] = RestrictedPages == "#" ? "" : RestrictedPages;
            }

            if (!CFunctions.IsNullOrEmpty(Application["RestrictedPages"].ToString()))
            {
                string pagename = Request.Url.AbsolutePath.Replace("/" + CCommon.LANG + "/", "");
                string querystring = CFunctions.IsNullOrEmpty(Request.Url.Query) ? "" : "&" + Request.Url.Query.Remove(0, 1) + "&";
                string RestrictedPages = Application["RestrictedPages"].ToString();
                if (RestrictedPages.IndexOf("#" + pagename + "#") != -1)
                {
                    List<Settingsite.RestrictedPages> list = (new CRestrictedPages()).Getlist(pagename);
                    if (list != null && list.Count > 0)
                    {
                        foreach (Settingsite.RestrictedPages info in list)
                        {
                            if (!CFunctions.IsNullOrEmpty(info.Query))
                            {
                                string[] param = info.Query.Split('&');
                                int count = param.Length;
                                foreach (string par in param)
                                {
                                    if (querystring.IndexOf("&" + par + "&") != -1)
                                        count--;
                                    else break;
                                }
                                if (count == 0)
                                {
                                    CCommon.PreviousUrl = Request.Url.PathAndQuery;
                                    return true;
                                }
                            }
                            else
                            {
                                CCommon.PreviousUrl = Request.Url.PathAndQuery;
                                return true;
                            }

                        }
                    }
                }
            }
            return false;
        }
        private void CheckLogin()
        {
            if (CCommon.CurrentMember == null)
                Response.Redirect("login.aspx?problem=restricted");
        }

        #region properties
        public int PageSizeDef = CConstants.PAGESIZE;
        public int PageSize
        {
            get
            {
                return ViewState["PageSize"] == null ? CConstants.PAGESIZE : int.Parse(ViewState["PageSize"].ToString());
            }
            set
            {
                ViewState["PageSize"] = value;
            }
        }
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
                return ViewState["SortExp"] == null ? "Orderd" : ViewState["SortExp"].ToString();
            }
            set
            {
                ViewState["SortExp"] = value;
            }
        }
        public string SortDir
        {
            get
            {
                return ViewState["SortDir"] == null ? SortDirection.Descending.ToString() : ViewState["SortDir"].ToString();
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
            //PageSize = PageSizeDef;
            options.PageSize = PageSize;
            options.SortExp = SortExp;
            options.SortDir = SortDir.ToString();

            return options;
        }
        public ListOptions Get_ListOptions(ucpager pagBuilder)
        {
            ListOptions options = new ListOptions();
            options.PageIndex = pagBuilder.Visible ? PageIndex : 1;
            //PageSize = pagBuilder != null ? (pagBuilder.PageSize != 0 ? pagBuilder.PageSize : PageSizeDef) : PageSizeDef;
            options.PageSize = pagBuilder != null ? (pagBuilder.PageSize != 0 ? pagBuilder.PageSize : PageSize) : PageSize;
            options.SortExp = SortExp;
            options.SortDir = pagBuilder.SortDir;

            return options;
        }
        public ListOptions Get_ListOptionsNoPaging()
        {
            ListOptions options = new ListOptions();
            options.SortExp = SortExp;
            options.SortDir = SortDir.ToString();

            return options;
        }

        public MemberInfo CurrentMember
        {
            get
            {
                return CCommon.CurrentMember;
            }
        }
        #endregion
    }
}
