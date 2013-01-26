using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm
{
    public partial class uccataloguemenu : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (this.Visible)
                {
                    this.Bind_dtlList(this.Belongto, this.Cid);
                }
            }
        }

        #region properties
        public int Belongto
        {
            get;
            set;
        }
        public int Cid
        {
            get;
            set;
        }
        #endregion

        #region private methods
        private void Bind_dtlList(int belongto, int pid)
        {
            int numResults = 0;
            ListOptions options = Get_ListOptionsNoPaging();
            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Getlist(belongto, pid, options, out numResults);
            (new GenericList<CategoryInfo>(PageIndex, PageSize, SortExp, SortDir)).Bind_DataList(rptList, null, list, 0);
            pnlList.Visible = list != null && list.Count > 0;
            return;
        }
        #endregion
    }
}