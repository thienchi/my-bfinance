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

namespace dangdongcmm.cmm
{
    public partial class ucsetupmember : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                cmdSave.Visible = CCommon.Right_upt(CFunctions.Get_Definecatrelate(BELONGTO, Queryparam.Defstring.Page));
                
            }
        }

        #region public methods
        public bool Show_Dialog(int belongto, string iid, int status)
        {
            this.IID = iid;
            this.BELONGTO = belongto;
            this.Bind_rptView(iid);

            this.Bind_Status(status);
            
            bool AllOK = pnlStatus.Visible;
            if (AllOK)
                MODALPOPUPEXTENDER1.Show();
            else
                return false;
            return true;
        }
        #endregion

        #region private methods
        private int BELONGTO
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
        private string IID
        {
            get
            {
                return ViewState["IID"] == null ? "0" : ViewState["IID"].ToString();
            }
            set
            {
                ViewState["IID"] = value;
            }
        }

        private void Bind_Status(int status)
        {
            radStatus.SelectedIndex = -1;
            if (status == Queryparam.Defstring.None || !CCommon.Right_sys())
            {
                pnlStatus.Visible = false;
            }
            else if (status == Queryparam.Defstring.Nospecifyint)
            {
                radStatus.SelectedIndex = 0;
            }
            else if (status == (int)CConstants.State.Status.Actived || status == (int)CConstants.State.Status.Disabled)
            {
                ListItem item = radStatus.Items.FindByValue(status.ToString());
                if (item != null)
                    item.Selected = true;
                else
                    radStatus.SelectedIndex = 0;
                pnlStatus.Visible = pnlStatus.Enabled = true;
            }
            else
            {
                radStatus.SelectedIndex = -1;
                pnlStatus.Enabled = false;
            }
        }
        private void Bind_rptView(string iid)
        {
            List<MemberInfo> list = (new CMember(CCommon.LANG)).Wcmm_Getlist(iid);
            if (list == null) return;

            rptView.DataSource = list;
            rptView.DataBind();
        }
        #endregion

        #region events
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                CMember DAL = new CMember(CCommon.LANG);
                if (pnlStatus.Visible && pnlStatus.Enabled)
                {
                    DAL.Updatenum(IID, Queryparam.Sqlcolumn.Status, radStatus.SelectedValue);
                }
                
                MODALPOPUPEXTENDER1.Hide();
                
                Control parent = this;
                do
                {
                    parent = (Control)parent.Parent;
                }
                while (!(parent is BasePage));
                BasePage control = (BasePage)parent;
                control.Bind_grdView();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            MODALPOPUPEXTENDER1.Hide();
        }
        #endregion
    }
}