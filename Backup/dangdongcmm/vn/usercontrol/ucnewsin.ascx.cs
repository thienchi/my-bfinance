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
    public partial class ucnewsin : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (this.Visible)
                {
                    this.Bind_dtlList(this.Cid);
                }
            }
        }

        #region properties
        public int Cid
        {
            get;
            set;
        }
        #endregion

        #region private methods
        private void Bind_dtlList(int cid)
        {
            int numResults = 0;
            ListOptions options = Get_ListOptions(pagBuilder);
            options.Markas = int.Parse(hidMarkas.Value);

            List<NewsInfo> list = (new CNews(CCommon.LANG)).Getlist(cid.ToString(), options, out numResults);
            (new GenericList<NewsInfo>()).Bind_DataList(rptList, pagBuilder, list, numResults);
            pnlList.Visible = numResults > 0;
            return;
        }
        #endregion
    }
}