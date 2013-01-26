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
    public partial class ucsetupattribute : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                cmdSave.Visible = CCommon.Right_upt(CFunctions.Get_Definecatrelate(BELONGTO, Queryparam.Defstring.Page));
                UserInfo logger = CCommon.Get_CurrentUser();
                if (logger != null && logger.Id == 2)
                {
                    pnlUser.Visible = true;
                    this.BindDDL_User();
                }
                else
                {
                    pnlUser.Visible = false;
                }

                this.Bind_Symbol();
            }
        }

        #region public methods
        public bool Show_Dialog(int belongto, string iid, string symbol, int status, int markas)
        {
            this.BELONGTO = belongto;
            this.IID = iid;
            this.Bind_grdView(iid, belongto);

            if (symbol == Queryparam.Defstring.Nosymbol)
            {
                pnlSymbol.Visible = false;
            }
            else
            {
                foreach (ListItem item in radSymbol.Items)
                    item.Selected = false;
                if (radSymbol.Items.FindByValue(symbol) != null)
                    radSymbol.SelectedValue = symbol;
                else
                    radSymbol.SelectedIndex = 0;
                pnlSymbol.Visible = true;
            }

            this.Bind_Status(status);
            this.Bind_Markas(markas);

            bool AllOK = pnlSymbol.Visible || pnlMarkas.Visible || pnlStatus.Visible || pnlUser.Visible;
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

        private void Bind_Symbol()
        {
            List<SymbolInfo> list = (new CSymbol(CCommon.LANG)).Getlist();

            radSymbol.DataSource = list;
            radSymbol.DataTextField = "eFilepreviewcmm";
            radSymbol.DataValueField = "Path";
            radSymbol.DataBind();
            ListItem item = new ListItem("Nothing", "");
            radSymbol.Items.Insert(0, item);
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
                //pnlStatus.Enabled = false;
            }
        }
        private void Bind_Markas(int markas)
        {
            radMarkas.SelectedIndex = -1;
            if (markas == Queryparam.Defstring.None)
            {
                pnlMarkas.Visible = false;
            }
            else
            {
                ListItem item = radMarkas.Items.FindByValue(markas.ToString());
                if (item != null)
                    item.Selected = true;
                else
                    radMarkas.SelectedIndex = 0;
                pnlMarkas.Visible = true;
            }
        }
        private void Bind_grdView(string iid, int belongto)
        {
            List<GeneralInfo> list = (new CGeneral(CCommon.LANG, belongto)).Wcmm_Getlist(iid);
            if (list == null) return;

            rptView.DataSource = list;
            rptView.DataBind();
        }
        private void BindDDL_User()
        {
            List<UserInfo> list = (new CUser()).Wcmm_Getlistuser(0);

            ddlUser.DataSource = list;
            ddlUser.DataTextField = "Username";
            ddlUser.DataValueField = "Username";
            ddlUser.DataBind();
            ListItem item = new ListItem(CCommon.Get_Definephrase(Definephrase.Firstitem_ddl), "");
            item.Attributes.Add("class", "textdefndis");
            ddlUser.Items.Insert(0, item);
        }
        #endregion

        #region events
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                CGeneral DAL = new CGeneral(CCommon.LANG, BELONGTO);
                if (pnlSymbol.Visible)
                {
                    DAL.Updatestr(IID, Queryparam.Sqlcolumn.Iconex, radSymbol.SelectedValue);
                }
                if (pnlStatus.Visible && pnlStatus.Enabled)
                {
                    if (radStatus.SelectedIndex != -1)
                        DAL.Updatenum(IID, Queryparam.Sqlcolumn.Status, radStatus.SelectedValue);
                }
                if (pnlMarkas.Visible)
                {
                    DAL.Updatenum(IID, Queryparam.Sqlcolumn.Markas, radMarkas.SelectedValue);
                }
                if (pnlUser.Visible && !CFunctions.IsNullOrEmpty(ddlUser.SelectedValue))
                {
                    DAL.Updatestr(IID, Queryparam.Sqlcolumn.Username, ddlUser.SelectedValue);
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