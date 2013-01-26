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

using dangdongcmm.dal;
using dangdongcmm.model;
using dangdongcmm.utilities;

namespace dangdongcmm
{
    public partial class ucsearch : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Init_State();
            }
        }

        #region private methods
        private void Init_State()
        {
            int levelofcid = int.Parse(LevelOfCid.Value);
            int rootofcid = int.Parse(RootOfCid.Value);
            if (pnlSearch.Visible && levelofcid > 0)
            {
                int count_nochecked = 0;
                foreach (ListItem item in chkBelongto.Items)
                {
                    if (item.Selected)
                    {
                        int cid;
                        int.TryParse(item.Value, out cid);
                        this.BindDDL_Cid(ddlCid, cid, rootofcid, levelofcid, "");
                    }
                    else
                        count_nochecked++;
                }
                if (count_nochecked != chkBelongto.Items.Count && ddlCid.Items.Count > 0)
                {
                    ddlCid.Visible = true;
                    if (rootofcid == 0)
                    {
                        CCommon.Insert_FirstitemDDL(ddlCid);
                    }
                    else
                    {
                        CategoryInfo rootinfo = (new CCategory(CCommon.LANG)).Getinfo(rootofcid);
                        if (rootinfo != null)
                        {
                            ListItem item = new ListItem(rootinfo.Name, rootinfo.Id.ToString());
                            ddlCid.Items.Insert(0, item);
                            ddlCid.SelectedIndex = 0;
                        }
                    }
                }
                else
                    ddlCid.Visible = false;
            }
        }
        private void BindDDL_Cid(DropDownList ddl, int cid, int pid, int levelofcid, string separator)
        {
            if (levelofcid == 0) return;

            ListOptions options = Get_ListOptionsNoPaging();
            options.SortExp = Queryparam.Sqlcolumn.Orderd;
            options.GetAll = true;

            int numResults = 0;
            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Getlist(cid, pid, options, out numResults);
            if (list == null) return;

            foreach (CategoryInfo info in list)
            {
                string sep = pid == 0 ? "" : separator + "---";
                ListItem item = new ListItem(sep + info.Name, info.Id.ToString());
                ddl.Items.Add(item);
                this.BindDDL_Cid(ddl, cid, info.Id, levelofcid - 1, sep);
            }
        }
        #endregion

        #region events
        protected void chkBelongto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ddlCid.Items.Clear();
                this.Init_State();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }

        protected void cmdSearch_Click(object sender, ImageClickEventArgs e)
        {
            if (CFunctions.IsNullOrEmpty(txtKeywords.Text)) return;

            string searchin = "";
            if (chkBelongto.SelectedItem != null)
            {
                bool firstItem = true;
                foreach (ListItem item in chkBelongto.Items)
                {
                    if (item.Selected && !CFunctions.IsNullOrEmpty(item.Value))
                    {
                        if (firstItem)
                        {
                            searchin += item.Value;
                            firstItem = false;
                        }
                        else
                        {
                            searchin += "," + item.Value;
                        }
                    }
                }
            }
            Response.Redirect(System.Uri.EscapeUriString("search.aspx?searchin=" + searchin + "&cid=" + ddlCid.SelectedValue + "&keywords=" + txtKeywords.Text));
        }
        #endregion
    }
}