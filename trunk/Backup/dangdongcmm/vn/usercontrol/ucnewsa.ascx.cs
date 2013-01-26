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
    public partial class ucnewsa : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Bind_rptList(this.Categoryid, this.Aid, this.Acode);
            }
        }


        public string Categoryid
        {
            get;
            set;
        }
        public string Aid
        {
            get;
            set;
        }
        public string Acode
        {
            get;
            set;
        }

        #region public methods
        public void Bind_rptList(string categoryid, string aid, string acode)
        {
            int numResults = 0;
            ListOptions options = Get_ListOptions(pagBuilder);
            List<NewsInfo> list = (new CNews(CCommon.LANG)).Getlistattr(categoryid, aid, acode, options, out numResults);
            (new GenericList<NewsInfo>()).Bind_DataList(rptList, pagBuilder, list, 0);
            pnlList.Visible = numResults > 0;
            return;
        }
        #endregion
    }
}