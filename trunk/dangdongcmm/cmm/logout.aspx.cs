using System;
using System.Collections;
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

using dangdongcmm.dal;
using dangdongcmm.model;
using dangdongcmm.utilities;

namespace dangdongcmm.cmm
{
    public partial class logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Logout();
        }

        #region private methods
        private void Logout()
        {
            CCommon.Session_Set(Sessionparam.USERLOGIN, null);
            CCommon.Session_Set(Sessionparam.PREVIOUSURL, null);
            Response.Redirect("login.aspx?problem=logout");
        }
        #endregion
    }
}
