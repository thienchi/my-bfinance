using System;
using System.Collections;
using System.Collections.Generic;
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
    public class RNLoadHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string cid = context.Request.QueryString["cid"];
            string vlreturn = "";
            if (!CFunctions.IsNullOrEmpty(cid) && cid != "0")
            {
                vlreturn = "{\"newsinfo\":[";
                List<NewsInfo> list = new CNews(CCommon.LANG).Wcmm_Getlist(int.Parse(cid));
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        NewsInfo info = (NewsInfo)list[i];
                        vlreturn += "{\"newsname\":\"" + info.Name.Replace("\"", "\\\"") + "\", \"newsid\":\"" + info.Id + "\"}" + (i == list.Count - 1 ? "" : ",");
                    }
                }
                vlreturn += "]}";
            }

            string iid = context.Request.QueryString["iid"];
            if (!CFunctions.IsNullOrEmpty(iid))
            {
                vlreturn = "{\"newsinfo\":[";
                List<NewsInfo> list = new CNews(CCommon.LANG).Wcmm_Getlist(iid);
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        NewsInfo info = (NewsInfo)list[i];
                        vlreturn += "{\"newsname\":\"" + info.Name.Replace("\"", "\\\"") + "\", \"newsid\":\"" + info.Id + "\", \"newscategory\":\"" + info.Cname + "\"}" + (i == list.Count - 1 ? "" : ",");
                    }
                }
                vlreturn += "]}";
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(vlreturn);
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
