using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm
{
    public partial class scontentview : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string cv = CCommon.Get_QueryString("cv");
                this.Load_Info(cv);
            }
        }

        #region private methods
        
        private void Load_Info(string cv)
        {
            StaticcontentInfo info = (new CStaticcontent(CCommon.LANG)).Getinfo(cv);
            if (info != null)
            {
                List<StaticcontentInfo> list = new List<StaticcontentInfo>();
                list.Add(info);
                (new GenericList<StaticcontentInfo>()).Bind_DataList(rptInfo, null, list, 0);
            }
        }
        #endregion

    }
}
