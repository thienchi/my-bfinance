using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm
{
    public partial class uccomment : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Visible = Allowcomment != 0;
                if (this.Visible)
                {
                    if (!CFunctions.IsNullOrEmpty(Texttitle))
                        lblTitle.Text = Texttitle;
                    if (!CFunctions.IsNullOrEmpty(Textbutton))
                        cmdSubmit.Text = Textbutton;
                    txtName.Text = "Re:" + Pname;
                    this.Bind_rptList(this.Belongto, this.Pid);
                    
                    CCommon.LoadCaptcha(imgCaptcha);
                }
            }
        }

        #region properties
        public int Allowcomment
        {
            get;
            set;
        }
        public int Belongto
        {
            get
            {
                return ViewState["Belongto"] == null ? 0 : int.Parse(ViewState["Belongto"].ToString());
            }
            set
            {
                ViewState["Belongto"] = value;
            }
        }
        public int Pid
        {
            get
            {
                return ViewState["Pid"] == null ? 0 : int.Parse(ViewState["Pid"].ToString());
            }
            set
            {
                ViewState["Pid"] = value;
            }
        }
        public string Pname
        {
            get;
            set;
        }

        public string Texttitle
        {
            get;
            set;
        }
        public string Textbutton
        {
            get;
            set;
        }
        #endregion

        #region private methods
        private CommentInfo Take()
        {
            try
            {
                CommentInfo info = new CommentInfo();
                info.Iid = Pid;
                info.Belongto = Belongto;
                info.Name = txtName.Text.Trim();
                info.Description = txtDescription.Text.Trim();
                info.Rating = int.Parse(radRating.SelectedValue);
                info.Sender_Name = CFunctions.IsNullOrEmpty(txtSender_Name.Text) ? "Guest" : txtSender_Name.Text.Trim();
                info.Sender_Email = txtSender_Email.Text.Trim();
                info.Sender_Phone = txtSender_Phone.Text.Trim();
                info.Status = (int)CConstants.State.Status.Waitactive;
                info.Username = "Guest";
                info.Timeupdate = DateTime.Now;
                return info;
            }
            catch
            {
                return null;
            }
        }
        private void Bind_rptList(int belongto, int iid)
        {
            int numResults = 0;
            ListOptions options = Get_ListOptionsNoPaging();
            List<CommentInfo> list = (new CComment(CCommon.LANG)).Getlist(belongto, iid, options, out numResults);
            (new GenericList<CommentInfo>(PageIndex, PageSize, SortExp, SortDir)).Bind_DataList(rptList, null, list, 0);
            pnlList.Visible = list != null && list.Count > 0;
            return;
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
                CommentInfo info = this.Take();
                if (info == null)
                {
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Takeinfo_error, "", null);
                    CCommon.Form_ShowError(lstError, lblError);
                    goto gotoExit;
                }

                CComment DAL = new CComment(CCommon.LANG);
                if (hidCheckemail.Value == "1" && !CFunctions.IsNullOrEmpty(info.Sender_Email))
                {
                    if (DAL.Checkdupemail(info.Belongto, info.Iid, info.Sender_Email))
                    {
                        lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Comment_dupemail, "", null);
                        CCommon.Form_ShowError(lstError, lblError);
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "showCForm", "showCForm();", true);
                        goto gotoExit;
                    }
                }
                if (hidCheckphone.Value == "1" && !CFunctions.IsNullOrEmpty(info.Sender_Phone))
                {
                    if (DAL.Checkdupphone(info.Belongto, info.Iid, info.Sender_Phone))
                    {
                        lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Comment_dupphone, "", null);
                        CCommon.Form_ShowError(lstError, lblError);
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "showCForm", "showCForm();", true);
                        goto gotoExit;
                    }
                }

                if (DAL.Save(info))
                {
                    new CGeneral(CCommon.LANG, Belongto).Updatenum(Pid.ToString(), Queryparam.Sqlcolumn.Allowcomment, CConstants.NUM_INCREASE);
                    CCommon.Session_Set(Sessionparam.WEBUSERCOMMENT, info);
                    pnlForm.Visible = false;
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Feedback_done, "", null);
                    CCommon.Form_ShowError(lstError, lblError);

                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Comment", "Comment();", true);
                }
                else
                {
                    pnlForm.Visible = false;
                    lstError = CCommon.Form_GetError(lstError, Errortype.Error, Definephrase.Feedback_error, "", null);
                    CCommon.Form_ShowError(lstError, lblError);
                }
                return;

            gotoExit:
                {
                    CCommon.LoadCaptcha(imgCaptcha);
                    txtCaptcha.Text = "";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "showCForm", "showCForm();", true);
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
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "showCForm", "showCForm();", true);
        }
        #endregion
    }
}