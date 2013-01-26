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
using System.IO;
using OfficeOpenXml;

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm.cmm
{
    public partial class report_lib : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Init_State();
                //this.Bind_grdView();
            }
        }
        
        #region private methods
        private void Init_State()
        {
            this.BindDDL_Cid(ddlCid);
        }

        private List<LibrariesInfo> Search(out int numResults)
        {
            SearchInfo isearch = new SearchInfo();
            isearch.Cid = CFunctions.IsNullOrEmpty(ddlCid.SelectedValue) ? 0 : int.Parse(ddlCid.SelectedValue);
            isearch.Datefr = CFunctions.IsNullOrEmpty(txtDatefr.Text.Trim()) ? CFunctions.Get_Datetime(CFunctions.Set_Datetime(DateTime.Now)) : CFunctions.Get_Datetime(txtDatefr.Text.Trim());
            isearch.Datefr = new DateTime(isearch.Datefr.Year, isearch.Datefr.Month, isearch.Datefr.Day, 0, 0, 0);
            isearch.Dateto = CFunctions.IsNullOrEmpty(txtDateto.Text.Trim()) ? CFunctions.Get_Datetime(CFunctions.Set_Datetime(DateTime.Now)) : CFunctions.Get_Datetime(txtDateto.Text.Trim());
            isearch.Dateto = new DateTime(isearch.Dateto.Year, isearch.Dateto.Month, isearch.Dateto.Day, 23, 59, 59);

            ListOptions options = Get_ListOptions();
            if (!CFunctions.IsNullOrEmpty(txtPagesize.Text))
                options.PageSize = int.Parse(txtPagesize.Text);
            options.SortExp = radReporttype.SelectedValue;

            numResults = 0;
            List<LibrariesInfo> list = (new CLibraries(CCommon.LANG)).Wcmm_Report(isearch, options, out numResults);
            return list;
        }
        public override void Bind_grdView()
        {
            int numResults = 0;
            List<LibrariesInfo> list = this.Search(out numResults);
            (new GenericList<LibrariesInfo>(PageIndex, PageSize, SortExp, SortDir)).Bind_GridView(grdView, pagBuilder, list, numResults);
            return;
        }

        private void BindDDL_Cid(DropDownList ddl)
        {
            this.BindDDL_Cid(ddl, 0, "");
            CCommon.Insert_FirstitemDDL(ddl);
        }
        private void BindDDL_Cid(DropDownList ddl, int pid, string separator)
        {
            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Wcmm_Getlist(Webcmm.Id.Libraries, pid, Get_ListOptionsNoPaging());
            if (list == null) return;

            foreach (CategoryInfo info in list)
            {
                string sep = pid == 0 ? "" : separator + "---";
                ListItem item = new ListItem(sep + info.Name, info.Id.ToString());
                ddl.Items.Add(item);
                this.BindDDL_Cid(ddl, info.Id, sep);
            }
        }
        #endregion

        #region events
        protected void grdView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                GridView gridView = (GridView)sender;
                BindData_Sorting(gridView, e);
                BuildCommand_Delete(e);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void grdView_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                SortExp = e.SortExpression;
                SortDir = SortDir == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
                this.Bind_grdView();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            try
            {
                PageIndex = 1;
                this.Bind_grdView();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdExport_Click(object sender, EventArgs e)
        {
            try
            {
                int numResults = 0;
                List<LibrariesInfo> list = this.Search(out numResults);
                if (list != null && list.Count > 0)
                {
                    FileInfo exTemplate = new FileInfo(Server.MapPath(@"~/xhtml/" + CCommon.LANG + "/ReportLib.xlsx"));
                    string filePath = @"~/commup/manual/ReportLib_" + DateTime.Now.Ticks.ToString() + ".xlsx";
                    FileInfo exNewfile = new FileInfo(Server.MapPath(filePath));
                    using (ExcelPackage exPackage = new ExcelPackage(exNewfile, exTemplate))
                    {
                        ExcelWorksheet exWorksheet = exPackage.Workbook.Worksheets[1];

                        for (int i = 0; i < list.Count; i++)
                        {
                            LibrariesInfo info = (LibrariesInfo)list[i];
                            exWorksheet.Cell(i + 2, 1).Value = (i + 1).ToString();
                            exWorksheet.Cell(i + 2, 2).Value = info.Name;
                            exWorksheet.Cell(i + 2, 2).Hyperlink = new Uri(CConstants.WEBSITE + "/" + CCommon.LANG + "/" + info.eUrl, UriKind.Absolute);
                            exWorksheet.Cell(i + 2, 3).Value = info.Introduce;
                            exWorksheet.Cell(i + 2, 4).Value = info.Allowcomment.ToString();
                            exWorksheet.Cell(i + 2, 5).Value = info.Viewcounter.ToString();
                            exWorksheet.Cell(i + 2, 6).Value = info.eTimeupdate;
                        }

                        exPackage.Workbook.Properties.Title = "Report Lib";
                        exPackage.Save();
                    }

                    lnkExport.Visible = true;
                    lnkExport.Text = "Download";
                    lnkExport.NavigateUrl = filePath;
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        #endregion
    }
}
