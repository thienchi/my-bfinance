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
    public partial class ucrelatednews : BaseUserControl
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
            BindDDL_Cid_DDL(RN_ddlCid, Webcmm.Id.News, 0, "");
            CCommon.Insert_FirstitemDDL(RN_ddlCid);
        }
        #endregion

        #region events
        public string Get()
        {
            return RN_txtRelatedid.Value;
        }
        public void Set(string relateditem)
        {
            try
            {
                string vlreturn = "<table id=\"RN_listselectedNews\" class=\"RN_listselectedNews\">";
                List<NewsInfo> list = new List<NewsInfo>();
                if (!CFunctions.IsNullOrEmpty(relateditem))
                {
                    list = new CNews(CCommon.LANG).Wcmm_Getlist(relateditem);
                    if (list != null && list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            NewsInfo info = (NewsInfo)list[i];
                            vlreturn += "<tr id=\"rnrowi" + i + "\" class=\"rnrowi\">"
                                + "<td class=\"rnnote\">" + info.Name + "</td>"
                                + "<td class=\"rnsort\">" + info.Cname + "</td>"
                                + "<td class=\"rncomd\"><a href=\"javascript:RNremove(" + info.Id + "," + i + ");\">x</a></td></tr>";
                        }
                    }
                }
                RN_containerNews.InnerHtml = vlreturn + "</table>";
                RN_containerNews.Attributes.Add("title", list == null ? "0" : list.Count.ToString());
                RN_txtRelatedid.Value = relateditem;
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