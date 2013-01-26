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

namespace dangdongcmm
{
    public partial class news : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
                int iid = CCommon.Get_QueryNumber(Queryparam.Iid);
                this.Cid = cid;
                this.Load_Info(iid);

                if (pnlInfo.Visible)
                {
                    this.Bind_rptCategory();
                    if (CFunctions.GetViewSetting(Webcmm.Id.News, ViewSetting.ListVisibled))
                    {
                        this.Bind_rptListByCategory();
                    }
                    else
                    {
                        pnlListByCategory.Visible = false;
                    }
                    if (CFunctions.GetViewSetting(Webcmm.Id.News, ViewSetting.ListFollowed))
                        this.Bind_rptListfollow();
                    else
                        pnlListfollow.Visible = false;
                }
                else
                {
                    if (!CFunctions.GetViewSetting(Webcmm.Id.News, ViewSetting.ListByCategory))
                        this.Bind_rptCategory();
                    this.Bind_rptListByCategory();
                }

            }
        }

        #region properties
        private int Cid
        {
            get
            {
                return ViewState["Cid"] == null ? 0 : int.Parse(ViewState["Cid"].ToString());
            }
            set
            {
                ViewState["Cid"] = value;
            }
        }
        #endregion

        #region private methods
        private void Bind_rptListByCategory()
        {
            CCategory DAL = new CCategory(CCommon.LANG);
            List<CategoryInfo> list = null;
            if (!CFunctions.GetViewSetting(Webcmm.Id.News, ViewSetting.ListWhenCategoryBrowse))
            {
                int numResults = 0;
                list = DAL.Getlist(Webcmm.Id.News, this.Cid, Get_ListOptions(), out numResults);
                if (list != null && list.Count > 0) return;
            }

            if (CFunctions.GetViewSetting(Webcmm.Id.News, ViewSetting.ListByCategory))
            {
                int numResults = 0;
                list = DAL.Getlist(Webcmm.Id.News, this.Cid, Get_ListOptions(), out numResults);
            }
            if (list == null || list.Count == 0 || !CFunctions.GetViewSetting(Webcmm.Id.News, ViewSetting.ListByCategory))
            {
                CategoryInfo cinfo = DAL.Getinfo(this.Cid);
                if (cinfo == null)
                    cinfo = new CategoryInfo();
                list = new List<CategoryInfo>();
                list.Add(cinfo);
            }

            (new GenericList<CategoryInfo>()).Bind_DataList(rptListByCategory, null, list, 0);
            pnlListByCategory.Visible = list != null && list.Count > 0;
            return;
        }

        private void Load_Info(int iid)
        {
            CNews DAL = new CNews(CCommon.LANG);
            NewsInfo info = DAL.Getinfo(iid);
            if (info != null)
            {
                this.Cid = info.Cid;
                if (info.Allowcomment > 0)
                {
                    CommentInfo comment = new CComment(CCommon.LANG).Getinforating(Webcmm.Id.News, info.Id);
                    if (comment != null)
                    {
                        if ((comment.Viewcounter + 1) != info.Allowcomment)
                        {
                            info.Allowcomment = comment.Viewcounter + 1;
                            DAL.Updatenum(info.Id.ToString(), Queryparam.Sqlcolumn.Allowcomment, info.Allowcomment);
                        }
                        info.Rating = comment.Rating / (comment.Viewcounter == 0 ? 1 : comment.Viewcounter);
                    }
                }
                List<NewsInfo> list = new List<NewsInfo>();
                list.Add(info);
                (new GenericList<NewsInfo>()).Bind_DataList(rptInfo, null, list, 0);
                DAL.Updatenum(iid.ToString(), Queryparam.Sqlcolumn.Viewcounter, CConstants.NUM_INCREASE);

                Master.AddMeta_Title(info.Name);
                Master.AddMeta_Description(info.Introduce);
                Master.AddMeta_Keywords(info.Tag);
            }

            pnlListfollow.Visible = pnlInfo.Visible = info != null;
        }
        private void Bind_rptListfollow()
        {
            int numResults = 0;
            ListOptions options = Get_ListOptions(pagBuilderfollow);
            options.PageIndex = PageIndex;
            List<NewsInfo> list = (new CNews(CCommon.LANG)).Getlist(this.Cid.ToString(), options, out numResults);
            (new GenericList<NewsInfo>(options)).Bind_DataList(rptListfollow, pagBuilderfollow, list, numResults);
            pnlListfollow.Visible = numResults > 0;
            if (numResults > 0)
            {
                NewsInfo info = (NewsInfo)list[0];
                if (info != null)
                    lblNamefollow.Text = info.Cname;
            }
            return;
        }

        private void Bind_rptCategory()
        {
            List<CategoryInfo> list = null;
            CategoryInfo cinfo = (new CCategory(CCommon.LANG)).Getinfo(this.Cid);
            if (cinfo != null)
            {
                list = new List<CategoryInfo>();
                list.Add(cinfo);

                if (!pnlInfo.Visible)
                {
                    Master.AddMeta_Title(cinfo.Name);
                    Master.AddMeta_Description(CFunctions.IsNullOrEmpty(cinfo.Note) ? cinfo.Name : cinfo.Note);
                    Master.AddMeta_Keywords(CFunctions.IsNullOrEmpty(cinfo.Note) ? cinfo.Name : cinfo.Note);
                }
            }
            else
            {
                CategorytypeofInfo tinfo = (new CCategorytypeof(CCommon.LANG)).Getinfo(Webcmm.Id.News);
                if (tinfo != null)
                {
                    cinfo = new CategoryInfo();
                    cinfo.Name = tinfo.Name;
                    list = new List<CategoryInfo>();
                    list.Add(cinfo);

                    if (!pnlInfo.Visible)
                    {
                        Master.AddMeta_Title(tinfo.Name);
                        Master.AddMeta_Description(CFunctions.IsNullOrEmpty(tinfo.Note) ? tinfo.Name : tinfo.Note);
                        Master.AddMeta_Keywords(CFunctions.IsNullOrEmpty(tinfo.Note) ? tinfo.Name : tinfo.Note);
                    }
                }
            }
            (new GenericList<CategoryInfo>()).Bind_DataList(rptCategory, null, list, 0);
            pnlCategory.Visible = list != null && list.Count > 0;
            return;
        }
        #endregion

        #region events
        protected void rptListByCategory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                CategoryInfo info = ((List<CategoryInfo>)rptListByCategory.DataSource)[e.Item.ItemIndex];
                ucnewsl Newsl = (ucnewsl)e.Item.FindControl("Newsl");
                if (Newsl != null)
                    Newsl.Bind_rptList(info.Id, PageIndex);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        #endregion
    }
}
