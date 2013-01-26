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

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm.cmm
{
    public partial class managerupload : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Bind_ListFolder();
                this.Bind_ListFiles("../../commup");
            }
        }

        #region properties
        public string WEBSITE
        {
            get
            {
                return ConfigurationSettings.AppSettings["WEBSITE"];
            }
        }
        public string PATHCURRENT
        {
            get
            {
                return ViewState["PATHCURRENT"] == null ? "" : ViewState["PATHCURRENT"].ToString();
            }
            set
            {
                ViewState["PATHCURRENT"] = value;
            }
        }
        #endregion

        #region private methods
        private void Bind_ListFolder()
        {
            if (!Directory.Exists(Server.MapPath("../../commup"))) return;
            
            List<FileattachInfo> list = new List<FileattachInfo>();
            FileattachInfo info = new FileattachInfo();
            info.Name = "commup";
            info.Path = "../../commup";
            info.Iconex = "----<img src='../images/folder_open.gif' border='0' align='absmiddle' />";
            info.Orderd = 2;
            list.Add(info);
            
            string[] directories = Directory.GetDirectories(Server.MapPath("../../commup"));
            foreach (string directory in directories)
            {
                info = new FileattachInfo();
                info.Name = directory.Substring(directory.LastIndexOf("\\") + 1);
                info.Path = "../../commup/" + info.Name;
                info.Iconex = "--------<img src='../images/folder_close.gif' border='0' align='absmiddle' />";
                info.Orderd = 3;
                list.Add(info);
                List<FileattachInfo> listout = this.Load_ListFolder(list, "../../commup/" + info.Name, 3);
                if (listout != null)
                    list.AddRange(listout);
            }
            (new GenericList<FileattachInfo>()).Bind_DataList(dtlListFolder, null, list, 0);
            pnlListFolder.Visible = list != null && list.Count > 0;
        }
        private List<FileattachInfo> Load_ListFolder(List<FileattachInfo> listin, string foldername, int folderlevel)
        {
            string pre_text = "";
            for (int i = 0; i < folderlevel; i++)
            {
                pre_text += "----";
            }

            List<FileattachInfo> list = null;
            string[] directories = Directory.GetDirectories(Server.MapPath(foldername));
            foreach (string directory in directories)
            {
                FileattachInfo info = new FileattachInfo();
                info.Name = directory.Substring(directory.LastIndexOf("\\") + 1);
                info.Path = foldername + "/" + info.Name;
                info.Iconex = pre_text + "<img src='../images/folder_close.gif' border='0' align='absmiddle' />";
                info.Orderd = folderlevel + 1;
                if (list == null)
                    list = new List<FileattachInfo>();
                list.Add(info);
                List<FileattachInfo> listout = this.Load_ListFolder(list, foldername + "/" + info.Name, folderlevel + 1);
                if (listout != null)
                    list.AddRange(listout);
            }
            return list;
        }

        private void Bind_ListFiles(string path)
        {
            PATHCURRENT = path;
            lblSearchLocation.Text = path.Replace("../", "") + "/";

            List<FileattachInfo> list = this.Load_ListFiles(path);
            (new GenericList<FileattachInfo>()).Bind_DataList(dtlListFiles, null, list, 0);
            pnlListFiles.Visible = list != null && list.Count > 0;
        }
        private List<FileattachInfo> Load_ListFiles(string path)
        {
            if (path == WEBSITE) return null;

            List<FileattachInfo> list = null;
            string[] files = Directory.GetFiles(Server.MapPath(path), "*.*", chkSearchOption.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                if (file.IndexOf(txtKeyword.Text.Trim().ToLower()) == -1) continue;

                FileattachInfo info = new FileattachInfo();
                FileInfo f = new FileInfo(file);
                info.Name = f.Name;
                info.Sized = f.Length;
                string filePath = file.Replace("\\", "/");
                info.Path = "../../" + filePath.Substring(filePath.IndexOf("commup"));
                if (list == null)
                    list = new List<FileattachInfo>();
                list.Add(info);
            }
            return list;
        }
        #endregion

        #region events
        protected void dtlListFolder_ItemCommand(object source, DataListCommandEventArgs e)
        {
            try
            {
                string path = e.CommandArgument.ToString();
                switch (e.CommandName)
                {
                    case "ViewFilesInFolder":
                        this.Bind_ListFiles(path);
                        break;
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void dtlListFiles_ItemCommand(object source, DataListCommandEventArgs e)
        {
            try
            {
                string path = e.CommandArgument.ToString();
                switch (e.CommandName)
                {
                    case "DeleteFile":
                        if (!CCommon.Right_sys())
                        {
                            lstError = new List<Errorobject>();
                            lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_right, "", null);
                            Master.Form_ShowError(lstError);
                            return;
                        }
                        string dir = Server.MapPath(path);
                        if (!System.IO.File.Exists(dir)) return;
                        System.IO.File.Delete(dir);
                        this.Bind_ListFiles(path.Remove(path.LastIndexOf("/")));
                        break;
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }

        protected void cmdUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CCommon.Right_sys())
                {
                    lstError = new List<Errorobject>();
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_right, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                if (!fileUpload.HasFile) return;

                HttpPostedFile postedFile = fileUpload.PostedFile;
                if (CFunctions.IsNullOrEmpty(postedFile.FileName)) return;
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(postedFile.FileName);

                string pathCurrent = CFunctions.IsNullOrEmpty(sourceName.Value) ? sourcePath.Value + fileInfo.Name : sourcePath.Value;
                postedFile.SaveAs(Server.MapPath(pathCurrent));
                if (CFunctions.IsNullOrEmpty(sourceName.Value))
                    this.Bind_ListFiles(PATHCURRENT);
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
                this.Bind_ListFiles(PATHCURRENT);
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdDeleteFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CCommon.Right_sys())
                {
                    lstError = new List<Errorobject>();
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_right, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                if (lblSearchLocation.Text == "commup/") return;

                string path = Server.MapPath("../../" + lblSearchLocation.Text);
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    this.Bind_ListFolder();

                    string parentFolder = path.Remove(path.Remove(path.Length - 1).LastIndexOf("\\")) + "\\";
                    string filePath = parentFolder.Replace("\\", "/");
                    this.Bind_ListFiles("../../" + filePath.Substring(filePath.IndexOf("commup")));
                }
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        protected void cmdNewFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CCommon.Right_sys())
                {
                    lstError = new List<Errorobject>();
                    lstError = Form_GetError(lstError, Errortype.Error, Definephrase.Invalid_right, "", null);
                    Master.Form_ShowError(lstError);
                    return;
                }

                if (CFunctions.IsNullOrEmpty(txtNewFolder.Text)) return;

                string path = Server.MapPath("../../" + lblSearchLocation.Text);
                if (Directory.Exists(path))
                {
                    if (CFunctions.IsNullOrEmpty(sourcePath.Value))
                    {
                        DirectoryInfo dirinfo = Directory.CreateDirectory(path + txtNewFolder.Text.Trim());
                    }
                    else
                    {
                        if (lblSearchLocation.Text == "commup/") return;
                        string newFolder = path.Remove(path.Remove(path.Length - 1).LastIndexOf("\\")) + "\\" + txtNewFolder.Text.Trim();
                        if (!Directory.Exists(newFolder))
                        {
                            Directory.Move(path, newFolder + "\\");
                        }
                    }
                    this.Bind_ListFolder();
                }
                txtNewFolder.Text = "";
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        #endregion
    }
}
