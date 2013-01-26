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

using dangdongcmm.model;
using dangdongcmm.utilities;
using dangdongcmm.dal;

namespace dangdongcmm
{
    public partial class contactus : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CCommon.LoadCaptcha(imgCaptcha);
            }
        }

        #region private methods
        private FeedbackInfo Take()
        {
            try
            {
                FeedbackInfo info = new FeedbackInfo();
                info.Sender_Name = txtSender_Nickname.Text.Trim();
                info.Sender_Email = txtSender_Email.Text.Trim();
                //info.Sender_Address = txtSender_Address.Text.Trim();
                info.Sender_Phone = txtSender_Phone.Text.Trim();
                info.Name = txtName.Text.Trim();
                info.Description = txtDescription.Text.Trim();
                info.Timeupdate = DateTime.Now;

                return info;
            }
            catch
            {
                return null;
            }
        }
        private bool Validata()
        {
            try
            {
                lstError = new List<Errorobject>();
                if (!imgCaptcha.ImageUrl.Contains("/" + CFunctions.MBEncrypt(txtCaptcha.Text) + ".png"))
                {
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_captcha, "", txtCaptcha);
                    txtCaptcha.Text = "";
                }
                return CCommon.Form_ShowError(lstError, lblError);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region events
        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.Validata())
                {
                    goto gotoExit;
                }

                lstError = new List<Errorobject>();
                FeedbackInfo info = this.Take();
                if (info == null)
                {
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Takeinfo_error, "", null);
                    CCommon.Form_ShowError(lstError, lblError);
                    goto gotoExit;
                }

                if ((new CFeedback()).Save(info))
                {
                    CCommon.Session_Set(Sessionparam.WEBUSERCOMMENT, info);
                    pnlForm.Visible = false;
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Feedback_done, "", null);
                    CCommon.Form_ShowError(lstError, lblError);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Feedback", "Feedback();", true);
                }
                else
                {
                    pnlForm.Visible = false;
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Feedback_error, "", null);
                    CCommon.Form_ShowError(lstError, lblError);
                }

            gotoExit:
                {
                    CCommon.LoadCaptcha(imgCaptcha);
                    txtCaptcha.Text = "";
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdCaptcha_Click(object sender, ImageClickEventArgs e)
        {
            CCommon.LoadCaptcha(imgCaptcha);
        }
        #endregion
    }
}
