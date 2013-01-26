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
    public partial class categorytypeofu : BasePage
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
                if (!CFunctions.IsInteger(txtId.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_int, "", txtId);
                return Master.Form_ShowError(lstError);
            }
            catch
            {
                return false;
            }
        }
        private CategorytypeofInfo Take()
        {
            try
            {
                int iid = 0;
                int.TryParse(txtId.Text, out iid);
                CategorytypeofInfo info = (new CCategorytypeof(CCommon.LANG)).Wcmm_Getinfo(iid);
                if (info == null)
                    info = new CategorytypeofInfo();
                info.Id = iid;
                info.Name = txtName.Text.Trim();
                info.Code = txtCode.Text.Trim();
                info.Note = txtNote.Text.Trim();
                info.Status = int.Parse(radStatus.SelectedValue);
                info.Markas = int.Parse(radMarkas.SelectedValue);
                info.Username = CCommon.Get_CurrentUsername();
                info.Timeupdate = DateTime.Now;
                return info;
            }
            catch
            {
                return null;
            }
        }
        private bool Save(CategorytypeofInfo info)
        {
            try
            {
                if (info == null) return false;
                return (new CCategorytypeof(CCommon.LANG)).Save(info);
            }
            catch
            {
                return false;
            }
        }
        private bool Save_Lang(CategorytypeofInfo info)
        {
            try
            {
                if (!CFunctions.IsMultiLanguage() || !chkSaveoption_golang.Checked) return false;

                int lang_num = CConstants.LANG_NUM;
                for (int i = 0; i < lang_num; i++)
                {
                    string lang_val = ConfigurationSettings.AppSettings["LANG_" + i];
                    if (lang_val == CCommon.LANG) continue;

                    CategorytypeofInfo lang_info = info.copy();
                    lang_info.Id = 0;
                    lang_info.Status = (int)CConstants.State.Status.Waitactive;
                    (new CCategorytypeof(lang_val)).Save(lang_info);
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
                CategorytypeofInfo info = null;
                if (iid != 0)
                {
                    info = (new CCategorytypeof(CCommon.LANG)).Wcmm_Getinfo(iid);
                    if (info != null)
                    {
                        lstError = new List<Errorobject>();
                        lstError = Form_GetError(lstError, Errortype.Notice, Definephrase.Save_notice, "[" + info.Id + "] " + info.Name, null);
                        Master.Form_ShowError(lstError);
                    }
                }
                if (info == null)
                    info = new CategorytypeofInfo();
                chkSaveoption_golist.Checked = info.Id != 0;
                chkSaveoption_golang.Checked = info.Id == 0;

                txtId.Text = info.Id == 0 ? "" : info.Id.ToString();
                txtId.ReadOnly = info.Id != 0;
                txtName.Text = info.Name;
                txtCode.Text = info.Code;
                txtNote.Text = info.Note;
                radStatus.SelectedValue = info.Status.ToString();
                radMarkas.SelectedValue = info.Markas.ToString();

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
                CategorytypeofInfo info = this.Take();
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
            this.Load_Info(0);
            Master.Form_ClearError();
        }
        #endregion
    }
}
