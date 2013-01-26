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

namespace dangdongcmm.cmm
{
    public partial class ucrelatedlibraries : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindDDL_Cid();
            }
        }

        #region private methods
        private void BindDDL_Cid()
        {
            BindDDL_Cid_DDL(RL_ddlCid, Webcmm.Id.Libraries, 0, "");
            CCommon.Insert_FirstitemDDL(RL_ddlCid);
        }
        #endregion

        #region events
        public string Get()
        {
            return RL_txtRelatedid.Value;
        }
        public void Set(string relateditem)
        {
            try
            {
                string vlreturn = "<table id=\"RL_listselectedLibraries\" class=\"RL_listselectedLibraries\">";
                List<LibrariesInfo> list = new List<LibrariesInfo>();
                if (!CFunctions.IsNullOrEmpty(relateditem))
                {
                    list = new CLibraries(CCommon.LANG).Wcmm_Getlist(relateditem);
                    if (list != null && list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            LibrariesInfo info = (LibrariesInfo)list[i];
                            vlreturn += "<tr id=\"rlrowi" + i + "\" class=\"rlrowi\">"
                                + "<td class=\"rlnote\">" + info.Name + "</td>"
                                + "<td class=\"rlsort\">" + info.Cname + "</td>"
                                + "<td class=\"rlcomd\"><a href=\"javascript:RLremove(" + info.Id + "," + i + ");\">x</a></td></tr>";
                        }
                    }
                }
                RL_containerLibraries.InnerHtml = vlreturn + "</table>";
                RL_containerLibraries.Attributes.Add("title", list == null ? "0" : list.Count.ToString());
                RL_txtRelatedid.Value = relateditem;
                return;
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        #endregion
    }
}