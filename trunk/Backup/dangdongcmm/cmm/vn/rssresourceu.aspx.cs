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
    public partial class rssresourceu : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Init_State();
                this.Load_Info(CCommon.Get_QueryNumber(Queryparam.Iid));
            }
        }
        
        #region private methods
        private void Init_State()
        {
            this.BindDDL_Cid();

            chkSaveoption_golang.Visible = CFunctions.IsMultiLanguage();
        }
        private void BindDDL_Cid()
        {
            BindDDL_Cid(ddlCid, Webcmm.Id.News, 0, "");
            CCommon.Insert_FirstitemDDL(ddlCid);
        }
        
        private bool Validata()
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(txtName.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_name, "", txtName);
                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(ddlCid.SelectedValue) || ddlCid.SelectedValue == "0")
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_catalogue, "", ddlCid);
                return Master.Form_ShowError(lstError);
            }
            catch
            {
                return false;
            }
        }
        private RSSResourceInfo Take()
        {
            try
            {
                int iid = 0;
                int.TryParse(txtId.Value, out iid);
                RSSResourceInfo info = (new CRSSResource(CCommon.LANG)).Wcmm_Getinfo(iid);
                if (info == null)
                    info = new RSSResourceInfo();
                info.Id = iid;
                info.Name = txtName.Text.Trim();
                info.Code = "";
                info.WebsiteUrl = txtWebsiteUrl.Text.Trim();
                info.RSSUrl = txtRSSUrl.Text.Trim();
                info.Cid = int.Parse(ddlCid.SelectedValue);
                info.Cname = ddlCid.SelectedItem.Text.Replace("-", "");
                info.Nodecontent = txtNodecontent.Text;
                info.Nodetitle = txtNodetitle.Text;
                info.Nodeintroduce = txtNodeintroduce.Text;

                info.Timeupdate = DateTime.Now;
                return info;
            }
            catch
            {
                return null;
            }
        }
        private bool Save(RSSResourceInfo info)
        {
            try
            {
                if (info == null) return false;
                return (new CRSSResource(CCommon.LANG)).Save(info);
            }
            catch
            {
                return false;
            }
        }
        private bool Save_Lang(RSSResourceInfo info)
        {
            try
            {
                if (!CFunctions.IsMultiLanguage() || !chkSaveoption_golang.Checked) return false;

                int lang_num = CConstants.LANG_NUM;
                for (int i = 0; i < lang_num; i++)
                {
                    string lang_val = ConfigurationSettings.AppSettings["LANG_" + i];
                    if (lang_val == CCommon.LANG) continue;

                    RSSResourceInfo lang_info = info.copy();
                    lang_info.Id = 0;
                    lang_info.Status = (int)CConstants.State.Status.Waitactive;
                    (new CRSSResource(lang_val)).Save(lang_info);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool Load_Info(int iid)
        {
            try
            {
                RSSResourceInfo info = (new CRSSResource(CCommon.LANG)).Wcmm_Getinfo(iid);
                if (info == null || info.Id == 0)
                    info = new RSSResourceInfo();
                else
                {
                    lstError = new List<Errorobject>();
                    lstError = Form_GetError(lstError, Errortype.Notice, Definephrase.Save_notice, "[" + info.Id + "] " + info.Name, null);
                    Master.Form_ShowError(lstError);
                }
                chkSaveoption_golist.Checked = info.Id != 0;
                chkSaveoption_golang.Checked = info.Id == 0;

                txtId.Value = info.Id.ToString();
                txtName.Text = info.Name;
                txtWebsiteUrl.Text = info.WebsiteUrl;
                txtRSSUrl.Text = info.RSSUrl;
                ddlCid.SelectedValue = info.Cid.ToString();
                lblTimelastestget.Text = CFunctions.IsNullOrEmpty(info.eTimelastestget) ? "n/a" : info.eTimelastestget;
                txtNodecontent.Text = info.Nodecontent;
                txtNodetitle.Text = info.Nodetitle;
                txtNodeintroduce.Text = info.Nodeintroduce;
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region events
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.Validata()) return;
                
                lstError = new List<Errorobject>();
                RSSResourceInfo info = this.Take();
                if (info == null)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Takeinfo_error, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                if (this.Save(info))
                {
                    lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Save_completed, "", null);
                    if (this.Save_Lang(info))
                        lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Savemultilang_completed, "", null);
                    Master.Form_ShowError(lstError);
                    Form_SaveOption(chkSaveoption_golist.Checked);
                    
                    this.Load_Info(0);
                }
                else
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Save_error, "", null);
                    Master.Form_ShowError(lstError);
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdClear_Click(object sender, EventArgs e)
        {
            try
            {
                this.Load_Info(0);
                Master.Form_ClearError();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }

        #endregion
    }
}
