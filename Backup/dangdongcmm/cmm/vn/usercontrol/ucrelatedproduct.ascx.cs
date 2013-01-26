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
    public partial class ucrelatedproduct : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindDDL_Cid(RP_ddlCid);
            }
        }

        #region private methods
        private void BindDDL_Cid(DropDownList ddl)
        {
            this.BindDDL_Cid(ddl, 0, "");
            CCommon.Insert_FirstitemDDL(ddl);
        }
        private void BindDDL_Cid(DropDownList ddl, int pid, string separator)
        {
            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Wcmm_Getlist(Webcmm.Id.Product, pid, Get_ListOptionsNoPaging());
            if (list == null) return;

            foreach (CategoryInfo info in list)
            {
                string sep = pid == 0 ? "" : separator + "---";
                ListItem item = new ListItem(sep + info.Name, info.Id.ToString());
                ddl.Items.Add(item);
                this.BindDDL_Cid(ddl, info.Id, sep);
            }
        }
        #endregion

        #region events
        public string Get()
        {
            return RP_txtRelatedid.Value;
        }
        public void Set(string relateditem)
        {
            try
            {
                string vlreturn = "<table id=\"RP_listselectedProduct\" class=\"RP_listselectedProduct\">";
                List<ProductInfo> list = new List<ProductInfo>();
                if (!CFunctions.IsNullOrEmpty(relateditem))
                {
                    list = new CProduct(CCommon.LANG).Wcmm_Getlist(relateditem);
                    if (list != null && list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            ProductInfo info = (ProductInfo)list[i];
                            vlreturn += "<tr id=\"rprowi" + i + "\" class=\"rprowi\">"
                                + "<td class=\"rpbase\"><img id=\"rpimag" + i + "\" src=\"" + (info.Filepreview.IndexOf(CConstants.WEBSITE) == 0 ? info.Filepreview : (CConstants.WEBSITE + "/" + info.Filepreview)) + "\" /></td>"
                            + "<td class=\"rpnote\">" + info.Name + "</td>"
                            + "<td class=\"rpsort\">" + info.Cname + "</td>"
                            + "<td class=\"rpcomd\"><a href=\"javascript:RPremove(" + info.Id + "," + i + ");\">x</a></td></tr>";
                        }
                    }
                }
                RP_containerProduct.InnerHtml = vlreturn + "</table>";
                RP_containerProduct.Attributes.Add("title", list == null ? "0" : list.Count.ToString());
                RP_txtRelatedid.Value = relateditem;
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