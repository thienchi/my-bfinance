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
using System.Xml;

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm.cmm
{
    public partial class menul : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PARENT = this.Get_Parent();
            if (!Page.IsPostBack)
            {
                this.Init_State();
                this.Bind_grdView();
            }
        }
        private MenuInfo PARENT = null;

        #region private methods
        private MenuInfo Get_Parent()
        {
            int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
            MenuInfo parent = (new CMenu(CCommon.LANG)).Wcmm_Getinfo(pid);
            return parent;
        }
        private string Generate_Parentpath(int pid, int cid)
        {
            string vlreturn = "";
            List<MenuInfo> list_parent = (new CMenu(CCommon.LANG)).Wcmm_Getlist_parent(pid);
            if (list_parent != null)
            {
                for (int i = 0; i < list_parent.Count; i++)
                {
                    MenuInfo info = list_parent[i];
                    vlreturn += " >> <a href='menul.aspx?cid=" + info.Cid + "&pid=" + info.Id + "'>" + info.Name + "</a>";
                    cid = info.Cid;
                }
            }
            MenutypeofInfo infotypeof = (new CMenutypeof(CCommon.LANG)).Wcmm_Getinfo(cid);
            vlreturn = (infotypeof == null ? "" : " <a href='menul.aspx?cid=" + infotypeof.Id + "'>" + infotypeof.Name + "</a>") + vlreturn;
            return vlreturn;
        }
        private void Init_State()
        {
            int pid = PARENT == null ? 0 : PARENT.Id;
            int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
            string parentpath = this.Generate_Parentpath(pid, cid);
            if (!CFunctions.IsNullOrEmpty(parentpath))
            {
                string notice = CCommon.Get_Definephrase(Definephrase.Display_havesub);
                notice = notice.Replace(Queryparam.Varstring.Depth, (PARENT == null ? 1 : PARENT.Depth + 1).ToString());
                notice = notice.Replace(Queryparam.Varstring.Path, parentpath);
                lblError.Text = notice;
            }

            cmdAdd.Attributes.Add("onclick", "CC_gotoUrl('menuu.aspx?cid=" + cid + "&pid=" + pid + "')");
            this.BindDDL_Cid(ddlCid);
            if (ddlCid.Items.FindByValue(cid.ToString()) != null)
                ddlCid.SelectedValue = cid.ToString();
        }

        private List<MenuInfo> Search(out int numResults)
        {
            numResults = 0;
            int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
            string keywords = txtKeyword.Text.Trim();
            List<MenuInfo> list = (new CMenu(CCommon.LANG)).Wcmm_Search(cid, keywords, Get_ListOptions(), out numResults);
            return list;
        }
        private List<MenuInfo> Load_List(out int numResults)
        {
            numResults = 0;
            int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
            int pid = CCommon.Get_QueryNumber(Queryparam.Pid);
            List<MenuInfo> list = (new CMenu(CCommon.LANG)).Wcmm_Getlist(cid, pid, Get_ListOptions(), out numResults);
            return list;
        }
        public override void Bind_grdView()
        {
            int numResults = 0;
            List<MenuInfo> list = CFunctions.IsNullOrEmpty(txtKeyword.Text.Trim()) ? this.Load_List(out numResults) : this.Search(out numResults);
            (new GenericList<MenuInfo>(PageIndex, PageSize, SortExp, SortDir)).Bind_GridView(grdView, pagBuilder, list, numResults);
            return;
        }

        private void BindDDL_Cid(DropDownList ddl)
        {
            List<MenutypeofInfo> list = (new CMenutypeof(CCommon.LANG)).Wcmm_Getlist((int)CConstants.State.Status.Actived);
            if (list == null || list.Count == 0) return;

            ddl.DataSource = list;
            ddl.DataTextField = "Name";
            ddl.DataValueField = "Id";
            ddl.DataBind();
            CCommon.Insert_FirstitemDDL(ddl);
        }
        private void BindDDL_Pid(DropDownList ddl, int cid, int pid, string separator)
        {
            List<MenuInfo> list = (new CMenu(CCommon.LANG)).Wcmm_Getlist(cid, pid);
            if (list == null) return;

            foreach (MenuInfo info in list)
            {
                string sep = pid == 0 ? "" : separator + "---";
                ListItem item = new ListItem(sep + info.Name, info.Id.ToString());
                ddl.Items.Add(item);
                this.BindDDL_Pid(ddl, cid, info.Id, sep);
            }
        }

        private void Copy_Info(MenuInfo source_info, int dest_cid, int dest_pid, int dest_depth, bool getchild, CMenu BLL)
        {
            if (source_info == null) return;

            MenuInfo info_copy = source_info.copy();
            info_copy.Id = 0;
            info_copy.Cid = dest_cid;
            info_copy.Pid = dest_pid;
            info_copy.Depth = dest_depth + 1;
            info_copy.Status = CCommon.GetStatus_upt();
            info_copy.Username = CCommon.Get_CurrentUsername();
            info_copy.Timeupdate = DateTime.Now;
            info_copy.Orderd = 0;
            info_copy.Pis = getchild ? info_copy.Pis : (info_copy.Pis > 0 ? 1 : 0);
            if (BLL.Save(info_copy))
            {
                //BLL.Updatenum(dest_pid.ToString(), Queryparam.Sqlcolumn.Pis, 1);
                if (getchild)
                {
                    List<MenuInfo> listsub = BLL.Wcmm_Getlist(source_info.Cid, source_info.Id);
                    if (listsub == null || listsub.Count == 0) return;
                    foreach (MenuInfo info_sub in listsub)
                    {
                        this.Copy_Info(info_sub, dest_cid, info_copy.Id, info_copy.Depth, getchild, BLL);
                    }
                }
            }
        }
        private void Move_Info(MenuInfo source_info, int dest_cid, int dest_pid, int dest_depth, bool getchild, CMenu BLL)
        {
            if (source_info == null) return;
            int offset_depth = source_info.Depth - (dest_depth + 1);

            MenuInfo info_copy = source_info.copy();
            info_copy.Cid = dest_cid;
            info_copy.Pid = dest_pid;
            info_copy.Depth = dest_depth + 1;
            info_copy.Status = CCommon.GetStatus_upt();
            info_copy.Username = CCommon.Get_CurrentUsername();
            info_copy.Timeupdate = DateTime.Now;
            if (BLL.Save(info_copy))
            {
                //BLL.Updatenum(dest_pid.ToString(), Queryparam.Sqlcolumn.Pis, 1);

                List<MenuInfo> listin = null;
                List<MenuInfo> listsub = BLL.Wcmm_Getlist_sub(source_info.Id, listin);
                if (listsub != null && listsub.Count > 0)
                {
                    foreach (MenuInfo info_sub in listsub)
                    {
                        info_sub.Cid = getchild ? dest_cid : source_info.Cid;
                        info_sub.Pid = getchild ? info_sub.Pid : (info_sub.Pid == source_info.Id ? source_info.Pid : info_sub.Pid);
                        info_sub.Depth = getchild ? info_sub.Depth + offset_depth : info_sub.Depth - 1;
                        info_sub.Status = CCommon.GetStatus_upt();
                        info_sub.Username = CCommon.Get_CurrentUsername();
                        info_sub.Timeupdate = DateTime.Now;
                        BLL.Save(info_sub);
                    }
                }
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
        protected void grdView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                string iid = grdView.DataKeys[e.RowIndex].Value.ToString();
                if (CFunctions.IsNullOrEmpty(iid)) return;

                string vlreturn = Delete(Webcmm.Id.Menu, iid);
                lstError = new List<Errorobject>();
                lstError = Form_GetError(lstError, vlreturn == Definephrase.Remove_completed ? Errortype.Completed : Errortype.Error, vlreturn, "", null);
                Master.Form_ShowError(lstError);
                this.Bind_grdView();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void grdView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int iid = 0;
                int.TryParse(e.CommandArgument.ToString(), out iid);
                switch (e.CommandName)
                {
                    case Commandparam.Setupattribute:
                        MenuInfo info = (new CMenu(CCommon.LANG)).Wcmm_Getinfo(iid);
                        if (info != null)
                            Setupattribute.Show_Dialog(Webcmm.Id.Menu, iid.ToString(), Queryparam.Defstring.Nosymbol, info.Status, Queryparam.Defstring.None);
                        break;
                }
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
        protected void cmdDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string iid = CCommon.Get_FormString(Queryparam.Chkcheck);
                if (CFunctions.IsNullOrEmpty(iid)) return;

                string vlreturn = Delete(Webcmm.Id.Menu, iid);
                lstError = new List<Errorobject>();
                lstError = Form_GetError(lstError, vlreturn == Definephrase.Remove_completed ? Errortype.Completed : Errortype.Error, vlreturn, "", null);
                Master.Form_ShowError(lstError);
                this.Bind_grdView();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdCopy_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string iid = CCommon.Get_FormString(Queryparam.Chkcheck);
                if (CFunctions.IsNullOrEmpty(iid))
                {
                    lstError = new List<Errorobject>();
                    lstError = Form_GetError(lstError, Errortype.Warning, Definephrase.Notice_noselecteditem, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }
                Master.Form_ClearError();

                List<MenuInfo> list = (new CMenu(CCommon.LANG)).Wcmm_Getlist(iid);
                if (list == null) return;

                grdCopy.DataSource = list;
                grdCopy.DataBind();
                pnlList.Visible = false;
                pnlCopy.Visible = !pnlList.Visible;
                if (ddlCopycid.Items.Count == 0) 
                    this.BindDDL_Cid(ddlCopycid);

                string iidstr = "";
                foreach (MenuInfo info in list)
                {
                    iidstr += info.Id + ",";
                }
                txtIidstr.Value = iidstr.Remove(iidstr.Length - 1);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdCopyOk_Click(object sender, EventArgs e)
        {
            try
            {
                string iidstr = txtIidstr.Value;
                int dest_cid = int.Parse(ddlCopycid.SelectedValue);
                int dest_pid = int.Parse(ddlCopypid.SelectedValue);
                bool getchild = chkCopyoption_getchild.Checked;

                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(iidstr) || dest_cid == 0)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Copy_error, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                CMenu BLL = new CMenu(CCommon.LANG);
                MenuInfo parent_info = BLL.Wcmm_Getinfo(dest_pid);
                int dest_depth = parent_info == null ? 1 : parent_info.Depth;
                string[] iidarr = iidstr.Split(',');
                bool isDup = false;
                for (int i = 0; i < iidarr.Length; i++)
                {
                    MenuInfo info = BLL.Wcmm_Getinfo(int.Parse(iidarr[i]));
                    if (info.Id != dest_pid)
                        this.Copy_Info(info, dest_cid, dest_pid, dest_depth, getchild, BLL);
                    else
                        isDup = true;
                }
                if (parent_info != null)
                {
                    int pis = parent_info.Pis == 0 ? iidarr.Length + 1 : iidarr.Length + parent_info.Pis;
                    if (isDup) pis--;
                    BLL.Updatenum(dest_pid.ToString(), Queryparam.Sqlcolumn.Pis, pis);
                }

                cmdCopyOk.Enabled = false;
                lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Copy_completed, "", null);
                Master.Form_ShowError(lstError);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdCopyCancel_Click(object sender, EventArgs e)
        {
            try
            {
                pnlList.Visible = true;
                pnlCopy.Visible = !pnlList.Visible;
                Master.Form_ClearError();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdMove_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string iid = CCommon.Get_FormString(Queryparam.Chkcheck);
                if (CFunctions.IsNullOrEmpty(iid))
                {
                    lstError = new List<Errorobject>();
                    lstError = Form_GetError(lstError, Errortype.Warning, Definephrase.Notice_noselecteditem, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }
                Master.Form_ClearError();

                List<MenuInfo> list = (new CMenu(CCommon.LANG)).Wcmm_Getlist(iid);
                if (list == null) return;

                grdMove.DataSource = list;
                grdMove.DataBind();
                pnlList.Visible = false;
                pnlMove.Visible = !pnlList.Visible;
                if (ddlMovecid.Items.Count == 0) 
                    this.BindDDL_Cid(ddlMovecid);

                string iidstr = "";
                foreach (MenuInfo info in list)
                {
                    iidstr += info.Id + ",";
                }
                txtIidstr.Value = iidstr.Remove(iidstr.Length - 1);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdMoveCancel_Click(object sender, EventArgs e)
        {
            try
            {
                pnlList.Visible = true;
                pnlMove.Visible = !pnlList.Visible;
                Master.Form_ClearError();
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdMoveOk_Click(object sender, EventArgs e)
        {
            try
            {
                string iidstr = txtIidstr.Value;
                int dest_cid = int.Parse(ddlMovecid.SelectedValue);
                int dest_pid = int.Parse(ddlMovepid.SelectedValue);
                bool getchild = chkMoveoption_getchild.Checked;

                lstError = new List<Errorobject>();
                if (CFunctions.IsNullOrEmpty(iidstr) || dest_cid == 0)
                {
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Move_error, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                CMenu BLL = new CMenu(CCommon.LANG);
                MenuInfo parent_info = BLL.Wcmm_Getinfo(dest_pid);
                int dest_depth = parent_info == null ? 1 : parent_info.Depth;
                string[] iidarr = iidstr.Split(',');
                bool isDup = false;
                for (int i = 0; i < iidarr.Length; i++)
                {
                    MenuInfo info = BLL.Wcmm_Getinfo(int.Parse(iidarr[i]));
                    if (info.Id != dest_pid)
                    {
                        if (info.Pid != 0)
                            BLL.Updatenum(info.Pid.ToString(), Queryparam.Sqlcolumn.Pis, CConstants.NUM_DECREASE);
                        this.Move_Info(info, dest_cid, dest_pid, dest_depth, getchild, BLL);
                    }
                    else
                        isDup = true;
                }
                if (parent_info != null)
                {
                    int pis = parent_info.Pis == 0 ? iidarr.Length + 1 : iidarr.Length + parent_info.Pis;
                    if (isDup) pis--;
                    BLL.Updatenum(dest_pid.ToString(), Queryparam.Sqlcolumn.Pis, pis);
                }

                cmdMoveOk.Enabled = false;
                lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Move_completed, "", null);
                Master.Form_ShowError(lstError);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdUpdateorder_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (grdView.Rows.Count == 0) return;

                CMenu BLL = new CMenu(CCommon.LANG);
                foreach (GridViewRow row in grdView.Rows)
                {
                    HiddenField txtId = (HiddenField)row.FindControl("txtId");
                    TextBox txtOrderd = (TextBox)row.FindControl("txtOrderd");
                    if (txtId != null && txtOrderd != null)
                    {
                        int id = int.Parse(txtId.Value);
                        int orderd = 0;
                        int.TryParse(txtOrderd.Text, out orderd);
                        BLL.Updatenum(id.ToString(), Queryparam.Sqlcolumn.Orderd, orderd);
                    }
                    lstError = new List<Errorobject>();
                    lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Saveorderd_completed, "", null);
                    Master.Form_ShowError(lstError);
                }
            }
            catch
            {
                lstError = new List<Errorobject>();
                lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Saveorderd_error, "", null);
                Master.Form_ShowError(lstError);
            }
        }
        protected void cmdSetupattribute_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string iid = CCommon.Get_FormString(Queryparam.Chkcheck);
                if (CFunctions.IsNullOrEmpty(iid)) return;

                if (iid.IndexOf(',') == -1)
                {
                    MenuInfo info = (new CMenu(CCommon.LANG)).Wcmm_Getinfo(int.Parse(iid));
                    if (info != null)
                        Setupattribute.Show_Dialog(Webcmm.Id.Menu, iid, Queryparam.Defstring.Nosymbol, info.Status, Queryparam.Defstring.None);
                }
                else
                    Setupattribute.Show_Dialog(Webcmm.Id.Menu, iid, Queryparam.Defstring.Nosymbol, Queryparam.Defstring.Nospecifyint, Queryparam.Defstring.None);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdBuild_Click(object sender, EventArgs e)
        {
            try
            {
                int cid = CCommon.Get_QueryNumber(Queryparam.Cid);
                int result = this.Build_Menu(cid, CCommon.LANG);
                if (result != 0)
                {
                    lstError = new List<Errorobject>();
                    lstError = Form_GetError(lstError, Errortype.Completed, Definephrase.Save_completed, "", null);
                    Master.Form_ShowError(lstError);
                }
                else
                {
                    lstError = new List<Errorobject>();
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Save_error, "", null);
                    Master.Form_ShowError(lstError);
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }

        #region build menu
        private string SetofCatalogue = "";
        public int Build_Menu(int cid, string lang)
        {
            try
            {
                if (cid == 0) return 0;
                MenutypeofInfo info_typeof = (new CMenutypeof(lang)).Wcmm_Getinfo(cid);
                if (info_typeof == null) return 0;

                string file = Server.MapPath(info_typeof.Path);
                XmlTextWriter write = new XmlTextWriter(file, System.Text.UnicodeEncoding.Unicode);
                if (write == null) return 0;

                write.Formatting = System.Xml.Formatting.Indented;
                write.WriteStartElement("siteMap");

                SetofCatalogue = (new CMenu(lang)).Get_SetofCatalogue(cid);
                SetofCatalogue = "," + SetofCatalogue;
                this.Build_Menunode(write, cid, 0, info_typeof.Insertbreak == 1, 0, lang);

                write.WriteEndElement();
                write.Close();

                return 1;
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

            UserInfo user = CCommon.Get_CurrentUser();
            if (user != null)
            {
                options.Username = user.Username;
                options.GetAll = CCommon.Right_sys();
            }
            return options;
        }
        private void Build_Menunode(XmlTextWriter write, int cid, int pid, bool insertbreak, int level, string lang)
        {
            try
            {
                int numResults = 0;
                List<MenuInfo> list = (new CMenu(lang)).Getlist(cid, pid, this.Get_ListOptionsMenu(), out numResults);
                if (list == null) return;

                for (int i = 0; i < list.Count; i++)
                {
                    MenuInfo info = (MenuInfo)list[i];
                    if (info.Cataloguetypeofid == 0)
                    {
                        this.Build_Menunode_noncat(write, cid, info, insertbreak, level, lang);
                        if (insertbreak && info.Visible != 0 && (i != list.Count - 1))
                            this.Write_Break(write, "BreakItemLook" + (level + 1));
                    }
                    else
                    {
                        if (info.Catalogueid == 0)
                        {
                            this.Build_Menunode_cat(write, info, true, info.Catalogueid, insertbreak, level, lang);
                            if (insertbreak && info.Visible != 0 && (i != list.Count - 1))
                                this.Write_Break(write, "BreakItemLook" + (level + 1));
                        }
                        else
                        {
                            this.Build_Menunode_catin(write, info, true, info.Catalogueid, insertbreak, level, lang);
                            if (insertbreak && info.Visible != 0 && (i != list.Count - 1))
                                this.Write_Break(write, "BreakItemLook" + (level + 1));
                        }
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                write.Close();
                throw ex;
            }
        }
        private void Build_Menunode_noncat(XmlTextWriter write, int cid, MenuInfo info, bool insertbreak, int level, string lang)
        {
            try
            {
                level++;
                write.WriteStartElement("item");
                write.WriteAttributeString("Text", info.Name);
                if (!CFunctions.IsNullOrEmpty(info.Navigateurl))
                    write.WriteAttributeString("NavigateUrl", info.Navigateurl);
                if (!CFunctions.IsNullOrEmpty(info.Tooltip))
                    write.WriteAttributeString("ToolTip", info.Tooltip);
                if (!CFunctions.IsNullOrEmpty(info.Attributes))
                    write.WriteAttributeString(info.Attributes, "");
                if (info.Visible == 0)
                    write.WriteAttributeString("Visible", "false");
                this.Write_LookId(write, info.Pis, level);
                if (info.Pis > 0)
                    this.Build_Menunode(write, cid, info.Id, insertbreak, level, lang);

                if (info.Attributes == "EXTENDCATALOGUE")
                {
                    this.Build_Menunode_catextend(write, info, info.ApplyAttributesChild == 1, insertbreak, level, lang);
                }

                write.WriteEndElement();
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Build_Menunode_cat(XmlTextWriter write, MenuInfo menuinfo, bool applyattributeschild, int catalogueid, bool insertbreak, int level, string lang)
        {
            try
            {
                int cataloguetypeofid = menuinfo.Cataloguetypeofid;
                int insertcatalogue = menuinfo.Insertcatalogue;
                string navigateurl = menuinfo.Navigateurl;
                string attributes = menuinfo.Attributes;
                bool visibled = menuinfo.Visible == 1;

                int numResults = 0;
                List<CategoryInfo> list = (new CCategory(lang)).Getlist(cataloguetypeofid, catalogueid, this.Get_ListOptionsMenu(), out numResults);
                if (list == null) return;

                level++;
                for (int i = 0; i < list.Count; i++)
                {
                    CategoryInfo info = (CategoryInfo)list[i];
                    if (SetofCatalogue != "" && SetofCatalogue.IndexOf("," + info.Id + ",") != -1) continue;

                    write.WriteStartElement("item");
                    write.WriteAttributeString("Text", info.Name);
                    write.WriteAttributeString("NavigateUrl", !CFunctions.IsNullOrEmpty(info.Url) ? info.Url : this.install_url(navigateurl, cataloguetypeofid, info.Id, 0, info.Name, lang));   // CFunctions.IsNullOrEmpty(info.Url) ? navigateurl + "?cid=" + info.Id : info.Url
                    if (!CFunctions.IsNullOrEmpty(attributes) && applyattributeschild)
                        write.WriteAttributeString(attributes, "");
                    if (!visibled)
                        write.WriteAttributeString("Visible", "false");
                    this.Write_LookId(write, info.Pis + insertcatalogue, level);
                    if (insertcatalogue == 1)
                        this.Build_Menunode_catitem(write, menuinfo, menuinfo.ApplyAttributesChild == 1, info.Id, insertbreak, level, lang);
                    this.Build_Menunode_cat(write, menuinfo, menuinfo.ApplyAttributesChild == 1, info.Id, insertbreak, level, lang);
                    write.WriteEndElement();
                    if (insertbreak && visibled && (i != list.Count - 1))
                    {
                        if (!((i + 1 == list.Count - 1) && (SetofCatalogue != "" && SetofCatalogue.IndexOf("," + ((CategoryInfo)list[i + 1]).Id + ",") != -1)))
                            this.Write_Break(write, "BreakItemLook" + level);
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Build_Menunode_catin(XmlTextWriter write, MenuInfo menuinfo, bool applyattributeschild, int catalogueid, bool insertbreak, int level, string lang)
        {
            try
            {
                int cataloguetypeofid = menuinfo.Cataloguetypeofid;
                int insertcatalogue = menuinfo.Insertcatalogue;
                string navigateurl = menuinfo.Navigateurl;
                string attributes = menuinfo.Attributes;
                bool visibled = menuinfo.Visible == 1;
                bool noroot = menuinfo.Noroot == 1;

                CategoryInfo info = (new CCategory(lang)).Getinfo(catalogueid);
                if (info == null) return;

                if (!noroot)
                {
                    level++;
                    write.WriteStartElement("item");
                    write.WriteAttributeString("Text", info.Name);
                    write.WriteAttributeString("NavigateUrl", !CFunctions.IsNullOrEmpty(info.Url) ? info.Url : this.install_url(navigateurl, cataloguetypeofid, info.Id, 0, info.Name, lang));   //CFunctions.IsNullOrEmpty(info.Url) ? navigateurl + "?cid=" + info.Id : info.Url);
                    if (!CFunctions.IsNullOrEmpty(attributes) && applyattributeschild)
                        write.WriteAttributeString(attributes, "");
                    if (!visibled)
                        write.WriteAttributeString("Visible", "false");
                    this.Write_LookId(write, info.Pis + insertcatalogue, level);
                    if (insertcatalogue == 1)
                        this.Build_Menunode_catitem(write, menuinfo, menuinfo.ApplyAttributesChild == 1, info.Id, insertbreak, level, lang);
                }
                else
                {
                    if (insertcatalogue == 1)
                        this.Build_Menunode_catitem(write, menuinfo, menuinfo.ApplyAttributesChild == 1, info.Id, insertbreak, level, lang);
                }
                this.Build_Menunode_cat(write, menuinfo, menuinfo.ApplyAttributesChild == 1, info.Id, insertbreak, level, lang);

                if (!noroot)
                    write.WriteEndElement();
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Build_Menunode_catitem(XmlTextWriter write, MenuInfo menuinfo, bool applyattributeschild, int catalogueid, bool insertbreak, int level, string lang)
        {
            try
            {
                int cataloguetypeofid = menuinfo.Cataloguetypeofid;
                string navigateurl = menuinfo.Navigateurl + "?cid=" + catalogueid;
                string attributes = menuinfo.Attributes;
                bool visibled = menuinfo.Visible == 1;
                
                level++;
                List<GeneralInfo> list = (new CGeneral(lang, cataloguetypeofid)).Wcmm_Getlist_buildmenu(catalogueid, this.Get_ListOptionsMenu());
                if (list == null) return;

                for (int i = 0; i < list.Count; i++)
                {
                    GeneralInfo info = (GeneralInfo)list[i];
                    write.WriteStartElement("item");
                    write.WriteAttributeString("Text", info.Name);
                    write.WriteAttributeString("NavigateUrl", this.install_url(menuinfo.Navigateurl, cataloguetypeofid, catalogueid, info.Id, info.Name, lang));   // navigateurl + "&iid=" + info.Id);
                    if (!CFunctions.IsNullOrEmpty(attributes) && applyattributeschild)
                        write.WriteAttributeString(attributes, "");
                    this.Write_LookId(write, info.Pis, level);
                    write.WriteEndElement();
                    if (insertbreak && visibled && (i != list.Count - 1))
                        this.Write_Break(write, "BreakItemLook" + level);
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Build_Menunode_catextend(XmlTextWriter write, MenuInfo menuinfo, bool applyattributeschild, bool insertbreak, int level, string lang)
        {
            try
            {
                //string attributes = menuinfo.Attributes;
                //bool visibled = menuinfo.Visible == 1;

                //int numResults = 0;
                //List<ExtendattrInfo> list = (new CExtendattr(lang)).Getlist(0, this.Get_ListOptionsMenu(), out numResults);
                //if (list == null) return;

                //level++;
                //for (int i = 0; i < list.Count; i++)
                //{
                //    ExtendattrInfo info = (ExtendattrInfo)list[i];
                    
                //    write.WriteStartElement("item");
                //    write.WriteAttributeString("Text", info.Name);
                //    write.WriteAttributeString("NavigateUrl", "extenditeml.aspx?blto=" + info.Belongto + "&aid=" + info.Id);
                //    if (!CFunctions.IsNullOrEmpty(attributes) && applyattributeschild)
                //        write.WriteAttributeString(attributes, "");
                //    if (!visibled)
                //        write.WriteAttributeString("Visible", "false");
                //    this.Write_LookId(write, 0, level);
                //    write.WriteEndElement();
                //    if (insertbreak && visibled && (i != list.Count - 1))
                //        this.Write_Break(write, "BreakItemLook" + level);
                //}
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        #region old
        /*
        private void Build_Menunode(XmlTextWriter write, int cid, int pid, bool insertbreak, int level, string lang)
        {
            try
            {
                int numResults = 0;
                List<MenuInfo> list = (new CMenu(lang)).Getlist(cid, pid, this.Get_ListOptionsMenu(), out numResults);
                if (list == null) return;

                for (int i = 0; i < list.Count; i++)
                {
                    MenuInfo info = (MenuInfo)list[i];
                    if (info.Cataloguetypeofid == 0)
                    {
                        this.Build_Menunode_noncat(write, cid, info, insertbreak, level, lang);
                        if (insertbreak && info.Visible != 0 && (i != list.Count - 1))
                            this.Write_Break(write, "BreakItemLook" + (level + 1));
                    }
                    else
                    {
                        if (info.Catalogueid == 0)
                        {
                            this.Build_Menunode_cat(write, info.Cataloguetypeofid, info.Catalogueid, info.Insertcatalogue, info.Navigateurl, insertbreak, level, info.Visible == 1, lang);
                            if (insertbreak && info.Visible != 0 && (i != list.Count - 1))
                                this.Write_Break(write, "BreakItemLook" + (level + 1));
                        }
                        else
                        {
                            this.Build_Menunode_catin(write, info.Cataloguetypeofid, info.Catalogueid, info.Insertcatalogue, info.Navigateurl, insertbreak, level, info.Visible == 1, info.Noroot == 1, lang);
                            if (insertbreak && info.Visible != 0 && (i != list.Count - 1))
                                this.Write_Break(write, "BreakItemLook" + (level + 1));
                        }
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                write.Close();
                throw ex;
            }
        }
        private void Build_Menunode_cat(XmlTextWriter write, int cataloguetypeofid, int catalogueid, int insertcatalogue, string navigateurl, bool insertbreak, int level, bool visibled, string lang)
        {
            try
            {
                int numResults = 0;
                List<CategoryInfo> list = (new CCategory(lang)).Getlist(cataloguetypeofid, catalogueid, this.Get_ListOptionsMenu(), out numResults);
                if (list == null) return;

                level++;
                for (int i = 0; i < list.Count; i++)
                {
                    CategoryInfo info = (CategoryInfo)list[i];
                    if (SetofCatalogue != "" && SetofCatalogue.IndexOf("," + info.Id + ",") != -1) continue;

                    write.WriteStartElement("item");
                    write.WriteAttributeString("Text", info.Name);
                    write.WriteAttributeString("NavigateUrl", navigateurl + "?cid=" + info.Id);
                    if (!visibled)
                        write.WriteAttributeString("Visible", "false");
                    this.Write_LookId(write, info.Pis + insertcatalogue, level);
                    if (insertcatalogue == 1)
                        this.Build_Menunode_catitem(write, cataloguetypeofid, info.Id, navigateurl + "?cid=" + info.Id, insertbreak, level, visibled, lang);
                    this.Build_Menunode_cat(write, cataloguetypeofid, info.Id, insertcatalogue, navigateurl, insertbreak, level, visibled, lang);
                    write.WriteEndElement();
                    if (insertbreak && visibled && (i != list.Count - 1))
                        this.Write_Break(write, "BreakItemLook" + level);
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Build_Menunode_catin(XmlTextWriter write, int cataloguetypeofid, int catalogueid, int insertcatalogue, string navigateurl, bool insertbreak, int level, bool visibled, bool noroot, string lang)
        {
            try
            {
                CategoryInfo info = (new CCategory(lang)).Getinfo(catalogueid);
                if (info == null) return;

                if (!noroot)
                {
                    level++;
                    write.WriteStartElement("item");
                    write.WriteAttributeString("Text", info.Name);
                    write.WriteAttributeString("NavigateUrl", navigateurl + "?cid=" + info.Id);
                    if (!visibled)
                        write.WriteAttributeString("Visible", "false");
                    this.Write_LookId(write, info.Pis + insertcatalogue, level);
                    if (insertcatalogue == 1)
                        this.Build_Menunode_catitem(write, cataloguetypeofid, info.Id, navigateurl + "?cid=" + info.Id, insertbreak, level, visibled, lang);
                }
                this.Build_Menunode_cat(write, cataloguetypeofid, info.Id, insertcatalogue, navigateurl, insertbreak, level, visibled, lang);
                if (!noroot) 
                    write.WriteEndElement();
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Build_Menunode_catitem(XmlTextWriter write, int cataloguetypeofid, int catalogueid, string navigateurl, bool insertbreak, int level, bool visibled, string lang)
        {
            try
            {
                level++;
                List<GeneralInfo> list = (new CGeneral(lang, cataloguetypeofid)).Wcmm_Getlist(catalogueid, 0, this.Get_ListOptionsMenu());
                if (list == null) return;

                for (int i = 0; i < list.Count; i++)
                {
                    GeneralInfo info = (GeneralInfo)list[i];
                    write.WriteStartElement("item");
                    write.WriteAttributeString("Text", info.Name);
                    write.WriteAttributeString("NavigateUrl", navigateurl + "&iid=" + info.Id);
                    this.Write_LookId(write, info.Pis, level);
                    write.WriteEndElement();
                    if (insertbreak && visibled && (i != list.Count - 1))
                        this.Write_Break(write, "BreakItemLook" + level);
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        */
        #endregion

        private void Write_Break(XmlTextWriter write, string breakname)
        {
            try
            {
                write.WriteStartElement("item");
                write.WriteAttributeString("LookId", breakname);
                write.WriteEndElement();
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Write_Iconright(XmlTextWriter write, int level)
        {
            try
            {
                if (write == null) return;
                write.WriteAttributeString("Look-RightIconUrl", "arrowr" + level + ".gif");
                write.WriteAttributeString("Look-HoverRightIconUrl", "arrowrh" + level + ".gif");
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Write_LookId(XmlTextWriter write, int pis, int level)
        {
            try
            {
                if (write == null) return;
                write.WriteAttributeString("LookId", "DefaultItemLook" + level);
                if (pis == 0)
                {
                    write.WriteAttributeString("SelectedLookId", "SelectedItemLook" + level);
                }
                else
                {
                    write.WriteAttributeString("SelectedLookId", "ChildSelectedItemLook" + level);
                    write.WriteAttributeString("ChildSelectedLookId", "ChildSelectedItemLook" + level);
                    this.Write_Iconright(write, level);
                }

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string install_url(string urldef, int cataloguetypeofid, int cid, int iid, string subject, string lang)
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

        protected void ddlCopycid_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int cid = int.Parse(ddlCopycid.SelectedValue);
                ddlCopypid.Items.Clear();
                cmdCopyOk.Enabled = cid != 0;
                if (cid == 0) return;

                this.BindDDL_Pid(ddlCopypid, cid, 0, "");
                CCommon.Insert_FirstitemDDL(ddlCopypid);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void ddlMovecid_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int cid = int.Parse(ddlMovecid.SelectedValue);
                ddlMovepid.Items.Clear();
                cmdMoveOk.Enabled = cid != 0;
                if (cid == 0) return;

                this.BindDDL_Pid(ddlMovepid, cid, 0, "");
                CCommon.Insert_FirstitemDDL(ddlMovepid);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }

        protected void ddlAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (ddlAction.SelectedValue)
                {
                    case Commandparam.Delete:
                        this.cmdDelete_Click(null, null);
                        break;
                    case Commandparam.Copy:
                        this.cmdCopy_Click(null, null);
                        break;
                    case Commandparam.Move:
                        this.cmdMove_Click(null, null);
                        break;
                    case Commandparam.Updatedisplayorder:
                        this.cmdUpdateorder_Click(null, null);
                        break;
                    case Commandparam.Setupattribute:
                        this.cmdSetupattribute_Click(null, null);
                        break;
                }
                ddlAction.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        #endregion
    }
}
