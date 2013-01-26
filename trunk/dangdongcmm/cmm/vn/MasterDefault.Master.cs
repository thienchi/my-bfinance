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
    public partial class MasterDefault : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Show_Login();
            if (!Page.IsPostBack)
            {
                ltrMenumain.Text = this.Build_Menu("MENUCMM");
                this.BindDDL_Lang(ddlLang);
            }
        }

        #region properties
        public string LANG
        {
            get
            {
                return CCommon.LANG;
            }
            set
            {
                CCommon.LANG = value;
            }
        }
        #endregion

        #region private methods
        private void Show_Login()
        {
            if (Session[Sessionparam.USERLOGIN] != null)
            {
                UserInfo info = (UserInfo)Session[Sessionparam.USERLOGIN];
                if (info != null)
                {
                    lblLoginusername.Text = info.Username;
                }
            }
        }
        
        private void BindDDL_Lang(DropDownList ddl)
        {
            try
            {
                int lang_num = Convert.ToInt32(ConfigurationSettings.AppSettings["LANG_NUM"]);
                string lang_str = ConfigurationSettings.AppSettings["LANG_TXT"];
                string[] lang_arr = lang_str.Split(Convert.ToChar("$"));
                if (lang_arr == null || lang_arr.Length == 0 || lang_arr.Length != lang_num)
                {
                    ddl.Visible = false;
                    return;
                }

                ddl.Visible = lang_num > 1;
                for (int i = 0; i < lang_num; i++)
                {
                    string lang_val = ConfigurationSettings.AppSettings["LANG_" + i];
                    string lang_txt = lang_arr[i];
                    ListItem item = new ListItem("-- " + lang_txt, lang_val);
                    ddl.Items.Add(item);
                }

                ListItem itemc = ddl.Items.FindByValue(LANG);
                if (itemc != null)
                    ddl.SelectedValue = itemc.Value;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool UserIsAdmin()
        {
            try
            {
                if (Session[Sessionparam.USERLOGIN] == null) return false;
                UserInfo info = (UserInfo)Session[Sessionparam.USERLOGIN];
                if (info == null || info.iRight == null) return false;

                return !CFunctions.IsNullOrEmpty(info.iRight.R_sys);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region build menu
        private string SetofCatalogue = "";
        public string Build_Menu(string code)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(code)) return "";
                MenutypeofInfo info_typeof = (new CMenutypeof(LANG)).Wcmm_Getinfo(code);
                if (info_typeof == null) return "";

                int cid = info_typeof.Id;
                StringBuilder write = new StringBuilder();
                SetofCatalogue = (new CMenu(LANG)).Get_SetofCatalogue(cid);
                SetofCatalogue = "," + SetofCatalogue;
                write.Append("<ul class=\"horizontal\">");
                this.Build_Menunode(write, cid, 0, info_typeof.Insertbreak == 1, 0);
                write.Append("</ul>");
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
                List<MenuInfo> list = (new CMenu(LANG)).Getlist(cid, pid, this.Get_ListOptionsMenu(), out numResults);
                if (list == null) return;

                for (int i = 0; i < list.Count; i++)
                {
                    MenuInfo info = (MenuInfo)list[i];
                    if (info.Cataloguetypeofid == 0)
                    {
                        if ((info.Attributes.Equals("ADMIN") || info.Attributes.Equals("SYSTEM")) && !this.UserIsAdmin()) continue;
                        this.Build_Menunode_noncat(write, cid, info, insertbreak, level);
                        if (insertbreak && info.Visible != 0 && (i != list.Count - 1))
                            this.Write_Break(write, level + 1);
                    }
                    else
                    {
                        if (info.Catalogueid == 0)
                        {
                            this.Build_Menunode_cat(write, info, info.Catalogueid, insertbreak, level);
                            if (insertbreak && info.Visible != 0 && (i != list.Count - 1))
                                this.Write_Break(write, level + 1);
                        }
                        else
                        {
                            this.Build_Menunode_catin(write, info, info.Catalogueid, insertbreak, level);
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
        private void Build_Menunode_noncat(StringBuilder write, int cid, MenuInfo menuinfo, bool insertbreak, int level)
        {
            try
            {
                level++;
                write.Append("<li" + (menuinfo.Visible == 0 ? " class=\"hide\"" : "") + ">");
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
        private void Build_Menunode_cat(StringBuilder write, MenuInfo menuinfo, int catalogueid, bool insertbreak, int level)
        {
            try
            {
                bool visibled = menuinfo.Visible == 1;

                int numResults = 0;
                List<CategoryInfo> list = (new CCategory(LANG)).Getlist(menuinfo.Cataloguetypeofid, catalogueid, this.Get_ListOptionsMenu(), out numResults);
                if (list == null) return;

                level++;
                for (int i = 0; i < list.Count; i++)
                {
                    CategoryInfo info = (CategoryInfo)list[i];
                    if (SetofCatalogue != "" && SetofCatalogue.IndexOf("," + info.Id + ",") != -1) continue;

                    write.Append("<li" + (menuinfo.Visible == 0 ? " class=\"hide\"" : "") + ">");
                    this.Write_Menuitem(write, menuinfo, info);

                    if (menuinfo.Insertcatalogue == 1 || info.Pis > 0)
                    {
                        write.Append("<ul class=\"vertical\">");
                        if (menuinfo.Insertcatalogue == 1)
                            this.Build_Menunode_catitem(write, menuinfo, info.Id, insertbreak, level);
                        if (info.Pis > 0)
                            this.Build_Menunode_cat(write, menuinfo, info.Id, insertbreak, level);
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
        private void Build_Menunode_catin(StringBuilder write, MenuInfo menuinfo, int catalogueid, bool insertbreak, int level)
        {
            try
            {
                bool noroot = menuinfo.Noroot == 1;

                CategoryInfo info = (new CCategory(LANG)).Getinfo(catalogueid);
                if (info == null) return;

                write.Append("<li" + (menuinfo.Visible == 0 ? " class=\"hide\"" : "") + ">");
                if (!noroot)
                {
                    level++;
                    this.Write_Menuitem(write, menuinfo, info);

                    if (menuinfo.Insertcatalogue == 1 || info.Pis > 0)
                    {
                        write.Append("<ul class=\"vertical\">");
                        if (menuinfo.Insertcatalogue == 1)
                            this.Build_Menunode_catitem(write, menuinfo, info.Id, insertbreak, level);
                        if (info.Pis > 0)
                            this.Build_Menunode_cat(write, menuinfo, info.Id, insertbreak, level);
                        write.Append("</ul>");
                    }
                }
                else
                {
                    if (menuinfo.Insertcatalogue == 1 || info.Pis > 0)
                    {
                        write.Append("<ul class=\"vertical\">");
                        if (menuinfo.Insertcatalogue == 1)
                            this.Build_Menunode_catitem(write, menuinfo, info.Id, insertbreak, level);
                        if (info.Pis > 0)
                            this.Build_Menunode_cat(write, menuinfo, info.Id, insertbreak, level);
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
        private void Build_Menunode_catitem(StringBuilder write, MenuInfo menuinfo, int catalogueid, bool insertbreak, int level)
        {
            try
            {
                bool visibled = menuinfo.Visible == 1;

                level++;
                List<GeneralInfo> list = (new CGeneral(LANG, menuinfo.Cataloguetypeofid)).Wcmm_Getlist_buildmenu(catalogueid, this.Get_ListOptionsMenu());
                if (list == null) return;

                for (int i = 0; i < list.Count; i++)
                {
                    GeneralInfo info = (GeneralInfo)list[i];

                    write.Append("<li" + (menuinfo.Visible == 0 ? " class=\"hide\"" : "") + ">");
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
            write.Append("<span class=\"wrap" + (info.Pis > 0 ? " havesub" : "") + "\"><a href=\"" + (!CFunctions.IsNullOrEmpty(info.Url) ? info.Url : this.install_url(menuinfo.Navigateurl, menuinfo.Cataloguetypeofid, info.Id, 0, info.Name)) + "\">" + info.Name + "</a></span>");
        }
        private void Write_Break(StringBuilder write, int level)
        {
            write.Append("<li class=\"menubreak" + level + "\"></li>");
        }
        private string install_url(string urldef, int cataloguetypeofid, int cid, int iid, string subject)
        {
            string navigateurl = urldef;
            if (CFunctions.IsNullOrEmpty(urldef))
            {
                var urlprefix = "";
                switch (cataloguetypeofid)
                {
                    case Webcmm.Id.News:
                        urlprefix = "n";
                        break;
                    case Webcmm.Id.Product:
                        urlprefix = "p";
                        break;
                }
                navigateurl = urlprefix + cid + "d" + iid + "d=" + CFunctions.install_urlname(subject);
            }
            else if (!CFunctions.IsNullOrEmpty(urldef) && cataloguetypeofid != 0)
            {
                navigateurl += "?cid=" + cid + (iid == 0 ? "" : "&iid=" + iid);
            }
            return navigateurl;
        }
        #endregion

        #region public methods
        public bool Form_ShowError(List<Errorobject> lstError)
        {
            try
            {
                if (lstError == null || lstError.Count == 0)
                {
                    lblError.Text = "";
                    pnlError.CssClass = "FLOUTNOTICE";
                    return true;
                }

                string ErrorMessage = "";
                foreach (Errorobject error in lstError)
                {
                    ErrorMessage += error.ErrorMessage;
                    if (error.Control != null)
                        ((WebControl)error.Control).Attributes.Add("style", "background-color:#ffffdd;");
                    //((WebControl)error.Control).BackColor = System.Drawing.Color.FromArgb(255, 251, 198);
                }
                lblError.Text = ErrorMessage;
                pnlError.CssClass = "FLOUTNOTICESHOW";
                cmdErrorClose.Focus();
                return false;
            }
            catch
            {
                return false;
            }
        }
        public void Form_ClearError()
        {
            lblError.Text = "";
            pnlError.CssClass = "FLOUTNOTICE";
        }

        #endregion

        #region events
        protected void cmdErrorClose_Click(object sender, ImageClickEventArgs e)
        {
            lblError.Text = "";
            pnlError.CssClass = "FLOUTNOTICE";
        }

        protected void ddlLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            string lang_cur = LANG;
            LANG = ddlLang.SelectedValue;
            Response.Redirect(Request.Url.PathAndQuery.Replace("/" + lang_cur + "/", "/" + LANG + "/"));
        }
        #endregion
    }
}
