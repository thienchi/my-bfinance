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

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm.cmm
{
    public partial class ucfileattach : BaseUserControl
    {
        public string Up
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FA_txtUp.Value = Up;
        }

        #region public methods
        public List<FileattachInfo> Save_Files(int iid, int belongto)
        {
            try
            {
                string strFiles = FA_txtFileurl.Value;
                if (CFunctions.IsNullOrEmpty(strFiles)) return null;

                List<FileattachInfo> list = new List<FileattachInfo>();
                string[] arrFiles = strFiles.Split("]#[".Split('$'), StringSplitOptions.None);
                for (int i = 0; i < arrFiles.Length - 1; i++)
                {
                    string[] everyFile = arrFiles[i].Split("][".Split('$'), StringSplitOptions.None);
                    FileattachInfo fileinfo = new FileattachInfo();
                    fileinfo.Name = fileinfo.Path = everyFile[0];
                    fileinfo.Note = everyFile[1];
                    fileinfo.Orderd = CFunctions.IsInteger(everyFile[2]) ? Convert.ToInt32(everyFile[2]) : 0;
                    fileinfo.Id = CFunctions.IsInteger(everyFile[3]) ? Convert.ToInt32(everyFile[3]) : 0;
                    fileinfo.Status = (int)CConstants.State.Status.Actived;
                    fileinfo.Timeupdate = DateTime.Now;
                    fileinfo.Iid = iid;
                    fileinfo.Belongto = belongto;
                    list.Add(fileinfo);
                }
                if (list.Count > 0)
                    if (new CFileattach(CCommon.LANG).Save(list))
                        (new CGeneral(CCommon.LANG, belongto)).Updatenum(iid.ToString(), Queryparam.Sqlcolumn.Album, list.Count, CConstants.NUM_DIRECTLY);
                return list;
            }
            catch
            {
                return null;
            }
        }
        public void Bind_Files(int iid, int belongto)
        {
            try
            {
                string vlreturn = "<table id=\"FA_listImage\">";
                List<FileattachInfo> list = new List<FileattachInfo>();
                if (iid != 0)
                {
                    list = new CFileattach(CCommon.LANG).Wcmm_Getlist(belongto, iid);
                    if (list != null && list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            FileattachInfo fileinfo = (FileattachInfo)list[i];
                            vlreturn += "<tr id=\"farowi" + i + "\" class=\"farowi\">"
                                + "<td title=\"" + fileinfo.Path + "\" class=\"fabase\"><img id=\"faimag" + i + "\" src=\"" + (fileinfo.Path.IndexOf(CConstants.WEBSITE) == 0 ? fileinfo.Path : (CConstants.WEBSITE + "/" + fileinfo.Path)) + "\" /></td>"
                            + "<td title=\"" + fileinfo.Note + "\" class=\"fanote\"><input type=\"text\" value=\"" + fileinfo.Note + "\" onblur=\"FAresetval(this)\" /></td>"
                            + "<td title=\"" + fileinfo.Orderd + "\" class=\"fasort\"><input type=\"text\" value=\"" + fileinfo.Orderd + "\" onkeypress=\"javascript:return UINumber_In(event);\" onblur=\"UINumber_Out(this);FAresetval(this)\" /></td>"
                            + "<td title=\"" + fileinfo.Id + "\" class=\"facomd\"><a href=\"javascript:FAremove(" + fileinfo.Id + "," + i + ");\">x</a></td></tr>";
                        }
                    }
                }
                FA_containerImage.InnerHtml = vlreturn + "</table>";
                FA_containerImage.Attributes.Add("title", list == null ? "0" : list.Count.ToString());
                return;
            }
            catch (Exception ex)
            {
                CCommon.CatchEx(ex);
            }
        }
        #endregion

    }
}