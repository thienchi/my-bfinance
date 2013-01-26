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
    public partial class changepassword : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        #region events
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session[Sessionparam.USERLOGIN] == null) return;
                UserInfo logger = (UserInfo)Session[Sessionparam.USERLOGIN];
                if (logger == null) return;

                lstError = new List<Errorobject>();

                string passwordold = CFunctions.MBEncrypt(txtPasswordold.Text);
                string passwordnew = txtPasswordnew.Text;
                string passwordcon = txtPasswordcon.Text;
                if (passwordold != logger.Password)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Changepassword_invalid_passwordold, "", txtPasswordold);
                    Master.Form_ShowError(lstError);
                    return;
                }
                if (CFunctions.IsNullOrEmpty(passwordnew))
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_password, "", txtPasswordnew);
                    Master.Form_ShowError(lstError);
                    return;
                }
                if (passwordnew != passwordcon)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Changepassword_invalid_passwordcon, "", txtPasswordcon);
                    Master.Form_ShowError(lstError);
                    return;
                }
                
                logger.Password = CFunctions.MBEncrypt(passwordnew);
                if ((new CUser()).ChangePwd(logger))
                {
                    CCommon.Session_Set(Sessionparam.USERLOGIN, logger);

                    lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Changepassword_done, "", null);
                    Master.Form_ShowError(lstError);
                }
                else
                {
                    lstError = Form_GetError(lstError, Errortype.Warning, Definephrase.Changepassword_error, "", null);
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
