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
    public partial class ucdisplaysetting : BaseUserControl
    {
        private bool _Showstatus = true;
        private bool _Showdisplayorder = true;
        private bool _Showicon = true;
        private bool _Showmarkas = false;

        public bool Showstatus
        {
            get{return _Showstatus;}
            set{_Showstatus=value;}
        }
        public bool Showdisplayorder
        {
            get { return _Showdisplayorder; }
            set { _Showdisplayorder = value; }
        }
        public bool Showicon
        {
            get { return _Showicon; }
            set { _Showicon = value; }
        }
        public bool Showmarkas
        {
            get { return _Showmarkas; }
            set { _Showmarkas = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlStatus.Visible = Showstatus;
            pnlDisplayorder.Visible = Showdisplayorder;
            pnlIcon.Visible = Showicon;
            pnlMarkas.Visible = Showmarkas;
        }

        #region public methods
        public void Bind_Listicon(string icon)
        {
            if (radIcon.Items.Count == 0)
            {
                List<SymbolInfo> list = (new CSymbol(CCommon.LANG)).Getlist();

                radIcon.DataSource = list;
                radIcon.DataTextField = "eFilepreviewcmm";
                radIcon.DataValueField = "Path";
                radIcon.DataBind();
                ListItem item = new ListItem("Nothing", "");
                radIcon.Items.Insert(0, item);
                item.Selected = true;
            }

            ListItem itemsel = radIcon.Items.FindByValue(icon);
            if (itemsel != null)
                itemsel.Selected = true;
        }

        public void Set(string icon, int status, int orderd)
        {
            try
            {
                if (pnlStatus.Visible)
                    radStatus.SelectedValue = status.ToString();
                if (pnlDisplayorder.Visible)
                    txtOrderd.Text = orderd == 0 ? "" : orderd.ToString();
                if (pnlIcon.Visible)
                    this.Bind_Listicon(icon);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        public void Set(string icon, int status, int orderd, int markas)
        {
            try
            {
                this.Set(icon, status, orderd);
                if (pnlMarkas.Visible)
                    radMarkas.SelectedValue = markas.ToString();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        public string Get_Icon()
        {
            return radIcon.SelectedValue;
        }
        public int Get_Status()
        {
            return CCommon.GetStatus_upt(radStatus.SelectedValue);
        }
        public int Get_Orderd()
        {
            return CFunctions.IsInteger(txtOrderd.Text) ? int.Parse(txtOrderd.Text) : 0;
        }
        public int Get_Markas()
        {
            return int.Parse(radMarkas.SelectedValue);
        }
        #endregion

    }
}