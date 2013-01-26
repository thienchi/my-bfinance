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
    public partial class ucsymbol : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        #region private methods
        private string get_lang()
        {
            try
            {
                string page_path = Request.Url.AbsolutePath;
                string page_lang = page_path.Substring(page_path.LastIndexOf("/") - 2, 2);
                return page_lang;
            }
            catch
            {
                return "";
            }
        }

        public void Bind_List(string symbol)
        {
            try
            {
                string lang = this.get_lang();
                List<SymbolInfo> list = (new CSymbol(lang)).Getlist();

                radSymbol.DataSource = list;
                radSymbol.DataTextField = "eFilepreviewcmm";
                radSymbol.DataValueField = "Path";
                radSymbol.DataBind();
                ListItem item = new ListItem("Nothing", "");
                radSymbol.Items.Insert(0, item);
                item.Selected = true;

                item = radSymbol.Items.FindByValue(symbol);
                if (item != null)
                    item.Selected = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region public methods
        public void Set_Selected(string symbol)
        {
            try
            {
                ListItem item = radSymbol.Items.FindByValue(symbol);
                if (item != null)
                    item.Selected = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}