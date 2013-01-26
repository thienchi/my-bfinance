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

namespace dangdongcmm
{
    public partial class M_password : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        #region events
        protected void cmdChangepassword_Click(object sender, EventArgs e)
        {
            try
            {
                MemberInfo logger = CCommon.CurrentMember.copy();
                if (logger == null) return;

                lstError = new List<Errorobject>();

                string passwordold = CFunctions.MBEncrypt(txtPasswordold.Text);
                string passwordnew = txtPasswordnew.Text;
                string passwordcon = txtPasswordcon.Text;
                if (passwordold != logger.Password)
                {
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Changepassword_invalid_passwordold, "", txtPasswordold);
                    CCommon.Form_ShowError(lstError, lblError);
                    return;
                }
                if (CFunctions.IsNullOrEmpty(passwordnew))
                {
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_password, "", txtPasswordnew);
                    CCommon.Form_ShowError(lstError, lblError);
                    return;
                }
                if (passwordnew != passwordcon)
                {
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Changepassword_invalid_passwordcon, "", txtPasswordcon);
                    CCommon.Form_ShowError(lstError, lblError);
                    return;
                }
                
                logger.Password = CFunctions.MBEncrypt(passwordnew);
                if ((new CMember()).ChangePwd(logger))
                {
                    CCommon.Session_Set(Sessionparam.WEBUSERLOGIN, logger);
                    lstError = CCommon.Form_GetError(lstError, Errortype.Notice, Definephrase.Changepassword_done, "", null);
                    CCommon.Session_Set(Sessionparam.ERROR, lstError);
                    Response.Redirect("M_profile.aspx", false);
                    return;
                }
                else
                {
                    lstError = CCommon.Form_GetError(lstError, Errortype.Warning, Definephrase.Changepassword_error, "", null);
                    CCommon.Form_ShowError(lstError, lblError);
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
