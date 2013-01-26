using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace dangdongcmm.utilities
{
    public class CConstants
    {
        #region constants
        public const int NUM_DIRECTLY = -1000;
        public const int NUM_INCREASE = -1001;
        public const int NUM_DECREASE = -1002;
        public const int NUM_INCREASEADD = -1003;
        public const int NUM_DECREASESUB = -1004;

        public const string PRE_FOLDER_WEBCMM = "../../";
        #endregion

        public static string WEBSITE = ConfigurationSettings.AppSettings["WEBSITE"];
        public static int LANG_NUM = Convert.ToInt32(ConfigurationSettings.AppSettings["LANG_NUM"]);
        public static string LANG_TXT = ConfigurationSettings.AppSettings["LANG_TXT"];
        public static string LANG_DEF = ConfigurationSettings.AppSettings["LANG_0"];
        public static string TBDBPREFIX = ConfigurationSettings.AppSettings["TBDBPREFIX"];
        public static string CARTPREFIX = ConfigurationSettings.AppSettings["CARTPREFIX"];

        public static string FORMAT_TIME = "T"; //ConfigurationSettings.AppSettings["FORMAT_TIME"];
        public static string FORMAT_DATE = "MM/dd/yyyy";  //ConfigurationSettings.AppSettings["FORMAT_DATE"];

        public static string PAGE_WELCOMEDEF = "medashboard.aspx";
        public static int REGISTERCONFIRM = 1;
        public static int CHECKLOGINFORDOWNLOAD = 0;
        public static int PAGESIZE = 20;

        public static int FILEUPLOAD_SIZE = Convert.ToInt32(ConfigurationSettings.AppSettings["FILEUPLOAD_SIZE"]);  //  *1024.000;
        public static bool FILEUPLOAD_THUMBNAIL = Convert.ToInt32(ConfigurationSettings.AppSettings["FILEUPLOAD_THUMBNAIL"]) == 1 ? true : false;

        public static int ACCESS_TOTAL = 0;
        public static int ACCESS_CURRENT = 0;

        public class State
        {
            public enum MarkAs : int
            {
                None = (int)0,
                OnHome = (int)1,
                Focus = (int)2,
                TopOnPage = (int)3,
            }
            public enum Status : int
            {
                None = (int)-1,
                Waitactive = (int)0,
                Actived = (int)1,
                Waitdelete = (int)2,
                Deleted = (int)3,
                Disabled = (int)4
            }
            public enum Existed : int
            {
                None = (int)0,
                Code = (int)1,
                Name = (int)2,
                Mail = (int)3,
                Phone = (int)4
            }
            public enum InfoStatus : int
            {
                None = (int)0,
                New = (int)1,
                Update = (int)2,
                Delete = (int)3
            }
            public enum Orderstatus : int
            {
                None = (int)-1,
                NewOrders = (int)0,
                InProduction = (int)1,
                Delivered = (int)2,
                Return = (int)3,
                Pending = (int)4,
                Completed = (int)6
            }
        }
    }

    public class ViewSetting
    {
        public const string ListByCategory = "ListByCategory";
        public const string ListVisibled = "ListVisibled";
        public const string ListWhenCategoryBrowse = "ListWhenCategoryBrowse";
        public const string ListFollowed = "ListFollowed";
    }

    public class Webcmm
    {
        public class Id
        {
            public const int Menutypeof = 200;
            public const int Menu = 201;
            public const int User = 100;
            public const int Userlog = 101;
            public const int Userright = 102;
            public const int Member = 300;
            public const int National = 301;
            public const int District = 302;
            public const int Profession = 303;
            public const int Paymenttype = 304;

            public const int General = 0;
            public const int Categorytypeof = 1;
            public const int Category = 2;
            public const int Categoryattr = 7;
            public const int Symbol = 3;
            public const int Fileattach = 4;
            public const int Staticcontent = 5;
            public const int Accesscounter = 9;
            public const int Comment = 6;
            public const int Banner = 8;
            
            public const int News = 12;
            public const int Product = 10;
            public const int Libraries = 18;
            public const int Video = 19;
            public const int Feedback = 14;
            public const int Supportonline = 15;
            public const int RSSResource = 21;

            public const int Settingsite_MailServer = 31;
        }
        public class Page
        {
            public const string Menutypeof = "menutypeof.aspx";
            public const string Menu = "menu.aspx";
            public const string User = "user.aspx";
            public const string Userlog = "userlog.aspx";
            public const string Userright = "userright.aspx";
            public const string Member = "member.aspx";
            
            public const string General = "general.aspx";
            public const string Categorytypeof = "categorytypeof.aspx";
            public const string Category = "category.aspx";
            public const string Categoryattr = "categoryattr.aspx";
            public const string Symbol = "symbol.aspx";
            public const string Fileattach = "albummanagement.aspx";
            public const string Staticcontent = "staticcontent.aspx";
            public const string Accesscounter = "accesscounter.aspx";
            public const string Comment = "comment.aspx";
            public const string Banner = "banner.aspx";
            
            public const string News = "news.aspx";
            public const string Product = "product.aspx";
            public const string Libraries = "libraries.aspx";
            public const string Video = "video.aspx";
            public const string Feedback = "feedback.aspx";
            public const string Supportonline = "supportonline.aspx";
            public const string RSSResource = "rssresource.aspx";
            
            public const string Settingsite_MailServer = "settingsite.aspx";
        }
        public class Table
        {
            public const string Menutypeof = "menutypeof";
            public const string Menu = "menu";
            public const string User = "user";
            public const string Userlog = "userlog";
            public const string Userright = "userright";
            public const string Member = "member";
            
            public const string General = "general";
            public const string Categorytypeof = "categorytypeof";
            public const string Category = "category";
            public const string Categoryattr = "categoryattr";
            public const string Symbol = "symbol";
            public const string Fileattach = "fileattach";
            public const string Staticcontent = "staticcontent";
            public const string Accesscounter = "accesscounter";
            public const string Comment = "comment";
            public const string Banner = "banner";
            
            public const string News = "news";
            public const string Product = "product";
            public const string Libraries = "libraries";
            public const string Video = "video";
            public const string Feedback = "feedback";
            public const string Supportonline = "supportonline";
            public const string RSSResource = "rssresource";

            public const string Settingsite_MailServer = "settingsite_mailserver";
        }
    }

    public class Commandparam
    {
        public const string Delete = "Delete";
        public const string Copy = "Copy";
        public const string Move = "Move";
        public const string View = "View";
        public const string Addnew = "Addnew";
        public const string Displaysetting = "Displaysetting";
        public const string Updatedisplayorder = "Updatedisplayorder";
        public const string Updatetaskstatus = "Updatetaskstatus";
        public const string Updaterating = "Updaterating";
        public const string Setupattribute = "Setupattribute";
        public const string Exportexcel = "Exportexcel";
    }

    public class Queryparam
    {
        public const string P = "p";	// page view: u, l, v
        public class Pval
        {
            public const string U = "u";
            public const string L = "l";
            public const string V = "v";
        }

        public const string Iid = "iid";	// item id
        public const string Pid = "pid";	// parent id
        public const string Cid = "cid";	// category id
        public const string Ted = "ted";	// temporary id
        public const string Tab = "tab";	// tab index
        public const string Pageindex = "pag";	// temporary id
        public const string Belongto = "blto";	// category id
        public const string Keywords = "keywords";	// category id
        public const string Code = "code";	// code
        public const string User = "user";	// code
        
        public const string Chkcheck = "chkcheck";
        public const string Chkcategorymulti = "chkcategorymulti";
        public const string Chkcategoryattr = "chkcategoryattr";

        public const string Problem = "problem";
        
        public class Sqlcolumn
        {
            public const string Id = "id";
            public const string Username = "username";
            public const string Status = "status";
            public const string Iconex = "iconex";
            public const string Markas = "markas";
            public const string Pis = "pis";
            public const string Orderd = "orderd";
            public const string Filepreview = "filepreview";
            public const string Viewcounter = "viewcounter";
            public const string Allowcomment = "allowcomment";
            public const string Album = "album";
            public const string Taskstatus = "taskstatus";
            public const string Timeupdate = "timeupdate";
            public const string Dateposted = "dateposted";
            public const string Dateassigned = "dateassigned";
            public const string Datecompleted = "datecompleted";
            public const string Dateclosed = "dateclosed";
            public const string Balance = "balance";
        }

        public class Varstring
        {
            public const string VAR_TABLENAME = "$VAR_TABLENAME$";
            public const string VAR_SORTEXPRESSION = "$VAR_SORTEXPRESSION$";
            public const string VAR_SORTLIMITATION = "$VAR_SORTLIMITATION$";
            public const string VAR_JOINEXPRESSION = "$VAR_JOINEXPRESSION$";
            public const string VAR_LANG = "$VAR_LANG$";
            
            public const string Id = "$VAR_ID$";
            public const string Name = "$VAR_NAME$";
            public const string Depth = "$VAR_DEPTH$";
            public const string Path = "$VAR_PATH$";
        }

        public class Defstring
        {
            public const string Table = "table";
            public const string Page = "page";
            public const string Recursive = "recursive";

            public const string Nosymbol = "nosymbol";
            public const int None = -1;
            public const int Nospecifyint = -2;
            public const string Nospecifystr = "-2";

            public const string AddtoCart = "AddtoCart";
            public const string SubfrCart = "SubfrCart";
            public const string RemovefromCart = "RemovefromCart";
        }
    }

    public class Sessionparam
    {
        public const string USERLOGIN = "USERLOGIN";
        public const string USERREGISTER = "USERREGISTER";
        public const string VERIFYCODE = "VERIFYCODE";
        public const string PREVIOUSURL = "PREVIOUSURL";

        public const string WEBUSERLOGIN = "WEBUSERLOGIN";
        public const string WEBUSERREGISTER = "WEBUSERREGISTER";
        public const string WEBUSERGETPASSWORD = "WEBUSERGETPASSWORD";
        public const string WEBUSERCOMMENT = "WEBUSERCOMMENT";
        public const string WEBPREVIOUSURL = "WEBPREVIOUSURL";

        public const string TRANSACTION = "TRANSACTION";
        public const string ERROR = "ERROR";

        public const string LANG = "LANG";
        public const string LANGCMM = "LANGCMM";

        public const string CART = "CART";
        
    }

    public class Symbolchar
    {
        public const string Sep_Ddlprefix = "---";
        public const string Sep_Treefirst = "";
        public const string Sep_Treeprefix = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
        public const string Sep_Path = "/";
    }

    public class Errortype
    {
        public const string Notice = "notice";
        public const string Warning = "warning";
        public const string Completed = "completed";
        public const string Error = "error";
    }

    public class DDir
    {
        public const string FOLDER_HTML = "html/";
        public const string FOLDER_MAIL = "mail/";
        public const string FOLDER_NOTICE = "notice/";
        public const string FOLDER_TEMPLATE = "template/";
    }
}
