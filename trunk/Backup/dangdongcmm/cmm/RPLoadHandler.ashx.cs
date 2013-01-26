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
    public class RPLoadHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string cid = context.Request.QueryString["cid"];
            string vlreturn = "";
            if (!CFunctions.IsNullOrEmpty(cid) && cid != "0")
            {
                vlreturn = "{\"productinfo\":[";
                List<ProductInfo> list = new CProduct(CCommon.LANG).Wcmm_Getlist(int.Parse(cid));
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ProductInfo info = (ProductInfo)list[i];
                        vlreturn += "{\"productname\":\"" + info.Name.Replace("\"", "\\\"") + "\", \"productid\":\"" + info.Id + "\"}" + (i == list.Count - 1 ? "" : ",");
                    }
                }
                vlreturn += "]}";
            }

            string iid = context.Request.QueryString["iid"];
            if (!CFunctions.IsNullOrEmpty(iid))
            {
                vlreturn = "{\"productinfo\":[";
                List<ProductInfo> list = new CProduct(CCommon.LANG).Wcmm_Getlist(iid);
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ProductInfo info = (ProductInfo)list[i];
                        vlreturn += "{\"productname\":\"" + info.Name.Replace("\"", "\\\"") + "\", \"productid\":\"" + info.Id + "\", \"productimage\":\"" + info.Filepreview + "\", \"productcategory\":\"" + info.Cname + "\"}" + (i == list.Count - 1 ? "" : ",");

                        //vlreturn += "<tr id=\"rprowi" + i + "\" class=\"rprowi\">"
                        //    + "<td class=\"rpbase\"><img id=\"rpimag" + i + "\" src=\"" + (proinfo.Filepreview.IndexOf(WEBSITE) == 0 ? proinfo.Filepreview : (WEBSITE + "/" + proinfo.Filepreview)) + "\" /></td>"
                        //+ "<td class=\"rpnote\">" + proinfo.Name + "</td>"
                        //+ "<td title=\"" + proinfo.Id + "\" class=\"rpcomd\"><a href=\"javascript:RPremove(" + proinfo.Id + "," + i + ");\">x</a></td></tr>";
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
