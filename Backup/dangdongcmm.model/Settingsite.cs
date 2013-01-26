using System;
using System.Collections.Generic;
using System.Text;

using dangdongcmm.utilities;

namespace dangdongcmm.model
{
    public class Settingsite
    {
        public class MailServer
        {
            public MailServer()
            {
                //
                // TODO: Add constructor logic here
                //
            }

            #region properties
            public int Id
            {
                get;
                set;
            }
            public string SMTPServer
            {
                get;
                set;
            }
            public int SMTPPort
            {
                get;
                set;
            }
            public int UseSSL
            {
                get;
                set;
            }
            public string Receiver
            {
                get;
                set;
            }
            public string Username
            {
                get;
                set;
            }
            public string Password
            {
                get;
                set;
            }
            public DateTime Timeupdate
            {
                get;
                set;
            }
            
            public string eTimeupdate
            {
                get
                {
                    return Timeupdate.Equals(new DateTime(0)) ? string.Empty : Timeupdate.ToString(CConstants.FORMAT_TIME) + " " + Timeupdate.ToString(CConstants.FORMAT_DATE);
                }
            }
            public string eTimeupdateshort
            {
                get
                {
                    return Timeupdate.Equals(new DateTime(0)) ? string.Empty : Timeupdate.ToString(CConstants.FORMAT_DATE);
                }
            }
            #endregion

            #region methods
            public MailServer copy()
            {
                return (MailServer)this.MemberwiseClone();
            }
            #endregion
        }

        public class RestrictedPages
        {
            public RestrictedPages()
            {
                //
                // TODO: Add constructor logic here
                //
            }

            #region properties
            public int Id
            {
                get;
                set;
            }
            public string Name
            {
                get;
                set;
            }
            public string Query
            {
                get;
                set;
            }
            public string PathandQuery
            {
                get;
                set;
            }
            #endregion

            #region methods
            public RestrictedPages copy()
            {
                return (RestrictedPages)this.MemberwiseClone();
            }
            #endregion
        }
    }
}
