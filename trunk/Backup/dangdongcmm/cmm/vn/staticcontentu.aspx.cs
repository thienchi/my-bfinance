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
    public partial class staticcontentu : BasePage
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
                if (!chkSeparatepage.Checked)
                    if (CFunctions.IsNullOrEmpty(txtFilepath.Text))
                        lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_file, "", txtFilepath);
                return Master.Form_ShowError(lstError);
            }
            catch
            {
                return false;
            }
        }
        private StaticcontentInfo Take()
        {
            try
            {
                int iid = 0;
                int.TryParse(txtId.Value, out iid);
                StaticcontentInfo info = (new CStaticcontent(CCommon.LANG)).Wcmm_Getinfo(iid);
                if (info == null)
                    info = new StaticcontentInfo();
                info.Id = iid;
                info.Name = txtName.Text.Trim();
                info.Code = txtCode.Text.Trim();
                info.Separatepage = chkSeparatepage.Checked ? 1 : 0;
                info.Filepath = txtFilepath.Text.Trim();
                info.Description = txtDescription.Text.Trim();
                info.Status = CCommon.GetStatus_upt();
                info.Username = CCommon.Get_CurrentUsername();
                info.Timeupdate = DateTime.Now;
                return info;
            }
            catch
            {
                return null;
            }
        }
        private bool Save(StaticcontentInfo info)
        {
            try
            {
                if (info == null) return false;
                if ((new CStaticcontent(CCommon.LANG)).Save(info))
                {
                    if (info.Separatepage == 0)
                        this.Save_File(CCommon.LANG);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool Save_Lang(StaticcontentInfo info)
        {
            try
            {
                if (!CFunctions.IsMultiLanguage() || !chkSaveoption_golang.Checked) return false;

                int lang_num = CConstants.LANG_NUM;
                for (int i = 0; i < lang_num; i++)
                {
                    string lang_val = ConfigurationSettings.AppSettings["LANG_" + i];
                    if (lang_val == CCommon.LANG) continue;

                    StaticcontentInfo lang_info = info.copy();
                    lang_info.Id = 0;
                    lang_info.Status = (int)CConstants.State.Status.Waitactive;
                    if ((new CStaticcontent(lang_val)).Save(lang_info))
                        if (lang_info.Separatepage == 0)
                            this.Save_File(lang_val);
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
                StaticcontentInfo info = null;
                if (iid != 0)
                {
                    info = (new CStaticcontent(CCommon.LANG)).Wcmm_Getinfo(iid);
                    if (info != null)
                    {
                        lstError = new List<Errorobject>();
                        lstError = Form_GetError(lstError, Errortype.Notice, Definephrase.Save_notice, "[" + info.Id + "] " + info.Name, null);
                        Master.Form_ShowError(lstError);
                    }
                }
                if (info == null)
                    info = new StaticcontentInfo();
                chkSaveoption_golist.Checked = info.Id != 0;
                chkSaveoption_golang.Checked = info.Id == 0;

                txtId.Value = info.Id.ToString();
                txtName.Text = info.Name;
                txtCode.Text = info.Code;
                chkSeparatepage.Checked = info.Separatepage == 1 ? true : false;
                txtFilepath.Text = info.Filepath;
                txtDescription.Text = info.Description;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Load_File(string ffilename)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(ffilename)) return false;

                string ffilepath = Server.MapPath(PRE_FOLDER + DIR_UPLOAD + CCommon.LANG + "/" + ffilename);
                if (!System.IO.File.Exists(ffilepath))
                {
                    txtDescription.Text = "";
                }
                else
                {
                    System.IO.StreamReader sw = new System.IO.StreamReader(ffilepath, System.Text.UTF8Encoding.UTF8, false);
                    txtDescription.Text = sw.ReadToEnd();
                    sw.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool Save_File(string lang)
        {
            try
            {
                string ffilecontent = txtDescription.Text.Trim();
                string ffilename = txtFilepath.Text.Trim();
                string ffilepath = Server.MapPath(PRE_FOLDER + DIR_UPLOAD + lang + "/" + ffilename);
                if (!System.IO.File.Exists(ffilepath))
                {
                    System.IO.Directory.CreateDirectory(ffilepath.Replace(ffilename, ""));
                }
                System.IO.StreamWriter sw = new System.IO.StreamWriter(ffilepath, false, System.Text.UTF8Encoding.UTF8);
                HtmlTextWriter fs = new HtmlTextWriter(sw);
                fs.Write(ffilecontent);
                fs.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }
        private string DIR_UPLOAD = "xhtml/";
        private string PRE_FOLDER = "../../";
        #endregion

        #region events
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.Validata()) return;

                lstError = new List<Errorobject>();
                StaticcontentInfo info = this.Take();
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
