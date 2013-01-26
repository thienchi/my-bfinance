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
using System.Text;

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm.cmm
{
    public partial class uccategorymulti : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        #region properties
        public int Belongto
        {
            get;
            set;
        }
        public int Pid
        {
            get;
            set;
        }
        public string SelectedId;
        #endregion

        #region build menu
        public void Build_Categorymulti(int belongto, int pid, string categoryid)
        {
            try
            {
                if (belongto == 0) return;

                this.SelectedId = categoryid;
                StringBuilder write = new StringBuilder();
                write.Append("<ul class=\"horizontal\">");
                this.Build_Categorynode(write, belongto, pid, 0);
                write.Append("</ul>");

                ltrCategory.Text = write.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ListOptions Get_ListOptionsforThis()
        {
            ListOptions options = Get_ListOptionsNoPaging();
            options.SortExp = Queryparam.Sqlcolumn.Orderd;
            options.SortDir = SortDirection.Descending.ToString();
            options.GetAll = true;
            return options;
        }
        private void Build_Categorynode(StringBuilder write, int belongto, int pid, int level)
        {
            try
            {
                int numResults = 0;
                List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Getlist(belongto, pid, this.Get_ListOptionsforThis(), out numResults);
                if (list == null || list.Count == 0) return;

                level++;
                foreach (CategoryInfo info in list)
                {
                    write.Append("<li>");
                    this.Write_Categoryitem(write, info);

                    if (info.Pis > 0)
                    {
                        write.Append("<ul class=\"vertical\">");
                        this.Build_Categorynode(write, info.Cid, info.Id, level);
                        write.Append("</ul>");
                    }

                    write.Append("</li>");
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Write_Categoryitem(StringBuilder write, CategoryInfo info)
        {
            write.Append("<span class=\"wrap" + (info.Depth == 1 ? " havesub" : "") + "\"><input type=\"checkbox\" name=\"chkcategorymulti\" id=\"chkcategorymulti" + info.Id + "\" value=\"" + info.Id + "\" " + this.Write_Checked(info.Id) + " /> <label for=\"chkcategorymulti" + info.Id + "\">" + info.Name + "</label></span>");
        }
        private void Write_Break(StringBuilder write, int level)
        {
            write.Append("<li class=\"menubreak" + level + "\"></li>");
        }
        private string Write_Checked(int categoryid)
        {
            if (("," + this.SelectedId + ",").IndexOf("," + categoryid.ToString() + ",") != -1)
                return "checked=\"checked\"";
            else
                return "";
        }

        #endregion
    }
}