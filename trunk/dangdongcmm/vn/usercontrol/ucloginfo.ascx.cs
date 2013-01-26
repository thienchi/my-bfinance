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
    public partial class ucloginfo : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                pnlLogin.Visible = CCommon.CurrentMember != null;
                pnlLoginempty.Visible = !pnlLogin.Visible;
            }
        }

        protected void lnkSignout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("home.aspx", false);
        }
    }
}