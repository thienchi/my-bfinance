using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.IO;

using dangdongcmm.model;
using dangdongcmm.utilities;
using dangdongcmm.dal;

namespace dangdongcmm.cmm
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class FPRemoveHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string fileURL = context.Request.QueryString["fileurl"];
            string pid = context.Request.QueryString["pid"];
            string iid = context.Request.QueryString["iid"];
            if (CFunctions.IsNullOrEmpty(fileURL)) return;

            if (pid == "0" || (pid != "0" && iid == "0"))
            {
                FileInfo fileInfo = new FileInfo(context.Server.MapPath("../" + fileURL));
                if (fileInfo != null || fileInfo.Exists)
                    fileInfo.Delete();
                context.Response.ContentType = "text/plain";
                context.Response.Write("{\"name\":\"done\"}");
                return;
            }

            if (!CFunctions.IsNullOrEmpty(iid) && iid != "0")
            {
                new CFileattach(CCommon.LANG).Delete(iid);
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write("{\"name\":\"done\"}");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
