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

using dangdongcmm.dal;
using dangdongcmm.model;
using dangdongcmm.utilities;

namespace dangdongcmm.cmm
{
    public class CCommon
    {
        public static string LANG
        {
            get
            {
                return HttpContext.Current.Session[Sessionparam.LANGCMM] == null ? CConstants.LANG_DEF : HttpContext.Current.Session[Sessionparam.LANGCMM].ToString();
            }
            set
            {
                HttpContext.Current.Session[Sessionparam.LANGCMM] = value;
            }
        }
        public static string PreviousUrl
        {
            get
            {
                return HttpContext.Current.Session[Sessionparam.PREVIOUSURL] != null ? HttpContext.Current.Session[Sessionparam.PREVIOUSURL].ToString() : string.Empty;
            }
            set
            {
                HttpContext.Current.Session[Sessionparam.PREVIOUSURL] = value;
            }
        }

        public static int Get_QueryNumber(string param)
        {
            try
            {
                if (HttpContext.Current.Request.QueryString[param] == null) return 0;
                string var = HttpContext.Current.Request.QueryString[param].ToString();
                if (CFunctions.IsNullOrEmpty(var) || !CFunctions.IsInteger(var)) return 0;

                return int.Parse(var);
            }
            catch
            {
                return 0;
            }
        }
        public static string Get_QueryString(string param)
        {
            try
            {
                if (HttpContext.Current.Request.QueryString[param] == null) return "";
                return HttpContext.Current.Request.QueryString[param].ToString();
            }
            catch
            {
                return "";
            }
        }
        public static string Get_FormString(string param)
        {
            try
            {
                if (HttpContext.Current.Request.Form[param] == null) return "";
                return HttpContext.Current.Request.Form[param].ToString();
            }
            catch
            {
                return "";
            }
        }
        public static object Session_Get(string name)
        {
            if (HttpContext.Current.Session[name] == null)
                return null;
            else
                return HttpContext.Current.Session[name];
        }
        public static void Session_Set(string name, object value)
        {
            if (HttpContext.Current.Session[name] == null)
                HttpContext.Current.Session.Add(name, value);
            else
                HttpContext.Current.Session[name] = value;
            return;
        }
        public static void Session_Remove(string name)
        {
            if (HttpContext.Current.Session[name] != null)
                HttpContext.Current.Session.Remove(name);
        }

