using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security;
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
    public class BaseUserControl : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            PageIndex = CCommon.Get_QueryNumber(Queryparam.Pageindex);
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
