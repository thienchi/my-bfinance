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
    public partial class ucassigntouser : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                UserInfo logger = CCommon.Get_CurrentUser();
                if (logger != null && logger.Id == 2)
                {
                    pnlUser.Visible = true;
                    this.BindDDL_User();
                }
                else
                {
                    pnlUser.Visible = false;
                }   
            }
        }

        #region private methods
        private void BindDDL_User()
        {
            if (ddlUser.Items.Count > 0) return;

            List<UserInfo> list = (new CUser()).Wcmm_Getlistuser(0);
            ddlUser.DataSource = list;
            ddlUser.DataTextField = "Username";
            ddlUser.DataValueField = "Username";
            ddlUser.DataBind();
            ListItem item = new ListItem(CCommon.Get_Definephrase(Definephrase.Firstitem_ddl), "");
            item.Attributes.Add("class", "textdefndis");
            ddlUser.Items.Insert(0, item);
        }

        #endregion

        #region public methods
        
        public void Set_User(string username)
        {
            this.BindDDL_User();
            if (ddlUser.Visible && ddlUser.Items.FindByValue(username) != null)
                ddlUser.SelectedValue = username;
        }
        public string Get_User()
        {
            return ddlUser.Visible && !CFunctions.IsNullOrEmpty(ddlUser.SelectedValue) ? ddlUser.SelectedValue : CCommon.Get_CurrentUsername();
        }
        #endregion
    }
}