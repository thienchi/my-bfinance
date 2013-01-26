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
    public partial class ucnewsh : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Bind_rptList(this.Categoryid);
            }
        }

        public string Categoryid
        {
            get;
            set;
        }

        #region private methods
        private void Bind_rptList(string categoryid)
        {
            int numResults = 0;
            ListOptions options = Get_ListOptions(pagBuilder);
            options.Markas = (int)CConstants.State.MarkAs.OnHome;
            List<NewsInfo> list = (new CNews(CCommon.LANG)).Getlist(categoryid, options, out numResults);
            (new GenericList<NewsInfo>()).Bind_DataList(rptList, pagBuilder, list, 0);
            pnlList.Visible = numResults > 0;
            return;
        }
        #endregion


    }
}