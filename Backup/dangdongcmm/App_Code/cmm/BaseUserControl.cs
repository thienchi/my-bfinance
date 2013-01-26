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
    public class BaseUserControl : System.Web.UI.UserControl
    {
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
                ViewState["PageIndex"] = value;
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

        public void BindDDL_Cid_DDL(DropDownList ddl, int cid, int pid, string separator)
        {
            CCommon.BindDDL_Cid_DDL(Get_ListOptionsNoPaging(), ddl, cid, pid, separator);
        }
        
    }
}
