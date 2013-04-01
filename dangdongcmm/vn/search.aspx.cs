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
    public partial class search : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Cid = CCommon.Get_QueryString(Queryparam.Cid);
                this.Keywords = CCommon.Get_QueryString(Queryparam.Keywords);
                this.SearchIn = CCommon.Get_QueryString("searchin");
                //this.SearchIn = "12,0";
                int pageindex = CCommon.Get_QueryNumber(Queryparam.Pageindex);
                this.Bind_rptList(pageindex);
            }
        }

        #region properties
        public string Keywords
        {
            get
            {
                return ViewState["Keywords"] == null ? "" : ViewState["Keywords"].ToString();
            }
            set
            {
                ViewState["Keywords"] = value;
            }
        }
        public string SearchIn
        {
            get
            {
                return ViewState["SearchIn"] == null ? "" : ViewState["SearchIn"].ToString();
            }
            set
            {
                ViewState["SearchIn"] = value;
            }
        }
        public string Cid
        {
            get
            {
                return ViewState["Cid"] == null ? "" : ViewState["Cid"].ToString();
            }
            set
            {
                ViewState["Cid"] = value;
            }
        }
        #endregion

        #region private methods
        private List<GeneralInfo> Search(out int numResults)
        {
            numResults = 0;
            if (CFunctions.IsNullOrEmpty(SearchIn)) return null;

            List<GeneralInfo> list = null;
            string[] typeofArr = SearchIn.Split(',');
            for (int i = 0; i < typeofArr.Length; i++)
            {
                int typeofId = 0;
                int.TryParse(typeofArr[i], out typeofId);
                if (typeofId == 0) continue;
                CategorytypeofInfo typeofInfo = (new CCategorytypeof(CCommon.LANG)).Getinfo(typeofId);
                if (typeofInfo == null) continue;

                int numResult = 0;
                List<GeneralInfo> listin = null;
                try
                {
                    listin = (new CGeneral(CCommon.LANG, typeofInfo.Id)).Search(typeofInfo.Id, typeofInfo.Name, Cid, Keywords, Get_ListOptionsNoPaging(), out numResult);
                    if (listin == null) continue;
                }
                catch
                {
                    continue;
                }

                if (list == null)
                    list = new List<GeneralInfo>();
                list.AddRange(listin);
                numResults += numResult;
            }
            List<GeneralInfo> listR = new List<GeneralInfo>();
            foreach (var info in list)
            {
                if (info.Cid == 61 || info.Cid == 67 || info.Cid == 62 || info.Cid == 63)
                {
                    info.isDictionary = 1;
                    listR.Add(info);
                }
            }
            foreach (var info in list)
            {
                if (info.Cid != 61 && info.Cid != 67 && info.Cid != 62 && info.Cid != 63)
                {
                    listR.Add(info);
                }
            }
            return listR;
        }
        private void Bind_rptList(int pageindex)
        {
            if (CFunctions.IsNullOrEmpty(Keywords)) return;

            int numResults = 0;
            ListOptions options = Get_ListOptions(pagBuilder);
            options.PageIndex = PageIndex = pageindex;
            List<GeneralInfo> list = this.Search(out numResults);
            (new GenericList<GeneralInfo>(PageIndex, PageSize, SortExp, SortDir)).Bind_DataListNoSort(rptList, pagBuilder, list);
            
            pagBuilder.Visible = !(list == null || list.Count == 0);
            this.Synch_Pager(pagBuilder, pagBuilderT);

            if (!Page.IsPostBack)
                lblCname.Text += "<u><i><b>" + Keywords + "</b></i></u>: " + numResults;
            return;
        }
        public void Synch_Pager(ucpager pagerS, ucpager pagerD)
        {
            if (pagerS == null || pagerD == null) return;

            pagerD.Visible = pagerS.Visible;
            pagerD.PageSize = pagerS.PageSize;
            pagerD.RecordCount = pagerS.RecordCount;
            pagerD.PageIndex = pagerS.PageIndex;
            return;
        }
        #endregion

    }
}
