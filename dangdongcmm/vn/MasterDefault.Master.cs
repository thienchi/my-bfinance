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

using dangdongcmm.utilities;

namespace dangdongcmm
{
    public partial class MasterDefault : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public string LANG
        {
            get
            {
                return CCommon.LANG;
            }
            set
            {
                CCommon.LANG = value;
            }
        }
        
        public void AddMeta_Title(string content)
        {
            try
            {
                //this.Head.Title = ConfigurationSettings.AppSettings["TitlePrefix"].Replace("$TITLE$", CFunctions.install_metatag(content));
                this.Head.Title = ConfigurationSettings.AppSettings["TitlePrefix"].Replace("$TITLE$", content);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public void AddMeta_Description(string content)
        {
            try
            {
                HtmlMeta Description = (HtmlMeta)this.Head.FindControl("meta_description");
                if (Description != null)
                {
                    Description.Content = CFunctions.install_metatag(content);
                }
            }
            catch(Exception ex)
            {
                throw (ex);
            }
        }
        public void AddMeta_Keywords(string content)
        {
            try
            {
                HtmlMeta Keywords = (HtmlMeta)this.Head.FindControl("meta_keywords");
                if (Keywords != null)
                {
                    Keywords.Content = CFunctions.install_metatag(content);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

    }
}
