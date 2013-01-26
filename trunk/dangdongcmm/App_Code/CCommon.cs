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
using System.IO;

using dangdongcmm.dal;
using dangdongcmm.model;
using dangdongcmm.utilities;

namespace dangdongcmm
{
    public class CCommon
    {
        public static string LANG
        {
            get
            {
                return HttpContext.Current.Session[Sessionparam.LANG] == null ? CConstants.LANG_DEF : HttpContext.Current.Session[Sessionparam.LANG].ToString();
            }
            set
            {
                HttpContext.Current.Session[Sessionparam.LANG] = value;
            }
        }
        public static string PreviousUrl
        {
            get
            {
                return HttpContext.Current.Session[Sessionparam.WEBPREVIOUSURL] != null ? HttpContext.Current.Session[Sessionparam.WEBPREVIOUSURL].ToString() : string.Empty;
            }
            set
            {
                HttpContext.Current.Session[Sessionparam.WEBPREVIOUSURL] = value;
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
        public long Get_QueryLong(string param)
        {
            try
            {
                if (HttpContext.Current.Request.QueryString[param] == null) return 0;
                string var = HttpContext.Current.Request.QueryString[param].ToString();
                long result;
                long.TryParse(var, out result);

                return result;
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

        public static MemberInfo CurrentMember
        {
            get
            {
                return Session_Get(Sessionparam.WEBUSERLOGIN) == null ? null : (MemberInfo)Session_Get(Sessionparam.WEBUSERLOGIN);
            }
        }

        public static string Get_Definephrase(string tag)
        {
            try
            {
                string ffile = "~/xhtml/Definephrase.xml";
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
            //throw ex;
            HttpContext.Current.Response.Write("Error: " + ex.Message);
        }

        public static List<Errorobject> Form_GetError(List<Errorobject> lstError, string errortype, string tag, string message, object txt)
        {
            if (lstError == null)
                lstError = new List<Errorobject>();
            Errorobject error = new Errorobject(txt, Get_Definephrase(tag) + message, errortype);
            lstError.Add(error);
            return lstError;
        }
        public static bool Form_ShowError(List<Errorobject> lstError, Label lblError, Panel pnlError, Panel pnlForm, bool isClear)
        {
            if (lstError == null || lstError.Count == 0)
            {
                if (lblError != null) lblError.Text = "";
                if (pnlError != null) pnlError.CssClass = "FLOUTNOTICE";
                return true;
            }

            string ErrorMessage = "";
            foreach (Errorobject error in lstError)
            {
                ErrorMessage += error.ErrorMessage;
                if (error.Control != null)
                    ((WebControl)error.Control).Attributes.Add("style", "background-color:#ffffdd;");
            }
            if (isClear)
            {
                pnlForm.Controls.Clear();
                Label lbl = new Label();
                lbl.CssClass = "FLOUTNOTICE";
                lbl.Text = ErrorMessage;
                pnlForm.Controls.Add(lbl);
            }
            else
            {
                if (lblError != null) lblError.Text = ErrorMessage;
                if (pnlError != null) pnlError.CssClass = "FLOUTNOTICESHOW";
            }
            return false;
        }
        public static bool Form_ShowError(List<Errorobject> lstError, Label lblError)
        {
            if (lstError == null || lstError.Count == 0)
            {
                lblError.Text = "";
                return true;
            }

            string ErrorMessage = "";
            foreach (Errorobject error in lstError)
            {
                ErrorMessage += "<div>" + error.ErrorMessage + "</div>";
                if (error.Control != null)
                    ((WebControl)error.Control).Attributes.Add("style", "background-color:#ffffdd;");
            }
            lblError.Text = ErrorMessage;
            return false;
        }
        public static void BindDDL_Cid(ListOptions options, DropDownList ddl, int cid, int pid, string separator)
        {
            int numResults = 0;
            List<CategoryInfo> list = (new CCategory(LANG)).Getlist(cid, pid, options, out numResults);
            if (list == null) return;

            foreach (CategoryInfo info in list)
            {
                string sep = pid == 0 ? "" : separator + "---";
                ListItem item = new ListItem(sep + info.Name, info.Id.ToString());
                ddl.Items.Add(item);
                BindDDL_Cid(options, ddl, cid, info.Id, sep);
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

        public static void LoadCaptcha(Image imgCaptcha)
        {
            string path = "~/commup/captcha/";
            string[] files = Directory.GetFiles(HttpContext.Current.Server.MapPath(path), "*.png", SearchOption.TopDirectoryOnly);
            string file = files[new Random().Next(files.Length)];
            imgCaptcha.ImageUrl = path + new FileInfo(file).Name;
        }
    }
}
