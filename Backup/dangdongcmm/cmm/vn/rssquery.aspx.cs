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
using System.Text;

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

using RssToolkit;
using HtmlAgilityPack;

namespace dangdongcmm.cmm
{
    public partial class rssquery : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindDDL_RSSUrl();
                this.BindDDL_Cid();

                int iid = CCommon.Get_QueryNumber(Queryparam.Iid);
                RSSResourceInfo info = (new CRSSResource(CCommon.LANG)).Wcmm_Getinfo(iid);
                if (info != null)
                {
                    if (ddlRSSUrl.Items.FindByValue(info.RSSUrl) != null)
                        ddlRSSUrl.SelectedValue = info.RSSUrl;
                    txtRSSUrl.Text = info.RSSUrl;

                    this.Bind_RSSInfo(info);
                    this.cmdQuery_Click(null, null);
                }
            }
        }
        
        #region private methods
        private void BindDDL_RSSUrl()
        {
            int numResults = 0;
            List<RSSResourceInfo> list = (new CRSSResource(CCommon.LANG)).Getlist(Get_ListOptionsNoPaging(), out numResults);
            if (list == null)
                list = new List<RSSResourceInfo>();

            ddlRSSUrl.DataSource = list;
            ddlRSSUrl.DataTextField = "Name";
            ddlRSSUrl.DataValueField = "RSSUrl";
            ddlRSSUrl.DataBind();

            CCommon.Insert_FirstitemDDL(ddlRSSUrl, "");
        }
        private void Bind_RSSInfo(RSSResourceInfo info)
        {
            if (info == null) 
                info = new RSSResourceInfo();
            List<RSSResourceInfo> list = new List<RSSResourceInfo>();
            list.Add(info);

            rptList.DataSource = list;
            rptList.DataBind();

            if (ddlCid.Items.Count > 0)
            {
                if (ddlCid.Items.FindByValue(info.Cid.ToString()) != null)
                    ddlCid.SelectedValue = info.Cid.ToString();
            }
        }
        private void BindDDL_Cid()
        {
            BindDDL_Cid(ddlCid, Webcmm.Id.News, 0, "");
            CCommon.Insert_FirstitemDDL(ddlCid);
        }

        private string Get_NewsContent(string newsUrl, string websiteUrl, string nodeContent, string nodeTitle, string nodeIntroduce)
        {
            if (CFunctions.IsNullOrEmpty(nodeContent)) return "";

            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = hw.Load(newsUrl);
            if (doc == null) return "";

            HtmlNodeCollection nc = doc.DocumentNode.SelectNodes("//*[@" + nodeContent + "]");

            StringBuilder sb = new StringBuilder();
            foreach (HtmlNode n in nc)
            {
                foreach (HtmlNode n_link in n.SelectNodes("//a[@href]"))
                {
                    HtmlAttribute att = n_link.Attributes["href"];
                    if (att.Value.IndexOf("http://") != -1) continue;
                    att.Value = att.Value.StartsWith("/") ? websiteUrl + att.Value : websiteUrl + "/" + att.Value;
                    n_link.Attributes.Add("target", "_blank");
                }
                foreach (HtmlNode n_img in n.SelectNodes("//img[@src]"))
                {
                    HtmlAttribute att = n_img.Attributes["src"];
                    if (att.Value.IndexOf("http://") != -1) continue;
                    att.Value = att.Value.StartsWith("/") ? websiteUrl + att.Value : websiteUrl + "/" + att.Value;
                }

                string allContent = n.InnerHtml;
                
                string title = "";
                if (!CFunctions.IsNullOrEmpty(nodeTitle))
                {
                    HtmlNodeCollection n_title = n.SelectNodes("//*[@" + nodeTitle + "]");
                    foreach (HtmlNode nc_title in n_title)
                        title += nc_title.InnerHtml;
                    allContent = allContent.Replace(title, "");
                }

                string lead = "";
                if (!CFunctions.IsNullOrEmpty(nodeIntroduce))
                {
                    HtmlNodeCollection n_lead = n.SelectNodes("//*[@" + nodeIntroduce + "]");
                    foreach (HtmlNode nc_lead in n_lead)
                        lead += nc_lead.InnerHtml;
                    allContent = allContent.Replace(lead, "");
                }

                sb.Append(allContent);
            }
            return sb.ToString();
        }
        #endregion

        #region events
        protected void grdView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                GridView gridView = (GridView)sender;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes.Add("onmouseover", "lEffect_Over(this)");
                    e.Row.Attributes.Add("onmouseout", "lEffect_Out(this)");
                    e.Row.Attributes.Add("id", e.Row.DataItemIndex.ToString());
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }

        protected void cmdQuery_Click(object sender, EventArgs e)
        {
            try
            {
                RSSResourceInfo info = (new CRSSResource(CCommon.LANG)).Wcmm_Getinfo(txtRSSUrl.Text.Trim());
                this.Bind_RSSInfo(info);
                
                RssDataSource1.Url = txtRSSUrl.Text.Trim();
                grdView.DataBind();

                Master.Form_ClearError();
            }
            catch
            {
                lstError = new List<Errorobject>();
                lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Takeinfo_error, "", null);
                Master.Form_ShowError(lstError);
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                string iid = CCommon.Get_FormString(Queryparam.Chkcheck);
                if (CFunctions.IsNullOrEmpty(iid)) return;

                int rssId = 0;
                string websiteUrl = "", nodeContent = "", nodeTitle = "", nodeIntroduce = "";
                foreach (RepeaterItem row in rptList.Items)
                {
                    HiddenField txtId = (HiddenField)row.FindControl("txtId");
                    rssId = txtId == null ? 0 : int.Parse(txtId.Value);
                    websiteUrl = ((HiddenField)row.FindControl("txtWebsiteUrl")).Value;
                    nodeContent = ((HiddenField)row.FindControl("txtNodecontent")).Value;
                    nodeTitle = ((HiddenField)row.FindControl("txtNodetitle")).Value;
                    nodeIntroduce = ((HiddenField)row.FindControl("txtNodeintroduce")).Value;
                }

                iid = "," + iid + ",";
                CNews DAL = new CNews(CCommon.LANG);
                foreach (GridViewRow row in grdView.Rows)
                {
                    HiddenField txtId = (HiddenField)row.FindControl("txtId");
                    HiddenField txtTitle = (HiddenField)row.FindControl("txtTitle");
                    HiddenField txtDescription = (HiddenField)row.FindControl("txtDescription");
                    HiddenField txtLink = (HiddenField)row.FindControl("txtLink");
                    if (txtId != null)
                    {
                        string id = txtId.Value;
                        if (iid.IndexOf("," + id + ",") != -1)
                        {
                            NewsInfo info = new NewsInfo();
                            info.Name = txtTitle.Value;
                            info.Code = "";
                            info.Introduce = txtDescription.Value;
                            info.Description = radSavetype.SelectedIndex == 0 ? this.Get_NewsContent(txtLink.Value, websiteUrl, nodeContent, nodeTitle, nodeIntroduce) : "";
                            info.Url = radSavetype.SelectedIndex == 0 ? "" : txtLink.Value;
                            info.Author = websiteUrl.Replace("http://", "");
                            info.Cid = int.Parse(ddlCid.SelectedValue);
                            info.Cname = ddlCid.SelectedItem.Text.Replace("-", "");
                            info.Timeexpire = CFunctions.Get_Datetime(CFunctions.Set_Datetime(DateTime.Now.AddDays(365)));

                            info.Status = CCommon.GetStatus_upt();
                            info.Username = CCommon.Get_CurrentUsername();
                            info.Timeupdate = DateTime.Now;

                            DAL.Save(info);
                        }
                    }
                }
                lstError = new List<Errorobject>();
                lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Save_completed, "", null);
                Master.Form_ShowError(lstError);

                (new CRSSResource(CCommon.LANG)).Updatestr(rssId.ToString(), "timelastestget", DateTime.Now);
            }
            catch
            {
                lstError = new List<Errorobject>();
                lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Save_error, "", null);
                Master.Form_ShowError(lstError);
            }
        }
        #endregion
    }
}
