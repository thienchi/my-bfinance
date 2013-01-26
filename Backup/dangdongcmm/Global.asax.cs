using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;

namespace dangdongcmm
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            Application["CurrentAccess"] = 0;
            Application["RestrictedPages"] = "";
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            int counter = (int)Application["CurrentAccess"];
            counter++;
            Application["CurrentAccess"] = counter;
            Application.UnLock();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            int counter = (int)Application["CurrentAccess"];
            counter--;
            Application["CurrentAccess"] = counter;
            Application.UnLock();
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}