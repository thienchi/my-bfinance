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

using dangdongcmm.dal;
using dangdongcmm.model;
using dangdongcmm.utilities;

namespace dangdongcmm
{
    public partial class confirmation : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string problem = CFunctions.MBDecrypt(CCommon.Get_QueryString(Queryparam.Problem));
                string username = CFunctions.MBDecrypt(CCommon.Get_QueryString(Queryparam.User));
                string temporarycode = CFunctions.MBDecrypt(CCommon.Get_QueryString(Queryparam.Code));
                
                switch (problem)
                {
                    case "resetpassword":
                        CMember DALRS = new CMember(CCommon.LANG);
                        MemberInfo memberRS = DALRS.Getinfo(username);
                        if (memberRS == null)
                            lblError.Text = CCommon.Get_Definephrase(Definephrase.Invalid_username_notexist);
                        else
                        {
                            if (memberRS.Temporarycode == temporarycode && memberRS.Status == (int)CConstants.State.Status.Actived)
                            {
                                string passwordnew = CFunctions.Randomstr(6);
                                if (DALRS.Updatestr(memberRS.Id.ToString(), "password", CFunctions.MBEncrypt(passwordnew)))
                                {
                                    memberRS.Password = passwordnew;
                                    CCommon.Session_Set(Sessionparam.WEBUSERGETPASSWORD, memberRS);
                                    lblError.Text = CCommon.Get_Definephrase(Definephrase.Getpassword_done);

                                    DALRS.Updatestr(memberRS.Id.ToString(), "temporarycode", "");
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "resetpassword", "Resetpassword();", true);
                                }
                            }
                            else
                                lblError.Text = CCommon.Get_Definephrase(Definephrase.Confirm_error);
                        }
                        break;
                    case "regconfirm":
                        CMember DALRG = new CMember(CCommon.LANG);
                        MemberInfo memberRG = DALRG.Wcmm_Getinfo(username);
                        if (memberRG == null)
                            lblError.Text = CCommon.Get_Definephrase(Definephrase.Invalid_username_notexist);
                        else
                        {
                            if (memberRG.Temporarycode == temporarycode && memberRG.Status == (int)CConstants.State.Status.Waitactive)
                            {
                                if (DALRG.Updatenum(memberRG.Id.ToString(), Queryparam.Sqlcolumn.Status, (int)CConstants.State.Status.Actived))
                                {
                                    CCommon.Session_Set(Sessionparam.WEBUSERREGISTER, memberRG);
                                    lblError.Text = CCommon.Get_Definephrase(Definephrase.Confirm_register_done);

                                    DALRG.Updatestr(memberRG.Id.ToString(), "temporarycode", "");
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "regconfirm", "Regconfirm();", true);
                                }
                            }
                            else
                                lblError.Text = CCommon.Get_Definephrase(Definephrase.Confirm_error);
                        }
                        break;
                    case "restricted":
                        lblError.Text = CCommon.Get_Definephrase(Definephrase.Problem_restricted);
                        break;
                    case "notfound":
                        lblError.Text = CCommon.Get_Definephrase(Definephrase.Problem_notfound);
                        break;
                }
            }
        }
    }
}
