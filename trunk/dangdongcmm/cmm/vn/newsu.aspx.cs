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
    public partial class newsu : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Init_State();
                this.BindDDL_Cid();
                this.Load_Info(CCommon.Get_QueryNumber(Queryparam.Iid));
            }
        }

        #region private methods
        private void Init_State()
        {
            int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
            int iid = CCommon.Get_QueryNumber(Queryparam.Iid);
            lblError.Text = CCommon.Get_Definephrase(Definephrase.Display_havesub_news).Replace(Queryparam.Varstring.Path, this.Generate_Parentpath(cid));

            chkSaveoption_golang.Visible = CFunctions.IsMultiLanguage();
        }
        private string Generate_Parentpath(int cid)
        {
            string vlreturn = "";
            List<CategoryInfo> listparent = (new CCategory(CCommon.LANG)).Wcmm_Getlist_parent(cid);
            if (listparent != null && listparent.Count > 0)
                foreach (CategoryInfo parent in listparent)
                    vlreturn += (CFunctions.IsNullOrEmpty(vlreturn) ? "" : " >> ") + "<a href='newsu.aspx?cid=" + parent.Id + "'>" + parent.Name + "</a>";

            if (CFunctions.IsNullOrEmpty(vlreturn))
            {
                CategorytypeofInfo infotypeof = (new CCategorytypeof(CCommon.LANG)).Wcmm_Getinfo(Webcmm.Id.News);
                vlreturn += (infotypeof == null ? "" : " <a href='newsl.aspx?cid=0'>" + infotypeof.Name + "</a>");
            }
            return vlreturn;
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
                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(ddlCid.SelectedValue) || ddlCid.SelectedValue == "0")
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_catalogue, "", ddlCid);
                if (CFunctions.IsNullOrEmpty(txtName.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_name, "", txtName);
                return Master.Form_ShowError(lstError);
            }
            catch
            {
                return false;
            }
        }
        private NewsInfo Take()
        {
            try
            {
                int iid = 0;
                int.TryParse(txtId.Value, out iid);
                NewsInfo info = (new CNews(CCommon.LANG)).Wcmm_Getinfo(iid);
                if (info == null)
                    info = new NewsInfo();
                info.Id = iid;
                info.Name = txtName.Text.Trim();
                info.Introduce = txtIntroduce.Text.Trim();
                info.Description = txtDescription.Text.Trim();
                info.Tag = txtTag.Text.Trim();
                info.Author = txtAuthor.Text.Trim();
                info.Url = txtUrl.Text.Trim();
                info.Timeexpire = CFunctions.IsNullOrEmpty(txtTimeexpire.Text.Trim()) ? CFunctions.Get_Datetime(CFunctions.Set_Datetime(DateTime.Now.AddDays(365))) : CFunctions.Get_Datetime(txtTimeexpire.Text.Trim());
                info.Allowcomment = chkAllowcomment.Checked ? (info.Allowcomment > 1 ? info.Allowcomment : 1) : 0;
                info.Filepreview = Filepreview.Get();
                info.Iconex = Displaysetting.Get_Icon();
                info.Status = Displaysetting.Get_Status();
                info.Orderd = Displaysetting.Get_Orderd();
                info.Markas = Displaysetting.Get_Markas();
                info.Relateditem = Relatednews.Get();
                info.Username = Assigntouser.Get_User();
                if (info.Id == 0) 
                    info.Timecreate = DateTime.Now;
                info.Timeupdate = DateTime.Now;

                info.Cid = int.Parse(ddlCid.SelectedValue);
                string categoryid = CCommon.Get_FormString(Queryparam.Chkcategorymulti);
                if (CFunctions.IsNullOrEmpty(categoryid)) 
                    info.lCategory = null;
                info.Categoryid = categoryid;
                string categoryattrid = CCommon.Get_FormString(Queryparam.Chkcategoryattr);
                if (CFunctions.IsNullOrEmpty(categoryattrid))
                    info.lCategoryattr = null;
                info.Categoryattrid = categoryattrid;
                
                return info;
            }
            catch
            {
                return null;
            }
        }
        private bool Save(NewsInfo info)
        {
            try
            {
                if (info == null) return false;
                (new CNews(CCommon.LANG)).Save(info);
                (new CNews(CCommon.LANG)).UpdateAliasInfo(info);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool Save_Lang(NewsInfo info)
        {
            try
            {
                if (!CFunctions.IsMultiLanguage() || !chkSaveoption_golang.Checked) return false;

                int lang_num = CConstants.LANG_NUM;
                for (int i = 0; i < lang_num; i++)
                {
                    string lang_val = ConfigurationSettings.AppSettings["LANG_" + i];
                    if (lang_val == CCommon.LANG) continue;

                    NewsInfo lang_info = info.copy();
                    lang_info.Id = 0;
                    lang_info.Status = (int)CConstants.State.Status.Waitactive;
                    (new CNews(lang_val)).Save(lang_info);
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
                NewsInfo info = null;
                if (iid != 0)
                {
                    info = (new CNews(CCommon.LANG)).Wcmm_Getinfo(iid);
                    if (info != null)
                    {
                        lstError = new List<Errorobject>();
                        lstError = Form_GetError(lstError, Errortype.Notice, Definephrase.Save_notice, "[" + info.Id + "] " + info.Name, null);
                        Master.Form_ShowError(lstError);
                    }
                }
                if (info == null)
                    info = new NewsInfo();
                chkSaveoption_golist.Checked = info.Id != 0;
                chkSaveoption_golang.Checked = info.Id == 0;

                txtId.Value = info.Id.ToString();
                txtName.Text = info.Name;
                txtIntroduce.Text = info.Introduce;
                txtDescription.Text = info.Description;
                txtTag.Text = info.Tag;
                txtAuthor.Text = info.Author;
                txtUrl.Text = info.Url;
                txtTimeexpire.Text = info.Timeexpire.Equals(new DateTime(0)) ? CFunctions.Set_Datetime(DateTime.Now.AddDays(365)) : CFunctions.Set_Datetime(info.Timeexpire);
                chkAllowcomment.Checked = info.Allowcomment > 0 ? true : false;
                Filepreview.Set(info.Filepreview);
                Displaysetting.Set(info.Iconex, info.Status, info.Orderd, info.Markas);
                Relatednews.Set(info.Relateditem);
                Assigntouser.Set_User(info.Username);

                ddlCid.SelectedValue = info.Cid != 0 ? info.Cid.ToString() : CCommon.Get_QueryNumber(Queryparam.Cid).ToString();
                Categorymulti.Build_Categorymulti(Webcmm.Id.News, 0, info.Categoryid);
                Categoryattr.Build_Categoryattr(Webcmm.Id.News, 0, info.Categoryattrid);
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
                NewsInfo info = this.Take();
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
                    if (chkSaveoption_golist.Checked)
                        this.Load_Info(info.Id);
                    Form_SaveOption(!chkSaveoption_golist.Checked);
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