        public static UserInfo Get_CurrentUser()
        {
            try
            {
                UserInfo info = (UserInfo)Session_Get(Sessionparam.USERLOGIN);
                return info;
            }
            catch
            {
                return null;
            }
        }
        public static string Get_CurrentUsername()
        {
            try
            {
                UserInfo info = (UserInfo)Session_Get(Sessionparam.USERLOGIN);
                return info == null ? "" : info.Username;
            }
            catch
            {
                return "";
            }
        }
        public static bool Right_upt(string page)
        {
            try
            {
                UserInfo info = Get_CurrentUser();
                if (info == null || info.iRight == null) return false;
                UserrightInfo rinfo = info.iRight;

                bool vlreturn = !CFunctions.IsNullOrEmpty(rinfo.R_sys) || IsRightAllow(page, rinfo.R_upt);
                return vlreturn;
            }
            catch
            {
                return false;
            }
        }
        public static bool Right_del(string page)
        {
            try
            {
                UserInfo info = Get_CurrentUser();
                if (info == null || info.iRight == null) return false;
                UserrightInfo rinfo = info.iRight;

                bool vlreturn = !CFunctions.IsNullOrEmpty(rinfo.R_sys) || IsRightAllow(page, rinfo.R_del);
                return vlreturn;
            }
            catch
            {
                return false;
            }
        }
        public static bool Right_sys()
        {
            try
            {
                UserInfo info = Get_CurrentUser();
                if (info == null || info.iRight == null) return false;
                UserrightInfo rinfo = info.iRight;

                bool vlreturn = !CFunctions.IsNullOrEmpty(rinfo.R_sys);
                return vlreturn;
            }
            catch
            {
                return false;
            }
        }
        private static bool IsRightAllow(string page, string right)
        {
            try
            {
                int index = right.IndexOf("#" + page.Replace(".aspx", "") + "#");
                bool vlreturn = index == -1 ? false : true;
                //bool vlreturn = CFunctions.IsNullOrEmpty(right) ? false : true;
                return vlreturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static int GetStatus_upt(string status)
        {
            try
            {
                return status == ((int)CConstants.State.Status.Disabled).ToString() ? (int)CConstants.State.Status.Disabled : (Right_sys() ? (int)CConstants.State.Status.Actived : (int)CConstants.State.Status.Waitactive);
            }
            catch
            {
                return (int)CConstants.State.Status.Waitdelete;
            }
        }
        public static int GetStatus_upt()
        {
            try
            {
                return Right_sys() ? (int)CConstants.State.Status.Actived : (int)CConstants.State.Status.Waitactive;
            }
            catch
            {
                return (int)CConstants.State.Status.Waitdelete;
            }
        }
        public static int GetStatus_del()
        {
            try
            {
                return Right_sys() ? (int)CConstants.State.Status.Deleted : (int)CConstants.State.Status.Waitdelete;
            }
            catch
            {
                return (int)CConstants.State.Status.Waitdelete;
            }
        }

        public static string Get_Definephrase(string tag)
        {
            try
            {
                string ffile = "/cmm/xhtml/Definephrase.xml";
                string fpath = HttpContext.Current.Server.MapPath(ffile);
                if (!System.IO.File.Exists(fpath)) return "";

                string vlreturn = "";
                System.Xml.XmlTextReader rdr = new System.Xml.XmlTextReader(fpath);
                while (rdr.Read())
                {
                    switch (rdr.NodeType)
                    {
                        case System.Xml.XmlNodeType.Element:
                            if (rdr.Name == LANG + tag)
                            {
                                vlreturn = rdr.ReadElementString();
                                goto lexit;
                            }
                            else if (rdr.Name != "phrase")
                                rdr.Skip();
                            break;
                        default:
                            break;
                    }
                }
            lexit:
                rdr.Close();
                return vlreturn;
            }
            catch
            {
                return "Cannot get phrase";
            }
        }
        public static void CatchEx(Exception ex)
        {
            //LOGGER.Error(ex.ToString());
            throw ex;
            //HttpContext.Current.Response.Write("Error: " + ex.Message);
        }

        #region for list

        public static void BindDDL_Cid_DDL(ListOptions options, DropDownList ddl, int cid, int pid, string separator)
        {
            options.SortExp = Queryparam.Sqlcolumn.Orderd;
            options.GetAll = true;

            List<CategoryInfo> list = (new CCategory(CCommon.LANG)).Wcmm_Getlist(cid, pid, options);
            if (list == null) return;

            foreach (CategoryInfo info in list)
            {
                string sep = pid == 0 ? "" : separator + "---";
                ListItem item = new ListItem(sep + info.Name, info.Id.ToString());
                ddl.Items.Add(item);
                BindDDL_Cid_DDL(options, ddl, cid, info.Id, sep);
            }
        }
        public static void Insert_FirstitemDDL(DropDownList ddl)
        {
            ListItem item = new ListItem(Get_Definephrase(Definephrase.Firstitem_ddl), "0");
            item.Attributes.Add("class", "textdefndis");
            ddl.Items.Insert(0, item);
        }
        public static void Insert_FirstitemDDL(DropDownList ddl, string value)
        {
            ListItem item = new ListItem(Get_Definephrase(Definephrase.Firstitem_ddl), value);
            item.Attributes.Add("class", "textdefndis");
            ddl.Items.Insert(0, item);
        }

        #endregion

    }
}
