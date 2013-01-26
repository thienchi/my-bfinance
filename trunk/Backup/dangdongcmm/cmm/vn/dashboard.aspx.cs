using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

using dangdongcmm.model;
using dangdongcmm.utilities;
using dangdongcmm.dal;

namespace dangdongcmm.cmm
{
    public partial class dashboard : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Load_Loginfirst();

                if (CCommon.Right_sys())
                {
                    int count_feedback = (new CFeedback()).Countnew();
                    Count_Feedback.Text = count_feedback > 0 ? Count_Feedback.Text.Replace("$COUNT$", count_feedback.ToString()) : "";

                    int count_updating = this.Count_Watting((int)CConstants.State.Status.Waitactive);
                    Count_Updating.Text = count_updating > 0 ? Count_Updating.Text.Replace("$COUNT$", count_updating.ToString()) : "";
                    int count_deleting = this.Count_Watting((int)CConstants.State.Status.Waitdelete);
                    Count_Deleting.Text = count_deleting > 0 ? Count_Deleting.Text.Replace("$COUNT$", count_deleting.ToString()) : "";
                    pnlForAdmin.Visible = (count_feedback > 0 || count_updating > 0 || count_deleting > 0);
                }

            }
        }

        #region private methods
        private void Load_Loginfirst()
        {
            UserInfo info = CCommon.Get_CurrentUser();
            if (info != null && info.Loginfirst == 0)
            {
                pnlLoginfirst.Visible = true;
                lblUsername.Text = info.Username;
                new CUser().Updatenum(info.Id.ToString(), "loginfirst", 1);
            }
        }

        private int Count_Watting(int status)
        {
            int Count = 0;
            List<CategorytypeofInfo> typeof_list = (new CCategorytypeof(CCommon.LANG)).Wcmm_Getlist((int)CConstants.State.Status.None, (int)CConstants.State.MarkAs.Focus);
            if (typeof_list == null) return 0;

            foreach (CategorytypeofInfo typeof_info in typeof_list)
            {
                int Countin = (new CGeneral(CCommon.LANG, typeof_info.Id)).Waitting_Count(status);
                Count += Countin;
            }
            return Count;
        }
        #endregion
    }
}
