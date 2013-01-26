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
    public partial class ucfilepreview : BaseUserControl
    {
        public string Up
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FP_txtUp.Value = Up;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "FPSelectsource", "FPSelectsource();", true);
        }

        #region public methods
        public bool Set(string filepath)
        {
            try
            {
                if (!CFunctions.IsNullOrEmpty(filepath))
                {
                    FP_txtFileurl.Value = filepath;
                    FP_fileLink.Text = CFunctions.Get_Filepreviewurl(filepath);
                    FP_fileLink.NavigateUrl = FP_fileLink.Text;
                    FP_filePreview.ImageUrl = FP_fileLink.Text;
                    FP_fileRemove.Text = "x";
                }
                else
                {
                    FP_txtFileurl.Value = FP_fileLink.Text = FP_fileLink.NavigateUrl = FP_fileRemove.Text = "";
                    FP_filePreview.ImageUrl = "~/commup/no_image.gif";
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string Get()
        {
            return FP_txtFileurl.Value;
        }
        #endregion

    }
}