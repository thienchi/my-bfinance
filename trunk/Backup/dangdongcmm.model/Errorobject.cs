using System;
using System.Collections.Generic;
using System.Text;

namespace dangdongcmm.model
{
    public class Errorobject
    {
        public Errorobject(object control, string message, string errortype)
        {
            //
            // TODO: Add constructor logic here
            //
            Control = control;
            Message = message;
            ErrorType = errortype;
        }

        public object Control
        {
            get;
            set;
        }
        public string Message
        {
            get;
            set;
        }
        public string ErrorType
        {
            get;
            set;
        }
        public string ErrorMessage
        {
            get
            {
                return "<span class=" + ErrorType + ">" + Message + "</span>";
            }
        }
    }
}
