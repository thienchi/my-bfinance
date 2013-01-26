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
    public partial class settingsite_email : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.mail_LoadInfo(1);
            }
        }

        #region private methods
        
        private bool mail_Validata()
        {
            try
            {
                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(mail_txtReceiver.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_email, "", mail_txtReceiver);
                else
                {
                    string[] email_arr = mail_txtReceiver.Text.Trim().Split(',');
                    foreach (string email in email_arr)
                    {
                        if (CFunctions.IsNullOrEmpty(email) || !CFunctions.IsEmail(email))
                        {
                            lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_email, "", mail_txtReceiver);
                            break;
                        }
                    }
                }

                if (CFunctions.IsNullOrEmpty(mail_txtUsername.Text) || !CFunctions.IsEmail(mail_txtUsername.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_email, "", mail_txtUsername);
                return Master.Form_ShowError(lstError);
            }
            catch
            {
                return false;
            }
        }
        private Settingsite.MailServer mail_Take()
        {
            try
            {
                int iid = 0;
                int.TryParse(mail_txtId.Value, out iid);
                Settingsite.MailServer info = (new CMailServer()).Getinfo(iid);
                if (info == null)
                    info = new Settingsite.MailServer();
                info.Id = iid;
                info.SMTPServer = mail_txtSMTPServer.Text.Trim();
                info.SMTPPort = int.Parse(mail_txtSMTPPort.Text.Trim());
                info.UseSSL = mail_chkUseSSL.Checked ? 1 : 0;
                info.Receiver = mail_txtReceiver.Text.Trim();
                info.Username = mail_txtUsername.Text.Trim();
                info.Password = mail_chkChangepassword.Checked ? CFunctions.MBEncrypt(mail_txtPassword.Text) : info.Password;
                info.Timeupdate = DateTime.Now;

                return info;
            }
            catch
            {
                return null;
            }
        }
        private bool mail_LoadInfo(int iid)
        {
            try
            {
                Settingsite.MailServer info = (new CMailServer()).Getinfo(iid);
                if (info == null || info.Id == 0)
                    info = new Settingsite.MailServer();

                mail_txtId.Value = info.Id.ToString();
                mail_txtSMTPServer.Text = info.SMTPServer;
                mail_txtSMTPPort.Text = info.SMTPPort.ToString();
                mail_chkUseSSL.Checked = info.UseSSL!=0;
                mail_txtReceiver.Text = info.Receiver;
                mail_txtUsername.Text = info.Username;
                mail_txtPassword.Text = CFunctions.MBDecrypt(info.Password);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region events
        protected void mail_chkChangepassword_CheckedChanged(object sender, EventArgs e)
        {
            mail_txtPassword.Enabled = mail_chkChangepassword.Checked;
        }
        protected void mail_cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.mail_Validata()) return;

                lstError = new List<Errorobject>();
                Settingsite.MailServer info = this.mail_Take();
                if (info == null)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Takeinfo_error, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                if ((new CMailServer()).Save(info))
                {
                    lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Save_completed, "", null);
                    Master.Form_ShowError(lstError);
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
        #endregion
    }
}
