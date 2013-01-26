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
    public partial class banneru : BasePage
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
            chkSaveoption_golang.Visible = CFunctions.IsMultiLanguage();
        }
        
        private bool Validata()
        {
            try
            {
                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(txtName.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_name, "", txtName);
                return Master.Form_ShowError(lstError);
            }
            catch
            {
                return false;
            }
        }
        private BannerInfo Take()
        {
            try
            {
                int iid = 0;
                int.TryParse(txtId.Value, out iid);
                BannerInfo info = (new CBanner(CCommon.LANG)).Wcmm_Getinfo(iid);
                if (info == null)
                    info = new BannerInfo();
                info.Id = iid;
                info.Name = txtName.Text.Trim();
                info.Code = txtCode.Text.Trim();
                info.Status = Displaysetting.Get_Status();
                info.Username = "admin";
                info.Timeupdate = DateTime.Now;
                
                return info;
            }
            catch
            {
                return null;
            }
        }
        private bool Save(BannerInfo info)
        {
            try
            {
                if (info == null) return false;
                return (new CBanner(CCommon.LANG)).Save(info);
            }
            catch
            {
                return false;
            }
        }
        private bool Save_Lang(BannerInfo info)
        {
            try
            {
                if (!CFunctions.IsMultiLanguage() || !chkSaveoption_golang.Checked) return false;

                int lang_num = CConstants.LANG_NUM;
                for (int i = 0; i < lang_num; i++)
                {
                    string lang_val = ConfigurationSettings.AppSettings["LANG_" + i];
                    if (lang_val == CCommon.LANG) continue;

                    BannerInfo lang_info = info.copy();
                    lang_info.Id = 0;
                    lang_info.Status = (int)CConstants.State.Status.Waitactive;
                    CBanner DALBanner = new CBanner(lang_val);
                    if (DALBanner.Save(lang_info))
                    {
                        if (lang_info.lFileattach != null && lang_info.lFileattach.Count > 0)
                            if (new CFileattach(lang_val).Save(lang_info.lFileattach, lang_info.Id))
                                DALBanner.Updatenum(lang_info.Id.ToString(), Queryparam.Sqlcolumn.Album, lang_info.lFileattach.Count);
                    }
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
                BannerInfo info = null;
                if (iid != 0)
                {
                    info = (new CBanner(CCommon.LANG)).Wcmm_Getinfo(iid);
                    if (info != null)
                    {
                        lstError = new List<Errorobject>();
                        lstError = Form_GetError(lstError, Errortype.Notice, Definephrase.Save_notice, "[" + info.Id + "] " + info.Name, null);
                        Master.Form_ShowError(lstError);
                    }
                }
                if (info == null)
                    info = new BannerInfo();
                chkSaveoption_golist.Checked = info.Id != 0;
                chkSaveoption_golang.Checked = info.Id == 0;

                txtId.Value = info.Id.ToString();
                txtName.Text = info.Name;
                txtCode.Text = info.Code;
                Displaysetting.Set(string.Empty, info.Status, 0);
                Filebanner.Bind_Files(iid, Webcmm.Id.Banner);

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
                BannerInfo info = this.Take();
                if (info == null)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Takeinfo_error, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                if (this.Save(info))
                {
                    info.lFileattach = Filebanner.Save_Files(info.Id, Webcmm.Id.Banner);

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
