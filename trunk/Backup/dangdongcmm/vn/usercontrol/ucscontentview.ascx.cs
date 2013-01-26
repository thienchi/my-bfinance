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
    public partial class ucscontentview : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Load_Info();
            }
        }

        public string Code
        {
            get;
            set;
        }

        private void Load_Info()
        {
            try
            {
                StaticcontentInfo info = new CStaticcontent(CCommon.LANG).Getinfo(Code);
                ltrInfo.Text = info == null ? "" : info.Description;
                return;
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
    }
}