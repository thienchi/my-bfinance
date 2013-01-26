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
    public partial class ucaccesscounter : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Count_Total();
                this.Count_Current();
            }
        }

        #region private methods
        private void Count_Current()
        {
            try
            {
                currentaccess.Text = ((int)Application["CurrentAccess"]).ToString();

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
                currentaccess.Text = ((int)Application["CurrentAccess"]).ToString();

                CAccesscounter DAL = new CAccesscounter();
                AccesscounterInfo icounter = DAL.Getinfototal();
                if (icounter == null)
                    icounter = new AccesscounterInfo();
                
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