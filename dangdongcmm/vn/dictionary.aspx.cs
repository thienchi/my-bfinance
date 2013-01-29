﻿using System;
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
    public partial class dictionary : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
                int iid = CCommon.Get_QueryNumber(Queryparam.Iid);
                this.Load_Info(iid);
                this.Bind_Categoryinfo();

                if (pnlInfo.Visible)
                {
                    if (CFunctions.GetViewSetting(Webcmm.Id.News, ViewSetting.ListFollowed))
                        this.Bind_rptListfollow(cid);
                    else
                        pnlListfollow.Visible = false;
                }
                else
                {
                    this.Bind_rptList();
                }

            }
        }

        #region properties
        public int Cidroot
        {
            get
            {
                return int.Parse(hidCidroot.Value);
            }
        }
        public string Langview
        {
            get
            {
                return CCommon.Get_QueryString("ll");
            }
        }
        public string Typeview
        {
            get
            {
                return CCommon.Get_QueryString("lb");
            }
        }
        #endregion

        #region private methods
        private void Load_Info(int iid)
        {
            CNews DAL = new CNews(CCommon.LANG);
            NewsInfo info = DAL.Getinfo(iid);
            if (info != null)
            {
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
        private void Bind_rptListfollow(int cid)
        {
            int numResults = 0;
            ListOptions options = Get_ListOptions(pagBuilderfollow);
            options.PageIndex = PageIndex;
            List<NewsInfo> list = (new CNews(CCommon.LANG)).Getlist(cid.ToString(), options, out numResults);
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
        private void Bind_rptList()
        {
            string listby = CCommon.Get_QueryString("lb"); // abc, category
            string listkey = CCommon.Get_QueryString("lk"); // a/b/c..., category
            string listlang = CCommon.Get_QueryString("ll"); // vn, en
            
            int numResults = 0;
            ListOptions options = Get_ListOptions(pagBuilder);
            options.PageIndex = PageIndex;
            List<NewsInfo> list = (new CNews(CCommon.LANG)).Getlistdic(this.Cidroot, listby, listkey, listlang, options, out numResults);
            (new GenericList<NewsInfo>(options)).Bind_DataList(rptList, pagBuilder, list, numResults);
            pnlList.Visible = numResults > 0;
            return;
        }
        
        private void Bind_Categoryinfo()
        {
            CategoryInfo cinfo = (new CCategory(CCommon.LANG)).Getinfo(this.Cidroot);
            if (!pnlInfo.Visible && cinfo != null)
            {
                Master.AddMeta_Title(cinfo.Name);
                Master.AddMeta_Description(CFunctions.IsNullOrEmpty(cinfo.Note) ? cinfo.Name : cinfo.Note);
                Master.AddMeta_Keywords(CFunctions.IsNullOrEmpty(cinfo.Note) ? cinfo.Name : cinfo.Note);
            }
            lblCidrootname.Text = cinfo == null ? "" : cinfo.Name;
                    
            return;
        }
        #endregion

        #region events
        #endregion
    }
}