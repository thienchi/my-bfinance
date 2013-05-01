using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;

namespace dangdongcmm.utilities
{
    public class CFunctions
    {
        private const string letters_lower = "abcdefghijklmnopqrstuvwxyz0123456789";
        private const string LETTERS_UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static bool IsMultiLanguage()
        {
            try
            {
                return CConstants.LANG_NUM > 1;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsInteger(string param)
        {
            try
            {
                if (param.Trim().Length == 0) return false;
                int i = Convert.ToInt32(param);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsDouble(string param)
        {
            try
            {
                if (param.Trim().Length == 0) return false;
                double d = Convert.ToDouble(param);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsDate(string param)
        {
            try
            {
                //if(param.Length!=10) return false;
                param.Replace("_", " ").Trim();
                DateTimeFormatInfo i = new DateTimeFormatInfo();
                i.ShortDatePattern = "dd/MM/yyyy";
                DateTime date = DateTime.Parse(param, i);
                if (date.Year < 1901) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsAlpha(string param)
        {
            try
            {
                // is c a String or a character?
                if (param.Length > 1)
                {
                    int count = 0;
                    for (int j = 0; j < param.Length; j++)
                    {
                        // call isAlpha recursively for each character
                        char alpha = param[j];
                        if (letters_lower.IndexOf(alpha) >= 0 || LETTERS_UPPER.IndexOf(alpha) >= 0)
                            count++;
                        else
                            return false;
                    }
                    if (count == param.Length)
                        return true;
                }
                else
                {
                    if (letters_lower.IndexOf(param) >= 0 || LETTERS_UPPER.IndexOf(param) >= 0)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsEmail(string param)
        {
            try
            {
                param = param.Trim().ToLower();
                string letters_allow = "abcdefghijklmnopqrstuvwxyz0123456789_-.";
                bool isvalid = true;
                string[] arr_input = param.Split(Convert.ToChar("@"));

                if (arr_input.Length != 2)
                    goto invalid;

                for (int i = 0; i < arr_input.Length; i++)
                {
                    string c_input = arr_input[i];
                    if (c_input.Length <= 0)
                        goto invalid;

                    if (c_input.Length > 0)
                    {
                        for (int j = 0; j < c_input.Length; j++)
                        {
                            char alpha = c_input[j];
                            if (letters_allow.IndexOf(alpha) < 0)
                                goto invalid;
                        }
                    }

                    if (c_input.IndexOf(".") == 0 || c_input.IndexOf(".") == c_input.Length - 1)
                        goto invalid;
                }

                string p2 = arr_input[1];
                if (p2.IndexOf(".") < 0)
                    goto invalid;
                if (p2.LastIndexOf(".") == p2.Length - 1)
                    goto invalid;

                string ext = p2.Substring(p2.IndexOf(".") + 1);
                //				if(ext.Length!=2 && ext.Length!=3 && ext.Length!=4)
                if (ext.Length == 0)
                    goto invalid;

                if (param.IndexOf("..") >= 0)
                    goto invalid;

                return true;

            invalid:
                {
                    isvalid = false;
                    return isvalid;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool IsNullOrEmpty(string param)
        {
            try
            {
                if (param == null) return true;
                return string.IsNullOrEmpty(param.Trim());
            }
            catch
            {
                return false;
            }
        }

        public static DateTime ConvertDatetime(string param)
        {
            try
            {
                if (string.IsNullOrEmpty(param))
                    return new DateTime(0);
                param = param.Replace("-", "/");
                param = param.Replace("_", "/");
                param = param.Replace(".", "/");
                int dd = 0;
                int MM = 0;
                int yyyy = 0;
                int indexDD = param.IndexOf("/");
                int indexMM = param.IndexOf("/", indexDD + 1);
                dd = Int32.Parse(param.Substring(0, indexDD));
                MM = Int32.Parse(param.Substring(indexDD + 1, indexMM - indexDD - 1));
                yyyy = Int32.Parse(param.Substring(indexMM + 1));
                return new DateTime(yyyy, MM, dd);
            }
            catch
            {
                return new DateTime(0);
            }
        }
        public static DateTime Get_DatetimeDeadline(string date, string hour)
        {
            try
            {
                string param = date;
                if (string.IsNullOrEmpty(param)) return new DateTime(0);
                param = param.Replace("-", "/");
                param = param.Replace("_", "/");
                param = param.Replace(".", "/");
                int dd = 0;
                int MM = 0;
                int yyyy = 0;
                int indexMM = param.IndexOf("/");
                int indexDD = param.IndexOf("/", indexMM + 1);
                MM = Int32.Parse(param.Substring(0, indexMM));
                dd = Int32.Parse(param.Substring(indexMM + 1, indexDD - indexMM - 1));
                yyyy = Int32.Parse(param.Substring(indexDD + 1));
                return new DateTime(yyyy, MM, dd, int.Parse(hour), 0, 0);
            }
            catch
            {
                return new DateTime(0);
            }
        }
        public static string Get_DateAgofromToday(DateTime currentdate)
        {
            try
            {
                if (currentdate.Equals(new DateTime(0))) return "";

                string vlreturn = "";
                DateTime today = DateTime.Now;
                string Subtract = today.Subtract(currentdate).ToString();
                string[] datehour_minu = Subtract.Split(':');
                int days = 0;
                int hour = 0;
                string[] date_hour = datehour_minu[0].Split('.');
                if (date_hour.Length > 1)
                {
                    days = int.Parse(date_hour[0]);
                    hour = int.Parse(date_hour[1]);
                }
                else
                {
                    days = 0;
                    hour = int.Parse(date_hour[0]);
                }
                
                int minu = int.Parse(datehour_minu[1]);
                if (days == 0)
                {
                    if (hour == 0)
                    {
                        if (minu == 0)
                        {
                            vlreturn = "cách đây 1 phút";
                        }
                        else
                        {
                            vlreturn = "cách đây " + minu + " phút";
                        }
                    }
                    else
                    {
                        vlreturn = "cách đây khoảng " + hour + " giờ";
                    }
                }
                else if (Math.Abs(days) > 0 && Math.Abs(days) < 7)
                {
                    vlreturn = "cách đây " + days + " ngày";
                }
                else
                {
                    //vlreturn = currentdate.ToLongDateString().Replace(", " + today.Year, "");
                    vlreturn = "lúc " + Get_Datetimetext(currentdate);
                }
                if (Subtract.IndexOf("-") != -1)
                    vlreturn = vlreturn.Replace("cách đây", "còn");
                return vlreturn;
            }
            catch
            {
                return "";
            }
        }
        public static string Get_Datetimetext(DateTime date)
        {
            if (date.Equals(new DateTime(0))) return "";

            //String[] arrMonth = new String[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "July", "Aug", "Sep", "Oct", "Nov", "Dec" };
            //string DayOfWeek = date.DayOfWeek.ToString().Substring(0, 3);
            String Time = date.ToString("T").Remove(date.ToString("T").LastIndexOf(":"), 3);

            //string vlreturn = Time + (DayOfWeek + ", " + arrMonth[date.Month - 1] + " " + date.Day) + (date.Year == DateTime.Now.Year ? "" : (" " + date.Year));
            string vlreturn = Time + " " + (date.Day.ToString().PadLeft(2, '0') + "/" + date.Month.ToString().PadLeft(2, '0')) + (date.Year == DateTime.Now.Year ? "" : (" " + date.Year));
            return vlreturn;
        }

        public static DateTime Get_Datetime(string param)
        {
            try
            {
                if (string.IsNullOrEmpty(param))
                    return new DateTime(0);
                return ConvertDatetime(param.Trim());
            }
            catch
            {
                return new DateTime(0);
            }
        }
        public static string Set_Datetime(DateTime param)
        {
            try
            {
                if (param.Equals(new DateTime(0)))
                    return "";
                return param.ToString("dd/MM/yyyy");
            }
            catch
            {
                return "";
            }
        }
        public static string Set_Currency(double paramin)
        {
            try
            {
                string param = paramin.ToString().Replace(",", "").Replace("$", "").Trim();
                if (IsNullOrEmpty(param)) return "";
                bool negative = false;
                if (param.IndexOf("-") != -1)
                {
                    param = param.Replace("-", "").Trim();
                    negative = true;
                }
                string[] arrchar = param.ToString().Split('.');
                string n_head = arrchar[0];
                string n_foot = arrchar.Length > 1 ? arrchar[1] : "";

                string n_head_result = "";
                if (n_head.Length > 3)
                {
                    int n_head_mod = n_head.Length % 3;
                    int n_head_div = n_head.Length / 3;

                    for (int i = 0; i <= n_head_div; i++)
                    {
                        if (i == n_head_div && n_head_mod == 0) break;
                        if (n_head_mod != 0)
                            if (i == 0)
                                n_head_result += n_head.Substring(0, n_head_mod) + ",";
                            else
                                n_head_result += n_head.Substring((i - 1) * 3 + n_head_mod, 3) + ",";
                        else
                            n_head_result += n_head.Substring(i * 3, 3) + ",";
                    }
                    n_head_result = n_head_result.Remove(n_head_result.Length - 1);
                }
                return (negative ? "-" : "") + (n_head_result == "" ? n_head : n_head_result) + (n_foot != "" ? ("." + (n_foot.Length == 1 ? n_foot + "0" : n_foot.Substring(0, 2))) : "");

            }
            catch
            {
                return "";
            }
        }

        public static string Iconformarkas(int markas, int id)
        {
            string icon = "<img id=\"micon" + id + "\" src=\"$VAR_ICONPATH$\" align=\"absmiddle\" />";
            string path = "";
            switch (markas)
            {
                default:
                case (int)CConstants.State.MarkAs.None:
                    path = "../images/icon_none.gif";
                    break;
                case (int)CConstants.State.MarkAs.Focus:
                    path = "../images/icon_focus.gif";
                    break;
                case (int)CConstants.State.MarkAs.OnHome:
                    path = "../images/icon_home.gif";
                    break;
            }
            icon = icon.Replace("$VAR_ICONPATH$", path);
            return icon;
        }
        public static string Iconforstatus(int status, int id)
        {
            string vlreturn = "";
            switch (status)
            {
                default:
                case (int)CConstants.State.Status.Waitactive:
                    vlreturn = "<font color=\"#0b55c4\">Waitting update</font>";
                    break;
                case (int)CConstants.State.Status.Actived:
                    vlreturn = "Actived";
                    break;
                case (int)CConstants.State.Status.Waitdelete:
                    vlreturn = "<font color=\"#c61444\">Waitting delete</font>";
                    break;
                case (int)CConstants.State.Status.Deleted:
                    vlreturn = "Deleted";
                    break;
                case (int)CConstants.State.Status.Disabled:
                    vlreturn = "<font color=\"#c61444\">Disabled</font>";
                    break;
            }
            return vlreturn;
        }
        public static string Iconforiconex(string iconex, int id)
        {
            string icon = CFunctions.IsNullOrEmpty(iconex) ? "" : "<img id=\"iicon" + id + "\" src=\"../" + iconex + "\" align=\"absmiddle\" />";
            return icon;
        }
        
        public static string Randomnum(int size)
        {
            try
            {
                System.Random ran = new System.Random();
                string v_return = ran.GetHashCode().ToString() + ran.Next(99).ToString();

                return v_return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string Randomstr(int size)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                char ch;
                int rancode;
                Random rand = new Random();
                for (int i = 0; i < size; i++)
                {
                    rancode = rand.Next(65, 90);
                    if (i % 2 == 0 && rancode % 2 == 0)
                    {
                        ch = Convert.ToChar(rand.Next(10).ToString());
                    }
                    else
                    {
                        ch = Convert.ToChar(Convert.ToInt32(rancode));
                    }
                    sb.Append(ch);
                }
                string vlreturn = sb.ToString();
                return vlreturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private const string extension_img = ".gif.jpg.jpeg.png";
        private const string extension_swf = ".swf";
        private const string extension_flv = ".flv";
        private const string extension_vid = ".wmv.mpg.mpeg.mpe.asf.asx.wax.wmv.wmx.avi";
        private const string extension_aud = ".mp3";
        public static string Get_Filepreviewurl(string filepreview)
        {
            if (IsNullOrEmpty(filepreview)) return "";
            
            string vlreturn = "";
            if (isExternallink(filepreview))
                vlreturn = filepreview;
            else
                vlreturn = ConfigurationSettings.AppSettings["WEBSITE"] + "/" + filepreview;
            
            return vlreturn;
        }
        public static string Get_Filepreview(string filepreview)
        {
            if (IsNullOrEmpty(filepreview)) return "";
            int dotindex = filepreview.LastIndexOf(".");
            if (dotindex == -1) return "";
            
            string extension = filepreview.Substring(dotindex).ToLower();
            string vlreturn = "";
            string src = filepreview;
            if (!isExternallink(filepreview))
                src = "/" + filepreview;
            string ran = filepreview.Substring(filepreview.LastIndexOf("/") + 1).Replace(" ", "_").Replace(".", "_");
            if (extension_img.IndexOf(extension) > -1)
            {
                vlreturn = "<img class=\"imgpreview\" id=\"imgpreview" + ran + "\" src=\"" + src + "\">";
            }
            else if (extension_swf.IndexOf(extension) > -1)
            {
                vlreturn = "<OBJECT class=\"swfpreview\">"
                    + " <param name=\"movie\" value=\"bannerSmall.swf\">"
                    + " <param name=\"quality\" value=\"high\" />"
                    + " <param name=\"wmode\" value=\"transparent\" />"
                    + " <EMBED src=\"" + src + "\" class=\"swfpreview\" wmode=\"transparent\" type=\"application/x-shockwave-flash\"></EMBED></OBJECT>";
            }
            else if (extension_flv.IndexOf(extension) > -1)
            {
                vlreturn = "<span id=\"FlashPlayer" + ran + "\" title=\"" + src + "\" class=\"Normal\" align=\"center\"></span>";
                vlreturn += "<script type=\"text/javascript\">"
                    + "createFlashPlayer(\"" + ran + "\",\"" + src + "\");"
                    + "</script>";
            }
            else if (extension_vid.IndexOf(extension) > -1)
                vlreturn = "<OBJECT id=\"mediaPlayer\" class=\"vidpreview\" "
                    + " classid=\"CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95\" "
                    + " codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701\""
                    + " standby=\"Loading Microsoft Windows Media Player components...\" type=\"application/x-oleobject\">"
                    + " <param name=\"fileName\" value=\"" + src + "\">"
                    + " <param name=\"animationatStart\" value=\"true\">"
                    + " <param name=\"transparentatStart\" value=\"false\">"
                    + " <param name=\"autoStart\" value=\"false\">"
                    + " <param name=\"showControls\" value=\"true\">"
                    + " <param name=\"loop\" value=\"false\">"
                    + " <EMBED class=\"vidpreview\" type=\"application/x-mplayer2\" pluginspage=\"http://microsoft.com/windows/mediaplayer/en/download/\""
                    + " src=\"" + src + "\" >"
                    + " </EMBED></OBJECT>";
            else if (extension_aud.IndexOf(extension) > -1)
                vlreturn = "<OBJECT id=\"mediaPlayer\" class=\"audpreview\" "
                    + " classid=\"CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95\" "
                    + " codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701\""
                    + " standby=\"Loading Microsoft Windows Media Player components...\" type='application/x-oleobject\">"
                    + " <param name=\"fileName\" value=\"" + src + "\">"
                    + " <param name=\"animationatStart\" value=\"true\">"
                    + " <param name=\"transparentatStart\" value=\"false\">"
                    + " <param name=\"autoStart\" value=\"false\">"
                    + " <param name=\"showControls\" value=\"true\">"
                    + " <param name=\"loop\" value=\"false\">"
                    + " <EMBED class=\"audpreview\" type=\"application/x-mplayer2\" pluginspage='http://microsoft.com/windows/mediaplayer/en/download/\""
                    + " src=\"" + src + "\" >"
                    + " </EMBED></OBJECT>";
            else
                vlreturn = "<a class=\"lnkpreview\" href=\"javascript:MB_Download('" + CFunctions.MBEncrypt(src) + "');\"><img src=\"../images/download.png\" border=\"0\" align=\"absmiddle\"></a>";
            return vlreturn;
        }
        public static string Get_Filepreview_cmm(string filepreview)
        {
            if (IsNullOrEmpty(filepreview)) return "";
            int dotindex = filepreview.LastIndexOf(".");
            if (dotindex == -1) return "";

            string extension = filepreview.Substring(dotindex).ToLower();
            string vlreturn = "";
            string src = filepreview;
            if (!isExternallink(filepreview))
                src = "../../" + filepreview;
            if (extension_img.IndexOf(extension) > -1)
                vlreturn = "<IMG class=\"imgpreview\" src=\"" + src + "\">";
            else if (extension_swf.IndexOf(extension) > -1)
                vlreturn = "<EMBED class=\"swfpreview\" src=\"" + src + "\" type=\"application/x-shockwave-flash\"></EMBED>";
            else if (extension_flv.IndexOf(extension) > -1)
            {
                string ran = filepreview.Substring(filepreview.LastIndexOf("/") + 1).Replace(" ", "_").Replace(".", "_");
                vlreturn = "<span id=\"FlashPlayer" + ran + "\" title=\"" + src + "\" class=\"Normal\" align=\"center\"></span>";
                vlreturn += "<script type=\"text/javascript\">"
                    + "createFlashPlayer(\"" + ran + "\",\"" + src + "\");"
                    + "</script>";
            }
            else if (extension_vid.IndexOf(extension) > -1)
                vlreturn = "<OBJECT id=\"mediaPlayer\" class=\"vidpreview\" "
                    + " classid=\"CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95\" "
                    + " codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701\""
                    + " standby=\"Loading Microsoft Windows Media Player components...\" type=\"application/x-oleobject\">"
                    + " <param name=\"fileName\" value=\"" + src + "\">"
                    + " <param name=\"animationatStart\" value=\"true\">"
                    + " <param name=\"transparentatStart\" value=\"false\">"
                    + " <param name=\"autoStart\" value=\"false\">"
                    + " <param name=\"showControls\" value=\"true\">"
                    + " <param name=\"loop\" value=\"false\">"
                    + " <EMBED class=\"vidpreview\" type=\"application/x-mplayer2\" pluginspage=\"http://microsoft.com/windows/mediaplayer/en/download/\""
                    + " src=\"" + src + "\" >"
                    + " </EMBED></OBJECT>";
            else if (extension_aud.IndexOf(extension) > -1)
                vlreturn = "<OBJECT id=\"mediaPlayer\" class=\"audpreview\" "
                    + " classid=\"CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95\" "
                    + " codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701\""
                    + " standby=\"Loading Microsoft Windows Media Player components...\" type=\"application/x-oleobject\">"
                    + " <param name=\"fileName\" value=\"" + src + "\">"
                    + " <param name=\"animationatStart\" value=\"true\">"
                    + " <param name=\"transparentatStart\" value=\"false\">"
                    + " <param name=\"autoStart\" value=\"false\">"
                    + " <param name=\"showControls\" value=\"true\">"
                    + " <param name=\"loop\" value=\"false\">"
                    + " <EMBED class=\"audpreview\" type=\"application/x-mplayer2\" pluginspage='http://microsoft.com/windows/mediaplayer/en/download/\""
                    + " src=\"" + src + "\" >"
                    + " </EMBED></OBJECT>";
            else
                vlreturn = "<A class=\"lnkpreview\" href=\"" + src + "\"><IMG src=\"../images/download.png\" border=\"0\" align=\"absmiddle\"></A>";
            return vlreturn;
        }
        public static string Get_Fileadvertise(string filepreview, int width, int height)
        {
            if (IsNullOrEmpty(filepreview)) return "";
            int dotindex = filepreview.LastIndexOf(".");
            if (dotindex == -1) return "";

            string extension = filepreview.Substring(dotindex).ToLower();
            string vlreturn = "";
            string src = filepreview;
            if (!isExternallink(filepreview))
                src = "../" + filepreview;
            if (extension_img.IndexOf(extension) > -1)
                vlreturn = "<IMG class=\"imgpreview\" style=\"" + (width == 0 ? "" : ("width:" + width + "px;")) + (height == 0 ? "" : ("height:" + height + "px;")) + "\" src=\"" + src + "\">";
            else if (extension_swf.IndexOf(extension) > -1)
                vlreturn = "<EMBED class=\"swfpreview\" style=\"" + (width == 0 ? "" : ("width:" + width + "px;")) + (height == 0 ? "" : ("height:" + height + "px;")) + "\" src=\"" + src + "\" type=\"application/x-shockwave-flash\"></EMBED>";
            else if (extension_flv.IndexOf(extension) > -1)
            {
                string ran = filepreview.Substring(filepreview.LastIndexOf("/") + 1).Replace(" ", "_").Replace(".", "_");
                vlreturn = "<span id=\"FlashPlayer" + ran + "\" class=\"Normal\" align=\"center\">Bạn cần cài <a href=\"http://www.macromedia.com/go/getflashplayer\">Flash Player</a> để xem được Clip này.</span>";
                vlreturn += "<script type=\"text/javascript\">"
                    + "var sFlashPlayer = new SWFObject(\"../script/mediaplayer.swf\",\"playlist\",\"" + width + "\",\"" + height + "\",\"7\");"
                    + "sFlashPlayer.addParam(\"allowfullscreen\",\"true\");"
                    + "sFlashPlayer.addVariable(\"file\",\"" + src + "\");"
                    + "sFlashPlayer.addVariable(\"displayheight\",\"" + height + "\");"
                    + "sFlashPlayer.addVariable(\"width\",\"" + width + "\");"
                    + "sFlashPlayer.addVariable(\"height\",\"" + height + "\");"
                    + "sFlashPlayer.addVariable(\"backcolor\",\"0x000000\");"
                    + "sFlashPlayer.addVariable(\"frontcolor\",\"0xCCCCCC\");"
                    + "sFlashPlayer.addVariable(\"lightcolor\",\"0x557722\");"
                    + "sFlashPlayer.addVariable(\"shuffle\",\"false\");"
                    + "sFlashPlayer.addVariable(\"repeat\",\"list\");"
                    + "sFlashPlayer.write(\"FlashPlayer" + ran + "\");"
                    + "</script>";
            }
            else if (extension_vid.IndexOf(extension) > -1)
                vlreturn = "<OBJECT id=\"mediaPlayer\" width=\"" + width + "\" height=\"" + height + "\" "
                    + " classid=\"CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95\" "
                    + " codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701\""
                    + " standby=\"Loading Microsoft Windows Media Player components...\" type=\"application/x-oleobject\">"
                    + " <param name=\"fileName\" value=\"" + src + "\">"
                    + " <param name=\"animationatStart\" value=\"true\">"
                    + " <param name=\"transparentatStart\" value=\"false\">"
                    + " <param name=\"autoStart\" value=\"false\">"
                    + " <param name=\"showControls\" value=\"true\">"
                    + " <param name=\"loop\" value=\"false\">"
                    + " <EMBED class=\"vidpreview\" style=\"width:" + width + "px;height:" + height + "px\" type=\"application/x-mplayer2\" pluginspage=\"http://microsoft.com/windows/mediaplayer/en/download/\""
                    + " src=\"" + src + "\" >"
                    + " </EMBED></OBJECT>";
            else if (extension_aud.IndexOf(extension) > -1)
                vlreturn = "<OBJECT id=\"mediaPlayer\" width=\"" + width + "\" height=\"" + height + "\" "
                    + " classid=\"CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95\" "
                    + " codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701\""
                    + " standby=\"Loading Microsoft Windows Media Player components...\" type=\"application/x-oleobject\">"
                    + " <param name=\"fileName\" value=\"" + src + "\">"
                    + " <param name=\"animationatStart\" value=\"true\">"
                    + " <param name=\"transparentatStart\" value=\"false\">"
                    + " <param name=\"autoStart\" value=\"false\">"
                    + " <param name=\"showControls\" value=\"true\">"
                    + " <param name=\"loop\" value=\"false\">"
                    + " <EMBED class=\"audpreview\" style=\"width:" + width + "px;height:" + height + "px\" type=\"application/x-mplayer2\" pluginspage=\"http://microsoft.com/windows/mediaplayer/en/download/\""
                    + " src=\"" + src + "\" >"
                    + " </EMBED></OBJECT>";
            else
                vlreturn = "<A class=\"lnkpreview\" href=\"" + src + "\"><IMG src=\"../images/download.png\" border=\"0\" align=\"absmiddle\"></A>";
            return vlreturn;
        }
        public static string Get_Fileadvertise_cmm(string filepreview, int width, int height)
        {
            if (IsNullOrEmpty(filepreview)) return "";
            int dotindex = filepreview.LastIndexOf(".");
            if (dotindex == -1) return "";

            string extension = filepreview.Substring(dotindex).ToLower();
            string vlreturn = "";
            string src = filepreview;
            if (!isExternallink(filepreview))
                src = "../../" + filepreview;
            if (extension_img.IndexOf(extension) > -1)
                vlreturn = "<IMG class=\"imgpreview\" style=\"" + (width == 0 ? "" : ("width:" + width + "px;")) + (height == 0 ? "" : ("height:" + height + "px;")) + "\" src=\"" + src + "\">";
            else if (extension_swf.IndexOf(extension) > -1)
                vlreturn = "<EMBED class=\"swfpreview\" style=\"" + (width == 0 ? "" : ("width:" + width + "px;")) + (height == 0 ? "" : ("height:" + height + "px;")) + "\" src=\"" + src + "\" type=\"application/x-shockwave-flash\"></EMBED>";
            else if (extension_flv.IndexOf(extension) > -1)
            {
                string ran = filepreview.Substring(filepreview.LastIndexOf("/") + 1).Replace(" ", "_").Replace(".", "_");// DateTime.Now.Ticks.ToString();
                vlreturn = "<span id=\"FlashPlayer" + ran + "\" class=\"Normal\" align=\"center\">Bạn cần cài <a href=\"http://www.macromedia.com/go/getflashplayer\">Flash Player</a> để xem được Clip này.</span>";
                vlreturn += "<script type=\"text/javascript\">"
                    + "var sFlashPlayer = new SWFObject(\"../script/mediaplayer.swf\",\"playlist\",\"" + width + "\",\"" + height + "\",\"7\");"
                    + "sFlashPlayer.addParam(\"allowfullscreen\",\"true\");"
                    + "sFlashPlayer.addVariable(\"file\",\"" + src + "\");"
                    + "sFlashPlayer.addVariable(\"displayheight\",\"" + height + "\");"
                    + "sFlashPlayer.addVariable(\"width\",\"" + width + "\");"
                    + "sFlashPlayer.addVariable(\"height\",\"" + height + "\");"
                    + "sFlashPlayer.addVariable(\"backcolor\",\"0x000000\");"
                    + "sFlashPlayer.addVariable(\"frontcolor\",\"0xCCCCCC\");"
                    + "sFlashPlayer.addVariable(\"lightcolor\",\"0x557722\");"
                    + "sFlashPlayer.addVariable(\"shuffle\",\"false\");"
                    + "sFlashPlayer.addVariable(\"repeat\",\"list\");"
                    + "sFlashPlayer.write(\"FlashPlayer" + ran + "\");"
                    + "</script>";
            }
            else if (extension_vid.IndexOf(extension) > -1)
                vlreturn = "<OBJECT id=\"mediaPlayer\" width=\"" + width + "\" height=\"" + height + "\" "
                    + " classid=\"CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95\" "
                    + " codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701\""
                    + " standby=\"Loading Microsoft Windows Media Player components...\" type=\"application/x-oleobject\">"
                    + " <param name=\"fileName\" value=\"" + src + "\">"
                    + " <param name=\"animationatStart\" value=\"true\">"
                    + " <param name=\"transparentatStart\" value=\"false\">"
                    + " <param name=\"autoStart\" value=\"false\">"
                    + " <param name=\"showControls\" value=\"true\">"
                    + " <param name=\"loop\" value=\"false\">"
                    + " <EMBED class=\"vidpreview\" style=\"width:" + width + "px;height:" + height + "px\" type=\"application/x-mplayer2\" pluginspage=\"http://microsoft.com/windows/mediaplayer/en/download/\""
                    + " src=\"" + src + "\" >"
                    + " </EMBED></OBJECT>";
            else if (extension_aud.IndexOf(extension) > -1)
                vlreturn = "<OBJECT id=\"mediaPlayer\" width=\"" + width + "\" height=\"" + height + "\" "
                    + " classid=\"CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95\" "
                    + " codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701\""
                    + " standby=\"Loading Microsoft Windows Media Player components...\" type=\"application/x-oleobject\">"
                    + " <param name=\"fileName\" value=\"" + src + "\">"
                    + " <param name=\"animationatStart\" value=\"true\">"
                    + " <param name=\"transparentatStart\" value=\"false\">"
                    + " <param name=\"autoStart\" value=\"false\">"
                    + " <param name=\"showControls\" value=\"true\">"
                    + " <param name=\"loop\" value=\"false\">"
                    + " <EMBED class=\"audpreview\" style=\"width:" + width + "px;height:" + height + "px\" type=\"application/x-mplayer2\" pluginspage=\"http://microsoft.com/windows/mediaplayer/en/download/\""
                    + " src=\"" + src + "\" >"
                    + " </EMBED></OBJECT>";
            else
                vlreturn = "<A class=\"lnkpreview\" href=\"" + src + "\"><IMG src=\"../images/download.png\" border=\"0\" align=\"absmiddle\"></A>";
            return vlreturn;
        }
        public static bool isExternallink(string fileurl)
        {
            if (fileurl.IndexOf("http://") != -1 || fileurl.IndexOf("https://") != -1 || fileurl.IndexOf("ftp://") != -1) return true;
            return false;
        }

        public static string Get_Videoplayer(string sourcetype, string url, int playerwidth)
        {
            if (IsNullOrEmpty(sourcetype) || IsNullOrEmpty(url)) return "";

            string urlsource, player = string.Empty;
            switch (sourcetype)
            {
                case "youtube.com":
                    urlsource = url.Replace("youtu.be/", "www.youtube.com/embed/");
                    player = "<iframe width=\"" + playerwidth + "\" height=\"" + (playerwidth * 21 / 28) + "\" src=\"" + urlsource + "\" frameborder=\"0\" allowfullscreen></iframe>";
                    break;
                case "clip.vn":
                    urlsource = url.Replace(url.Substring(0, url.LastIndexOf(",") + 1), "http://clip.vn/embed/");
                    player = "<iframe width=\"" + playerwidth + "\" height=\"" + (playerwidth * 21 / 28) + "\" src=\"" + urlsource + "\" frameborder=\"0\" allowfullscreen></iframe>";
                    break;
                default:
                    break;
            }

            return player;
        }
        
        private const string extension_showonweb = ".gif.jpg.jpeg.png.swf.flv.wmv.mpg.mpeg.mpe.asf.asx.wax.wmv.wmx.avi.mp3";
        public static bool CanShowOnWeb(string filepreview)
        {
            if (IsNullOrEmpty(filepreview)) return false;
            int dotindex = filepreview.LastIndexOf(".");
            if (dotindex == -1) return false;

            string extension = filepreview.Substring(dotindex).ToLower();
            if (extension_showonweb.IndexOf(extension) > -1)
                return true;
            
            return false;
        }
        
        public static string[] analys_keyword(string pattern)
        {
            if (pattern == string.Empty) return null;
            string keyword = pattern.Trim();
            string qoute = "\"", split = "$", vltemp = "";
            string[] vlreturn;
            int ista = keyword.IndexOf(qoute);
            while (ista != -1)
            {
                int iend = keyword.IndexOf(qoute, ista + 1);
                if (iend == -1) break;
                string temp = keyword.Substring(ista + 1, iend - ista - 1).Trim();
                vltemp += split + remove_blank(temp);
                keyword = keyword.Replace(qoute + temp + qoute, "").Trim(); //keyword.Remove(ista, iend-ista+1);
                ista = keyword.IndexOf(qoute);
            }
            keyword = remove_blank(keyword).Replace(" ", split);
            keyword += vltemp;
            vlreturn = keyword.Split(Convert.ToChar(split));
            return vlreturn;
        }
        public static string remove_blank(string pattern)
        {
            if (IsNullOrEmpty(pattern)) return "";
            string keyword = pattern.Trim();
            string[] arr = keyword.Split(Convert.ToChar(" "));
            string vlreturn = "";
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Trim() == "") continue;
                vlreturn += arr[i].Trim() + " ";
            }
            return vlreturn.Trim();
        }
        public static string remove_html(string pattern)
        {
            if (IsNullOrEmpty(pattern)) return "";
            string keyword = pattern.Trim();
            string qoute1 = "<", qoute2 = ">";
            int ista = keyword.IndexOf(qoute1);
            while (ista != -1)
            {
                int iend = keyword.IndexOf(qoute2, ista + 1);
                if (iend == -1) break;
                keyword = keyword.Remove(ista, iend - ista + 1);
                ista = keyword.IndexOf(qoute1);
            }
            keyword = remove_blank(keyword);
            return keyword.Trim();
        }
        public static string remove_specialcharacters(string pattern)
        {
            if (pattern == null || IsNullOrEmpty(pattern.ToString())) return "";
            string vlreturn = pattern.ToString();
            const string FindText = "`~!@#$%^&*()-_=+[{]}\\|;:'\",<.>/?";
            int index = -1;
            while ((index = vlreturn.IndexOfAny(FindText.ToCharArray())) != -1)
            {
                vlreturn = vlreturn.Remove(index, 1);
            }
            return vlreturn.Trim();
        }
        public static string to_lower(string pattern)
        {
            pattern = pattern.ToLower();
            const string FindText = "ÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            const string ReplText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵ";
            int index = -1;
            while ((index = pattern.IndexOfAny(FindText.ToCharArray())) != -1)
            {
                int index2 = FindText.IndexOf(pattern[index]);
                pattern = pattern.Replace(pattern[index], ReplText[index2]);
            }
            return pattern;
        }
        public static string to_nonunicode(string pattern)
        {
            const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
            int index = -1;
            while ((index = pattern.IndexOfAny(FindText.ToCharArray())) != -1)
            {
                int index2 = FindText.IndexOf(pattern[index]);
                pattern = pattern.Replace(pattern[index], ReplText[index2]);
            }
            return pattern;
        }
        public static string install_keyword(string pattern)
        {
            if (IsNullOrEmpty(pattern)) return "";
            return CFunctions.to_nonunicode(remove_html(pattern)).ToLower();
        }

        public static string cleaner_urlname(object pattern)
        {
            if (pattern == null || IsNullOrEmpty(pattern.ToString())) return "";
            string vlreturn = remove_specialcharacters(pattern.ToString());
            vlreturn = remove_blank(vlreturn).Replace(" ", "-");
            return vlreturn + ConfigurationSettings.AppSettings["SEOExtension"];
        }
        public static string install_urlname(object pattern)
        {
            if (pattern==null || IsNullOrEmpty(pattern.ToString())) return "";
            string vlreturn = cleaner_urlname(to_nonunicode(pattern.ToString()));
            return Convert_Chuoi_Khong_Dau(vlreturn);
        }
        public static string convertUnicodeToAscii(string unicodeString)
        {
            // Create two different encodings.
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;
            unicodeString = unicodeString.Replace('\u201d','?');
            unicodeString = unicodeString.Replace('\u201c', '?');
            unicodeString = unicodeString.Replace('\u005F', '?');
            // Convert the string into a byte[].“
            byte[] unicodeBytes = unicode.GetBytes(unicodeString);

            // Perform the conversion from one encoding to the other.
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

            // Convert the new byte[] into a char[] and then into a string.
            // This is a slightly different approach to converting to illustrate
            // the use of GetCharCount/GetChars.
            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);
            // Display the strings created before and after the conversio            
            return asciiString;
        }
        public static string Convert_Chuoi_Khong_Dau(string s)
        {
            s = convertUnicodeToAscii(s);
            s = s.Trim().Replace("  ", " ").Replace(",", "");
            s = s.Replace(" ", "-");
            s = s.Replace(":", "-");
            s = s.Replace("&", "");
            s = s.Replace("_", "");
            s = s.Replace("'", "");
            s = s.Replace("\"", "");
            s = s.Replace("“", "");
            s = s.Replace("”", "");
            s = s.Replace("?", "");
            s = s.Replace("`", "");
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string strFormD = s.Normalize(NormalizationForm.FormD);
            string result = regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            result = result.Replace(":", "").Replace("\"", "");
            result = result.ToLower();
            return result;
        }
        public static string install_metatag(object pattern)
        {
            if (pattern == null || IsNullOrEmpty(pattern.ToString())) return "";
            string pattern_origin = pattern.ToString();
            pattern_origin = remove_blank(remove_html(pattern_origin));
            string pattern_nounicode = to_nonunicode(remove_specialcharacters(pattern_origin));
            string vlreturn = pattern_origin + " - " + pattern_nounicode;
            return vlreturn;
        }

        public static string MBEncrypt(string pattern)
        {
            try
            {
                byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(pattern);
                string encrypted = Convert.ToBase64String(b);
                return encrypted;
            }
            catch
            {
                return "";
            }
        }
        public static string MBDecrypt(string pattern)
        {
            try
            {
                byte[] b = Convert.FromBase64String(pattern);
                string decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);
                return decrypted;
            }
            catch
            {
                return "";
            }
        }

        public static string Expression_GetSort(string sortexp, string sortdir)
        {
            return ", ROW_NUMBER() OVER(ORDER BY " + (sortdir == "Random" ? ("NEWID()") : ("A." + sortexp + (sortdir == "Ascending" ? " ASC" : " DESC"))) + ") AS rownumber";
        }
        public static string Expression_GetLimit(int pageindex, int pagesize)
        {
            return (pageindex == 0 && pagesize == 0) ? "" : " AND rownumber BETWEEN " + ((pageindex - 1) * pagesize + 1) + " AND " + pageindex * pagesize;
        }
        public static string Expression_GetPermit(bool getall, string username)
        {
            return getall ? "" : " AND A.username='" + username + "'";
        }

        public static string Get_Definecatrelate(int did, string what)
        {
            try
            {
                string tablename = "", pagename = "", recursive = "";
                switch (did)
                {
                    case Webcmm.Id.Menutypeof:
                        tablename = Webcmm.Table.Menutypeof;
                        pagename = Webcmm.Page.Menutypeof;
                        break;
                    case Webcmm.Id.Menu:
                        tablename = Webcmm.Table.Menu;
                        pagename = Webcmm.Page.Menu;
                        recursive = Queryparam.Defstring.Recursive;
                        break;
                    case Webcmm.Id.User:
                        tablename = Webcmm.Table.User;
                        pagename = Webcmm.Page.User;
                        recursive = Queryparam.Defstring.Recursive;
                        break;
                    case Webcmm.Id.Userlog:
                        tablename = Webcmm.Table.Userlog;
                        pagename = Webcmm.Page.Userlog;
                        break;
                    case Webcmm.Id.Userright:
                        tablename = Webcmm.Table.Userright;
                        pagename = Webcmm.Page.Userright;
                        break;
                    case Webcmm.Id.Member:
                        tablename = Webcmm.Table.Member;
                        pagename = Webcmm.Page.Member;
                        break;
                    case Webcmm.Id.Categorytypeof:
                        tablename = Webcmm.Table.Categorytypeof;
                        pagename = Webcmm.Page.Categorytypeof;
                        break;
                    case Webcmm.Id.Category:
                        tablename = Webcmm.Table.Category;
                        pagename = Webcmm.Page.Category;
                        recursive = Queryparam.Defstring.Recursive;
                        break;
                    case Webcmm.Id.Categoryattr:
                        tablename = Webcmm.Table.Categoryattr;
                        pagename = Webcmm.Page.Categoryattr;
                        recursive = Queryparam.Defstring.Recursive;
                        break;
                    case Webcmm.Id.Symbol:
                        tablename = Webcmm.Table.Symbol;
                        pagename = Webcmm.Page.Symbol;
                        break;
                    case Webcmm.Id.Fileattach:
                        tablename = Webcmm.Table.Fileattach;
                        pagename = Webcmm.Page.Fileattach;
                        break;
                    case Webcmm.Id.Staticcontent:
                        tablename = Webcmm.Table.Staticcontent;
                        pagename = Webcmm.Page.Staticcontent;
                        break;
                    case Webcmm.Id.Accesscounter:
                        tablename = Webcmm.Table.Accesscounter;
                        pagename = Webcmm.Page.Accesscounter;
                        break;
                    case Webcmm.Id.Comment:
                        tablename = Webcmm.Table.Comment;
                        pagename = Webcmm.Page.Comment;
                        break;
                    case Webcmm.Id.Banner:
                        tablename = Webcmm.Table.Banner;
                        pagename = Webcmm.Page.Banner;
                        break;

                    case Webcmm.Id.News:
                        tablename = Webcmm.Table.News;
                        pagename = Webcmm.Page.News;
                        break;
                    case Webcmm.Id.Product:
                        tablename = Webcmm.Table.Product;
                        pagename = Webcmm.Page.Product;
                        break;
                    case Webcmm.Id.Libraries:
                        tablename = Webcmm.Table.Libraries;
                        pagename = Webcmm.Page.Libraries;
                        break;
                    case Webcmm.Id.Video:
                        tablename = Webcmm.Table.Video;
                        pagename = Webcmm.Page.Video;
                        break;
                    case Webcmm.Id.Feedback:
                        tablename = Webcmm.Table.Feedback;
                        pagename = Webcmm.Page.Feedback;
                        recursive = Queryparam.Defstring.Recursive;
                        break;
                    case Webcmm.Id.Supportonline:
                        tablename = Webcmm.Table.Supportonline;
                        pagename = Webcmm.Page.Supportonline;
                        break;
                    case Webcmm.Id.RSSResource:
                        tablename = Webcmm.Table.RSSResource;
                        pagename = Webcmm.Page.RSSResource;
                        break;

                    case Webcmm.Id.Settingsite_MailServer:
                        tablename = Webcmm.Table.Settingsite_MailServer;
                        pagename = Webcmm.Page.Settingsite_MailServer;
                        break;
                };
                if (what == Queryparam.Defstring.Table) return (CConstants.TBDBPREFIX + tablename);
                else if (what == Queryparam.Defstring.Page) return pagename;
                else if (what == Queryparam.Defstring.Recursive) return recursive;
                else return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static int Get_Definecat(string pagename)
        {
            try
            {
                int did = -1;
                switch (pagename)
                {
                    case Webcmm.Page.Menutypeof:
                        did = Webcmm.Id.Menutypeof;
                        break;
                    case Webcmm.Page.Menu:
                        did = Webcmm.Id.Menu;
                        break;
                    case Webcmm.Page.User:
                        did = Webcmm.Id.User;
                        break;
                    case Webcmm.Page.Userlog:
                        did = Webcmm.Id.Userlog;
                        break;
                    case Webcmm.Page.Userright:
                        did = Webcmm.Id.Userright;
                        break;
                    case Webcmm.Page.Member:
                        did = Webcmm.Id.Member;
                        break;
                    case Webcmm.Page.Categorytypeof:
                        did = Webcmm.Id.Categorytypeof;
                        break;
                    case Webcmm.Page.Category:
                        did = Webcmm.Id.Category;
                        break;
                    case Webcmm.Page.Categoryattr:
                        did = Webcmm.Id.Categoryattr;
                        break;
                    case Webcmm.Page.Symbol:
                        did = Webcmm.Id.Symbol;
                        break;
                    case Webcmm.Page.Fileattach:
                        did = Webcmm.Id.Fileattach;
                        break;
                    case Webcmm.Page.Staticcontent:
                        did = Webcmm.Id.Staticcontent;
                        break;
                    case Webcmm.Page.Accesscounter:
                        did = Webcmm.Id.Accesscounter;
                        break;
                    case Webcmm.Page.Comment:
                        did = Webcmm.Id.Comment;
                        break;
                    case Webcmm.Page.Banner:
                        did = Webcmm.Id.Banner;
                        break;

                    case Webcmm.Page.News:
                        did = Webcmm.Id.News;
                        break;
                    case Webcmm.Page.Product:
                        did = Webcmm.Id.Product;
                        break;
                    case Webcmm.Page.Libraries:
                        did = Webcmm.Id.Libraries;
                        break;
                    case Webcmm.Page.Video:
                        did = Webcmm.Id.Video;
                        break;
                    case Webcmm.Page.Feedback:
                        did = Webcmm.Id.Feedback;
                        break;
                    case Webcmm.Page.Supportonline:
                        did = Webcmm.Id.Supportonline;
                        break;
                    case Webcmm.Page.RSSResource:
                        did = Webcmm.Id.RSSResource;
                        break;

                    case Webcmm.Page.Settingsite_MailServer:
                        did = Webcmm.Id.Settingsite_MailServer;
                        break;
                };
                return did;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool GetViewSetting(int belongto, string viewtype)
        {
            string Setting = ConfigurationSettings.AppSettings[belongto.ToString()].ToString();
            char vlreturn = '0';
            switch (viewtype)
            {
                case ViewSetting.ListByCategory:
                    vlreturn = Setting[0];
                    break;
                case ViewSetting.ListVisibled:
                    vlreturn = Setting[1];
                    break;
                case ViewSetting.ListWhenCategoryBrowse:
                    vlreturn = Setting[2];
                    break;
                case ViewSetting.ListFollowed:
                    vlreturn = Setting[3];
                    break;
            }
            return (vlreturn == '0' ? false : true);
        }
        public static int Langindex(string lang)
        {
            return 0;
        }

        public static int DayOfWeek(DateTime datein)
        {
            if (datein.Equals(new DateTime(0)))
                return -1;
            int dayofweek = -1;
            switch (datein.DayOfWeek)
            {
                case System.DayOfWeek.Monday:
                    dayofweek = 0;
                    break;
                case System.DayOfWeek.Tuesday:
                    dayofweek = 1;
                    break;
                case System.DayOfWeek.Wednesday:
                    dayofweek = 2;
                    break;
                case System.DayOfWeek.Thursday:
                    dayofweek = 3;
                    break;
                case System.DayOfWeek.Friday:
                    dayofweek = 4;
                    break;
                case System.DayOfWeek.Saturday:
                    dayofweek = 5;
                    break;
                case System.DayOfWeek.Sunday:
                    dayofweek = 6;
                    break;
            }
            return dayofweek;
        }
        public static DateTime FirstDayOfWeek(DateTime datein)
        {
            if (datein.Equals(new DateTime(0)))
                return new DateTime(0);

            int dayofweek = DayOfWeek(datein);
            return datein.AddDays(dayofweek - dayofweek * 2);
        }
        public static DateTime LastDayOfWeek(DateTime datein)
        {
            if (datein.Equals(new DateTime(0)))
                return new DateTime(0);

            int dayofweek = DayOfWeek(datein);
            return datein.AddDays(6 - dayofweek);
        }

        public static object SetDBString(string param)
        {
            if (param == null) 
                return DBNull.Value;
            else 
                return param;
        }
        public static object SetDBDatetime(DateTime param)
        {
            if (param.Equals(new DateTime(0))) 
                return DBNull.Value;
            else 
                return param;
        }
    }
}
