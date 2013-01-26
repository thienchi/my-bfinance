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
    public partial class ucnewsrelated : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Bind_dtlList();
            }
        }

        public string Relateditem
        {
            get;
            set;
        }

        #region private methods
        private void Bind_dtlList()
        {
            List<NewsInfo> list = new CNews(CCommon.LANG).Getlistrelated(Relateditem);
            (new GenericList<NewsInfo>()).Bind_DataList(rptList, null, list, 0);
            pnlList.Visible = list != null && list.Count > 0;
            return;
        }
        #endregion

    }
}