using System;
using System.Collections.Generic;
using System.Text;

namespace dangdongcmm.model
{
    public class ListOptions
    {
        public ListOptions()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public int PageSize
        {
            get;
            set;
        }
        public int PageIndex
        {
            get;
            set;
        }
        public string SortExp
        {
            get;
            set;
        }
        public string SortDir
        {
            get;
            set;
        }

        public int Markas
        {
            get;
            set;
        }
        public string Username
        {
            get;
            set;
        }
        public bool GetAll
        {
            get;
            set;
        }
    }
}
