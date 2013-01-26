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

namespace dangdongcmm
{
    public partial class ucaccesscounter : BaseUserControl
    {
        private CAccesscounter DAL;

        protected void Page_Load(object sender, EventArgs e)
        {
            DAL = new CAccesscounter();
            if (!Page.IsPostBack)
            {
                if (Session["ACCESSCOUNTER"] == null)
                {
                    DAL.Updatetotal();
                    Session.Add("ACCESSCOUNTER", 1);
                }

                this.Count_Total();
                this.Count_Current();
                //this.Count_Pageview();
            }
        }

        #region private methods
        private void Count_Current()
        {
            try
            {
                currentaccess.Text = ((int)Application["CurrentAccess"] + CConstants.ACCESS_CURRENT).ToString();

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Count_Pageview()
        {
            try
            {
                string page = Request.Url.PathAndQuery;
                pageview.Text = DAL.Updatecouter(page).ToString();
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Count_Total()
        {
            try
            {
                AccesscounterInfo icounter = DAL.Getinfototal();
                if (icounter == null)
                    icounter = new AccesscounterInfo();
                icounter.Counter_Total += CConstants.ACCESS_TOTAL;

                List<AccesscounterInfo> list = new List<AccesscounterInfo>();
                list.Add(icounter);
                (new GenericList<AccesscounterInfo>()).Bind_DataList(rptList, null, list, 0);

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}