using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using dangdongcmm.model;

namespace dangdongcmm
{
    public class GenericList<I>
    {
        public GenericList()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public GenericList(int pageindex, int pagesize, string sortexp, string sortdir)
        {
            //
            // TODO: Add constructor logic here
            //
            this.PageIndex = pageindex;
            this.PageSize = pagesize;
            this.SortExp = sortexp;
            this.SortDir = this.parseSortDir(sortdir);
        }
        public GenericList(ListOptions options)
        {
            if (options != null)
            {
                this.PageIndex = options.PageIndex;
                this.PageSize = options.PageSize;
                this.SortExp = options.SortExp;
                this.SortDir = options.SortDir == SortDirection.Ascending.ToString() ? SortDirection.Ascending : SortDirection.Descending;
            }
        }

        private int PageIndex, PageSize;
        private string SortExp;
        private SortDirection SortDir;

        private List<I> FillInRange(List<I> allList, int pageIndex, int pageSize)
        {
            if (allList == null || allList.Count == 0) return null;

            int count = allList.Count;
            int start = (pageIndex - 1) * pageSize;
            int lsize = count - (pageIndex * pageSize) > 0 ? pageSize : count - start;
            List<I> itemsInRange = allList.GetRange(start, lsize);
            return itemsInRange;
        }
        
        public void Bind_GridView(GridView grdView, ucpager pagerBuilder, List<I> allList, int numResults)
        {
            if (allList == null)
                allList = new List<I>();

            if (pagerBuilder != null)
            {
                pagerBuilder.Visible = pagerBuilder.Visible ? (numResults > PageSize) : false;
                pagerBuilder.PageSize = PageSize;
                pagerBuilder.RecordCount = numResults;
                pagerBuilder.PageIndex = PageIndex;
            }

            grdView.DataSource = allList;
            grdView.DataBind();
            return;
        }
        public void Bind_GridView(GridView grdView, ucpager pagerBuilder, List<I> allList)
        {
            if (allList == null)
                allList = new List<I>();

            if (pagerBuilder != null)
            {
                pagerBuilder.Visible = pagerBuilder.Visible ? (allList.Count > PageSize) : false;
                pagerBuilder.PageSize = PageSize;
                pagerBuilder.RecordCount = allList.Count;
                pagerBuilder.PageIndex = PageIndex;
            }

            allList.Sort(new GenericComparer<I>(SortExp, SortDir));
            List<I> pagList = this.FillInRange(allList, PageIndex, PageSize);
            grdView.DataSource = pagList;
            grdView.DataBind();

            return;
        }
        
        public void Bind_DataList(DataList dtlList, ucpager pagerBuilder, List<I> allList, int numResults)
        {
            if (allList == null)
                allList = new List<I>();

            if (pagerBuilder != null)
            {
                pagerBuilder.Visible = pagerBuilder.Visible ? (numResults > PageSize) : false;
                pagerBuilder.PageSize = PageSize;
                pagerBuilder.RecordCount = numResults;
                pagerBuilder.PageIndex = PageIndex;
            }

            dtlList.DataSource = allList;
            dtlList.DataBind();
            return;
        }
        public void Bind_DataList(DataList dtlList, ucpager pagerBuilder, List<I> allList)
        {
            if (allList == null)
                allList = new List<I>();

            if (pagerBuilder != null)
            {
                pagerBuilder.Visible = pagerBuilder.Visible ? (allList.Count > PageSize) : false;
                pagerBuilder.PageSize = PageSize;
                pagerBuilder.RecordCount = allList.Count;
                pagerBuilder.PageIndex = PageIndex;
            }

            allList.Sort(new GenericComparer<I>(SortExp, SortDir));
            List<I> pagList = this.FillInRange(allList, PageIndex, PageSize);
            dtlList.DataSource = pagList;
            dtlList.DataBind();

            return;
        }

        public void Bind_DataList(Repeater rptList, ucpager pagerBuilder, List<I> allList, int numResults)
        {
            if (allList == null)
                allList = new List<I>();

            if (pagerBuilder != null)
            {
                pagerBuilder.Visible = pagerBuilder.Visible ? (numResults > PageSize) : false;
                pagerBuilder.PageSize = PageSize;
                pagerBuilder.RecordCount = numResults;
                pagerBuilder.PageIndex = PageIndex;
            }

            rptList.DataSource = allList;
            rptList.DataBind();
            return;
        }
        public void Bind_DataList(Repeater rptList, ucpager pagerBuilder, List<I> allList)
        {
            if (allList == null)
                allList = new List<I>();

            if (pagerBuilder != null)
            {
                pagerBuilder.Visible = pagerBuilder.Visible ? (allList.Count > PageSize) : false;
                pagerBuilder.PageSize = PageSize;
                pagerBuilder.RecordCount = allList.Count;
                pagerBuilder.PageIndex = PageIndex;
            }

            allList.Sort(new GenericComparer<I>(SortExp, SortDir));
            List<I> pagList = this.FillInRange(allList, PageIndex, PageSize);
            rptList.DataSource = pagList;
            rptList.DataBind();

            return;
        }

        private SortDirection parseSortDir(string sortdir)
        {
            return sortdir == SortDirection.Ascending.ToString() ? SortDirection.Ascending : SortDirection.Descending;
        }
    }
}
