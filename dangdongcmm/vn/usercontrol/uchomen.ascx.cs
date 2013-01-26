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
    public partial class uchomen : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Bind_rptList();
            }
        }

        #region private methods
        private void Bind_rptList()
        {
            int numResults = 0;
            ListOptions options = Get_ListOptionsNoPaging();
            options.Markas = (int)CConstants.State.MarkAs.OnHome;

            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Getlist(Webcmm.Id.News, -1, options, out numResults);
            (new GenericList<CategoryInfo>()).Bind_DataList(rptList, null, list, 0);
            pnlList.Visible = numResults > 0;

            return;
        }
        #endregion

        #region events

        #endregion
    }
}