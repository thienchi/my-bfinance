﻿using System;
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

namespace dangdongcmm
{
    public partial class ucmenu : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ltrMenumain.Text = this.Build_Menu(Code);
            }
        }

        public string Code
        {
            get;
            set;
        }

        #region build menu
        private string SetofCatalogue = "";
        public string Build_Menu(string code)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(code)) return "";
                MenutypeofInfo info_typeof = (new CMenutypeof(CCommon.LANG)).Wcmm_Getinfo(code);
                if (info_typeof == null) return "";

                int cid = info_typeof.Id;
                StringBuilder write = new StringBuilder();
                SetofCatalogue = (new CMenu(CCommon.LANG)).Get_SetofCatalogue(cid);
                SetofCatalogue = "," + SetofCatalogue;
                write.Append("<ul class=\"horizontal\">");
                this.Build_Menunode(write, cid, 0, info_typeof.Insertbreak == 1, 0);
                write.Append("</ul>");
                write = write.Replace("Tu-dien-thuat-ngu.aspx", "tu-dien-thuat-ngu-abc-vn-at-.aspx");
                return write.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ListOptions Get_ListOptionsMenu()
        {
            ListOptions options = new ListOptions();
            options.SortExp = Queryparam.Sqlcolumn.Orderd;
            options.SortDir = SortDirection.Descending.ToString();
            options.GetAll = true;
            return options;
        }
        private void Build_Menunode(StringBuilder write, int cid, int pid, bool insertbreak, int level)
        {
            try
            {
                int numResults = 0;
                List<MenuInfo> list = (new CMenu(CCommon.LANG)).Getlist(cid, pid, this.Get_ListOptionsMenu(), out numResults);
                if (list == null) return;

                for (int i = 0; i < list.Count; i++)
                {
                    MenuInfo info = (MenuInfo)list[i];
                    if (info.Cataloguetypeofid == 0)
                    {
                        this.Build_Menunode_noncat(write, cid, info, insertbreak, level, i);
                        if (insertbreak && info.Visible != 0 && (i != list.Count - 1))
                            this.Write_Break(write, level + 1);
                    }
                    else
                    {
                        if (info.Catalogueid == 0)
                        {
                            this.Build_Menunode_cat(write, info, info.Catalogueid, insertbreak, level, i);
                            if (insertbreak && info.Visible != 0 && (i != list.Count - 1))
                                this.Write_Break(write, level + 1);
                        }
                        else
                        {
                            this.Build_Menunode_catin(write, info, info.Catalogueid, insertbreak, level, i);
                            if (insertbreak && info.Visible != 0 && (i != list.Count - 1))
                                this.Write_Break(write, level + 1);
                        }
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Build_Menunode_noncat(StringBuilder write, int cid, MenuInfo menuinfo, bool insertbreak, int level, int index)
        {
            try
            {
                level++;
                //write.Append("<li" + (menuinfo.Visible == 0 ? " class=\"hide\"" : "") + ">");
                write.Append("<li" + (menuinfo.Visible == 0 || index == 0 ? (" class=\"" + (menuinfo.Visible == 0 ? "hide" : "") + (index == 0 ? " first" : "") + "\"") : ("")) + ">");
                this.Write_Menuitem(write, menuinfo);
                if (menuinfo.Pis > 0)
                {
                    write.Append("<ul class=\"vertical\">");
                    this.Build_Menunode(write, cid, menuinfo.Id, insertbreak, level);
                    write.Append("</ul>");
                }
                write.Append("</li>");
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Build_Menunode_cat(StringBuilder write, MenuInfo menuinfo, int catalogueid, bool insertbreak, int level, int index)
        {
            try
            {
                bool visibled = menuinfo.Visible == 1;

                int numResults = 0;
                List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Getlist(menuinfo.Cataloguetypeofid, catalogueid, this.Get_ListOptionsMenu(), out numResults);
                if (list == null) return;

                level++;
                for (int i = 0; i < list.Count; i++)
                {
                    CategoryInfo info = (CategoryInfo)list[i];
                    if (SetofCatalogue != "" && SetofCatalogue.IndexOf("," + info.Id + ",") != -1) continue;

                    write.Append("<li" + (menuinfo.Visible == 0 || i == 0 ? (" class=\"" + (menuinfo.Visible == 0 ? "hide" : "") + (i == 0 ? " first" : "") + "\"") : ("")) + ">");
                    this.Write_Menuitem(write, menuinfo, info);

                    if (menuinfo.Insertcatalogue == 1 || info.Pis > 0)
                    {
                        write.Append("<ul class=\"vertical\">");
                        if (menuinfo.Insertcatalogue == 1)
                            this.Build_Menunode_catitem(write, menuinfo, info.Id, insertbreak, level, i);
                        if (info.Pis > 0)
                            this.Build_Menunode_cat(write, menuinfo, info.Id, insertbreak, level, i);
                        write.Append("</ul>");
                    }

                    write.Append("</li>");

                    if (insertbreak && visibled && (i != list.Count - 1))
                    {
                        if (!((i + 1 == list.Count - 1) && (SetofCatalogue != "" && SetofCatalogue.IndexOf("," + ((CategoryInfo)list[i + 1]).Id + ",") != -1)))
                            this.Write_Break(write, level);
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Build_Menunode_catin(StringBuilder write, MenuInfo menuinfo, int catalogueid, bool insertbreak, int level, int index)
        {
            try
            {
                bool noroot = menuinfo.Noroot == 1;

                CategoryInfo info = (new CCategory(CCommon.LANG)).Getinfo(catalogueid);
                if (info == null) return;

                write.Append("<li" + (menuinfo.Visible == 0 || index == 0 ? (" class=\"" + (menuinfo.Visible == 0 ? "hide" : "") + (index == 0 ? " first" : "") + "\"") : ("")) + ">");
                if (!noroot)
                {
                    level++;
                    this.Write_Menuitem(write, menuinfo, info);

                    if (menuinfo.Insertcatalogue == 1 || info.Pis > 0)
                    {
                        write.Append("<ul class=\"vertical\">");
                        if (menuinfo.Insertcatalogue == 1)
                            this.Build_Menunode_catitem(write, menuinfo, info.Id, insertbreak, level, index);
                        if (info.Pis > 0)
                            this.Build_Menunode_cat(write, menuinfo, info.Id, insertbreak, level, index);
                        write.Append("</ul>");
                    }
                }
                else
                {
                    if (menuinfo.Insertcatalogue == 1 || info.Pis > 0)
                    {
                        write.Append("<ul class=\"vertical\">");
                        if (menuinfo.Insertcatalogue == 1)
                            this.Build_Menunode_catitem(write, menuinfo, info.Id, insertbreak, level, index);
                        if (info.Pis > 0)
                            this.Build_Menunode_cat(write, menuinfo, info.Id, insertbreak, level, index);
                        write.Append("</ul>");
                    }
                }

                write.Append("</li>");
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Build_Menunode_catitem(StringBuilder write, MenuInfo menuinfo, int catalogueid, bool insertbreak, int level, int index)
        {
            try
            {
                bool visibled = menuinfo.Visible == 1;

                level++;
                List<GeneralInfo> list = (new CGeneral(CCommon.LANG, menuinfo.Cataloguetypeofid)).Wcmm_Getlist_buildmenu(catalogueid, this.Get_ListOptionsMenu());
                if (list == null) return;

                for (int i = 0; i < list.Count; i++)
                {
                    GeneralInfo info = (GeneralInfo)list[i];

                    write.Append("<li" + (menuinfo.Visible == 0 || i == 0 ? (" class=\"" + (menuinfo.Visible == 0 ? "hide" : "") + (i == 0 ? " first" : "") + "\"") : ("")) + ">");
                    write.Append("<span class=\"wrap\"><a href=\"" + this.install_url(menuinfo.Navigateurl, menuinfo.Cataloguetypeofid, catalogueid, info.Id, info.Name) + "\">" + info.Name + "</a></span>");
                    write.Append("</li>");
                    if (insertbreak && visibled && (i != list.Count - 1))
                        this.Write_Break(write, level);
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Write_Menuitem(StringBuilder write, MenuInfo menuinfo)
        {
            write.Append("<span class=\"wrap" + (menuinfo.Pis > 0 ? " havesub" : "") + "\"><a" + (CFunctions.IsNullOrEmpty(menuinfo.Navigateurl) ? "" : (" href=\"" + menuinfo.Navigateurl + "\"")) + ">" + menuinfo.Name + "</a></span>");
        }
        private void Write_Menuitem(StringBuilder write, MenuInfo menuinfo, CategoryInfo info)
        {
            write.Append("<span class=\"wrap" + (info.Pis > 0 ? " havesub" : "") + "\"><a href=\"" + (this.install_url(menuinfo.Navigateurl, menuinfo.Cataloguetypeofid, info.Id, 0, info.Name)) + "\">" + info.Name + "</a></span>");
        }
        private void Write_Break(StringBuilder write, int level)
        {
            write.Append("<li class=\"menubreak" + level + "\"></li>");
        }
        private string install_url(string urldef, int cataloguetypeofid, int cid, int iid, string subject)
        {
            //string navigateurl = urldef;
            //if (CFunctions.IsNullOrEmpty(urldef))
            //{
            //    var urlprefix = "";
            //    switch (cataloguetypeofid)
            //    {
            //        case Webcmm.Id.News:
            //            urlprefix = "n";
            //            break;
            //        case Webcmm.Id.Product:
            //            urlprefix = "p";
            //            break;
            //        case Webcmm.Id.Libraries:
            //            urlprefix = "l";
            //            break;
            //    }
            //    navigateurl = urlprefix + cid + "d" + iid + "d=" + CFunctions.install_urlname(subject);
            //}
            //else if (!CFunctions.IsNullOrEmpty(urldef) && cataloguetypeofid != 0)
            //{
            //    navigateurl += "?cid=" + cid + (iid == 0 ? "" : "&iid=" + iid);
            //}
            string navigateurl = "/"+CFunctions.install_urlname(subject);
            return navigateurl;
        }
        #endregion
    }
}