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

namespace dangdongcmm.cmm
{
    public partial class settingsite_restrictedpages : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindDLL();
                this.Re_LoadInfo();
            }
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Re_radChange", "Re_radChange();", true);
        }

        #region private methods
        private bool Validata()
        {
            try
            {
                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(txtName.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_name, "", txtName);
                if (!CFunctions.IsNullOrEmpty(txtName.Text) && txtName.Text.IndexOf(".aspx") == -1)
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_data, "", txtName);
                return Master.Form_ShowError(lstError);
            }
            catch
            {
                return false;
            }
        }
        private void BindDLL()
        {
            List<Settingsite.RestrictedPages> list = (new CRestrictedPages()).Wcmm_Getlist();
            if(list!=null && list.Count>0)
                foreach (Settingsite.RestrictedPages info in list)
                {
                    ListItem item = new ListItem(info.PathandQuery, info.Id.ToString());
                    ddlPathandQuery.Items.Add(item);
                }
        }

        private void Re_LoadInfo()
        {
            Re_radPage.SelectedIndex = CFunctions.IsNullOrEmpty(CConstants.PAGE_WELCOMEDEF) ? 0 : 1;
            Re_txtPage.Text = CConstants.PAGE_WELCOMEDEF;

            radRegisterconfirm.SelectedIndex = CConstants.REGISTERCONFIRM;
        }
        #endregion

        #region events
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                lstError = new List<Errorobject>();
                CRestrictedPages DAL = new CRestrictedPages();

                if (!DAL.Wcmm_Deleteall())
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Remove_completed, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                string RestrictedPages = "#";
                if (ddlPathandQuery.Items.Count > 0)
                {
                    List<Settingsite.RestrictedPages> list = new List<Settingsite.RestrictedPages>();
                    foreach (ListItem item in ddlPathandQuery.Items)
                    {
                        Settingsite.RestrictedPages info = new Settingsite.RestrictedPages();
                        info.Id = 0;
                        info.PathandQuery = item.Text;
                        if (item.Text.IndexOf(".aspx?") != -1)
                        {
                            string[] PathandQuery = item.Text.Split('?');
                            info.Name = PathandQuery[0];
                            info.Query = PathandQuery[1];
                        }
                        else
                        {
                            info.Name = item.Text;
                            info.Query = "";
                        }
                        RestrictedPages += info.Name + "#";
                        list.Add(info);
                    }

                    if (!DAL.Save(list))
                    {
                        lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Save_error, "", null);
                        Master.Form_ShowError(lstError);
                        return;
                    }
                }
                Application["RestrictedPages"] = RestrictedPages == "#" ? "" : RestrictedPages;

                CConstants.PAGE_WELCOMEDEF = Re_radPage.SelectedIndex == 0 ? "" : Re_txtPage.Text.Trim();
                CConstants.REGISTERCONFIRM = radRegisterconfirm.SelectedIndex;

                lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Save_completed, "", null);
                Master.Form_ShowError(lstError);

            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.Validata()) return;

                ListItem item = new ListItem(txtName.Text.Trim(), "0");
                ddlPathandQuery.Items.Insert(0, item);
                txtName.Text = "";
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlPathandQuery.SelectedIndex == -1) return;

                for (int i = ddlPathandQuery.Items.Count - 1; i >= 0; i--)
                {
                    ListItem item = ddlPathandQuery.Items[i];
                    if (item.Selected)
                        ddlPathandQuery.Items.RemoveAt(i);
                }
                ddlPathandQuery.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        #endregion
    }
}
