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
    public partial class commentu : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Init_State();
                this.Load_Info(CCommon.Get_QueryNumber(Queryparam.Iid));
            }
        }

        #region properties
        public int BELONGTO
        {
            get
            {
                return ViewState["BELONGTO"] == null ? 0 : int.Parse(ViewState["BELONGTO"].ToString());
            }
            set
            {
                ViewState["BELONGTO"] = value;
            }
        }
        public int CID
        {
            get
            {
                return ViewState["CID"] == null ? 0 : int.Parse(ViewState["CID"].ToString());
            }
            set
            {
                ViewState["CID"] = value;
            }
        }
        public int PID
        {
            get
            {
                return ViewState["PID"] == null ? 0 : int.Parse(ViewState["PID"].ToString());
            }
            set
            {
                ViewState["PID"] = value;
            }
        }
        #endregion

        #region private methods
        private void Init_State()
        {
            int belongto = CCommon.Get_QueryNumber(Queryparam.Belongto);
            int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
            int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
            this.BELONGTO = belongto;
            this.CID = cid;
            this.PID = pid;
            
            if (belongto != 0)
            {
                this.BindDDL_Cid(ddlCid, belongto);
                if (ddlCid.Items.Count > 0)
                {
                    ddlCid.SelectedValue = ddlCid.Items.FindByValue(cid.ToString()) == null ? "0" : cid.ToString();
                    this.ddlCid_SelectedIndexChanged(null, null);
                    if (cid != 0)
                    {
                        if (ddlName.Items.Count > 0)
                        {
                            ddlName.SelectedValue = ddlName.Items.FindByValue(pid.ToString()) == null ? "0" : pid.ToString();
                        }
                    }
                }
            }

            chkSaveoption_golang.Visible = CFunctions.IsMultiLanguage();
        }

        private void BindDDL_Cid(DropDownList ddl, int belongto)
        {
            BindDDL_Cid(ddl, belongto, 0, "");
            CCommon.Insert_FirstitemDDL(ddl);
        }
        private void BindDDL_Name(int belongto, DropDownList ddl, int cid, string separator)
        {
            List<GeneralInfo> list = (new CGeneral(CCommon.LANG, belongto)).Wcmm_Getlist_cid(cid, Get_ListOptionsNoPaging());
            if (list == null) return;

            foreach (GeneralInfo info in list)
            {
                string sep = "";
                ListItem item = new ListItem(sep + info.Name, info.Id.ToString());
                ddl.Items.Add(item);
            }
        }

        private bool Validata()
        {
            try
            {
                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(txtName.Text))
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_name, "", txtName);
                //if (CFunctions.IsNullOrEmpty(ddlName.SelectedValue) || ddlName.SelectedValue == "0")
                //    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Require_catalogueinfo, "", ddlName);
                return Master.Form_ShowError(lstError);
            }
            catch
            {
                return false;
            }
        }
        private CommentInfo Take()
        {
            try
            {
                int iid = 0;
                int.TryParse(txtId.Value, out iid);
                CommentInfo info = (new CComment(CCommon.LANG)).Wcmm_Getinfo(iid);
                if (info == null)
                    info = new CommentInfo();
                info.Id = iid;
                info.Name = txtName.Text.Trim();
                info.Introduce = txtIntroduce.Text.Trim();
                info.Description = txtDescription.Text.Trim();
                if (info.Id == 0)
                {
                    info.Belongto = this.BELONGTO;
                    int pid = 0;
                    int.TryParse(ddlName.SelectedValue, out pid);
                    info.Iid = pid;
                }
                info.Sender_Name = txtSender_Name.Text.Trim();
                info.Sender_Email = txtSender_Email.Text.Trim();
                info.Sender_Address = txtSender_Address.Text.Trim();
                info.Sender_Phone = txtSender_Phone.Text.Trim();
                info.Iconex = Displaysetting.Get_Icon();
                info.Status = Displaysetting.Get_Status();
                info.Markas = Displaysetting.Get_Markas();
                info.Orderd = Displaysetting.Get_Orderd();
                info.Username = CCommon.Get_CurrentUsername();
                if (info.Id == 0)
                    info.Timecreate = DateTime.Now;
                info.Timeupdate = DateTime.Now;
                return info;
            }
            catch
            {
                return null;
            }
        }
        private bool Save(CommentInfo info)
        {
            try
            {
                if (info == null) return false;
                int id = info.Id;
                if ((new CComment(CCommon.LANG)).Save(info))
                {
                    if (id == 0)
                        (new CGeneral(CCommon.LANG, info.Belongto)).Updatenum(info.Iid.ToString(), Queryparam.Sqlcolumn.Allowcomment, CConstants.NUM_INCREASE);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool Save_Lang(CommentInfo info)
        {
            try
            {
                if (!CFunctions.IsMultiLanguage() || !chkSaveoption_golang.Checked) return false;

                int lang_num = CConstants.LANG_NUM;
                for (int i = 0; i < lang_num; i++)
                {
                    string lang_val = ConfigurationSettings.AppSettings["LANG_" + i];
                    if (lang_val == CCommon.LANG) continue;

                    CommentInfo lang_info = info.copy();
                    lang_info.Id = 0;
                    lang_info.Status = (int)CConstants.State.Status.Waitactive;
                    (new CComment(lang_val)).Save(lang_info);
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
                CommentInfo info = (new CComment(CCommon.LANG)).Wcmm_Getinfo(iid);
                if (info == null || info.Id == 0)
                {
                    info = new CommentInfo();
                }
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
                txtIntroduce.Text = info.Introduce;
                txtDescription.Text = info.Description;
                txtSender_Name.Text = info.Sender_Name;
                txtSender_Email.Text = info.Sender_Email;
                txtSender_Address.Text = info.Sender_Address;
                txtSender_Phone.Text = info.Sender_Phone;
                Displaysetting.Set(info.Iconex, info.Status, info.Orderd, info.Markas);

                if (info.Id != 0)
                {
                    ddlCid.SelectedValue = this.CID.ToString();
                    this.ddlCid_SelectedIndexChanged(null, null);
                    ddlName.SelectedValue = info.Iid.ToString();
                }
                //ddlName.Enabled = info.Id == 0;
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region events
        protected void ddlCid_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int belongto = this.BELONGTO;
                int cid = int.Parse(ddlCid.SelectedValue);
                ddlName.Items.Clear();
                this.CID = cid;
                if (belongto == 0) return;

                this.BindDDL_Name(belongto, ddlName, cid, "");
                CCommon.Insert_FirstitemDDL(ddlName);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.Validata()) return;

                lstError = new List<Errorobject>();
                CommentInfo info = this.Take();
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
