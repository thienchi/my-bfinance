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
    public partial class ucpager : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!this.Visible) return;

            if (this.TotalPages == 0 || PageIndex > this.TotalPages)
            {
                pnlList.Visible = false;
                return;
            }

            this.Initialize_Buttons();
            this.Initialize_Pages();
            
            string template = this.Infotemplate;
            template = template.Replace("$PAGEBEGIN$", ((PageIndex - 1) * PageSize + 1).ToString());
            template = template.Replace("$PAGEEND$", (PageIndex == TotalPages ? RecordCount : PageIndex * PageSize).ToString());
            template = template.Replace("$RECORDCOUNT$", RecordCount.ToString());
            lblInfotemplate.Text = template;
            pnlList.CssClass = this.CssClass;
        }

        #region properties
        public int TotalPages
        {
            get
            {
                if (ViewState["TotalPages"] != null)
                {
                    return (int)ViewState["TotalPages"];
                }
                else if (PageSize > 1)
                {
                    return RecordCount == 0 ? 0 : (RecordCount % PageSize == 0 ? RecordCount / PageSize : (RecordCount / PageSize) + 1);
                }
                else
                    return 1;
            }
            set
            {
                ViewState["TotalPages"] = value;
            }
        }
        public int RecordCount
        {
            get
            {
                if (ViewState["RecordCount"] != null)
                {
                    return (int)ViewState["RecordCount"];
                }
                else
                {
                    return 1;
                }
            }
            set
            {
                ViewState["RecordCount"] = value;
            }
        }
        public string Infotemplate
        {
            get;
            set;
        }
        public string CssClass
        {
            get;
            set;
        }
        public const int numPageview = 3;
        #endregion

        #region private methods
        private void Initialize_Pages()
        {
            int numBegin = 0, numEnd = numPageview;
            int numIndex = (PageIndex - 1) / numPageview;
            numBegin = numPageview * numIndex;
            numEnd = ((numPageview + numBegin) > this.TotalPages ? this.TotalPages : (numPageview + numBegin));

            List<ListItem> list = new List<ListItem>();
            for (int i = numBegin; i < numEnd; i++)
            {
                ListItem item = new ListItem((i + 1).ToString(), (i + 1).ToString());
                list.Add(item);
            }
            if (numBegin >= numPageview)
                list.Insert(0, new ListItem("...", numBegin.ToString()));
            if (numEnd < this.TotalPages)
                list.Add(new ListItem("...", (numEnd + 1).ToString()));

            rptList.DataSource = list;
            rptList.DataBind();
        }
        private void Initialize_Buttons()
        {
            cmdFirst.Enabled = cmdPrev.Enabled = cmdLast.Enabled = cmdNext.Enabled = true;
            if (TotalPages == 1)
            {
                cmdFirst.Enabled = cmdPrev.Enabled = cmdLast.Enabled = cmdNext.Enabled = false;
            }
            else if (PageIndex == TotalPages)
            {
                cmdLast.Enabled = cmdNext.Enabled = false;
            }
            else if (PageIndex == 1)
            {
                cmdFirst.Enabled = cmdPrev.Enabled = false;
            }
        }

        private void PagerIndexChanged(int pagecurrent)
        {
            string RawUrl = Request.RawUrl;
            RawUrl = RawUrl.IndexOf(".aspx") + 5 < RawUrl.Length ? RawUrl.Remove(RawUrl.IndexOf(".aspx") + 5) : RawUrl;
            System.Collections.Specialized.NameValueCollection collection = Request.QueryString;
            string querystring = "?";
            bool hasPage = false;
            if (collection != null && collection.Count > 0)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if (collection.GetKey(i) != Queryparam.Pageindex)
                    {
                        querystring += (i == 0 ? "" : "&") + collection.GetKey(i) + "=" + collection[i];
                    }
                    else
                    {
                        hasPage = true;
                        querystring += (i == 0 ? "" : "&") + collection.GetKey(i) + "=" + pagecurrent.ToString();
                    }
                }
            }
            if (!hasPage)
                querystring += (querystring == "?" ? "" : "&") + Queryparam.Pageindex + "=" + pagecurrent.ToString();

            Response.Redirect(RawUrl + querystring, false);
        }
        #endregion

        #region events
        protected void cmdFirst_Click(object sender, EventArgs e)
        {
            this.PageIndex = 1;
            PagerIndexChanged(this.PageIndex);
        }
        protected void cmdPrev_Click(object sender, EventArgs e)
        {
            this.PageIndex--;
            PagerIndexChanged(this.PageIndex);
        }
        protected void cmdLast_Click(object sender, EventArgs e)
        {
            this.PageIndex = this.TotalPages;
            PagerIndexChanged(this.PageIndex);
        }
        protected void cmdNext_Click(object sender, EventArgs e)
        {
            this.PageIndex++;
            PagerIndexChanged(this.PageIndex);
        }

        protected void rptList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Pageindex")
            {
                this.PageIndex = int.Parse(e.CommandArgument.ToString());
                PagerIndexChanged(this.PageIndex);
            }
        }
        #endregion
    }
}