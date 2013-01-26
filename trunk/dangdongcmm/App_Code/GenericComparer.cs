using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Reflection;

namespace dangdongcmm
{
    public class GenericComparer<T> : IComparer<T>
    {
        private string SortExp;
        private SortDirection SortDir;

        public GenericComparer(string sortExp, SortDirection sortDir)
        {
            this.SortExp = sortExp;
            this.SortDir = sortDir;
        }
        public int Compare(T x, T y)
        {
            PropertyInfo Info = typeof(T).GetProperty(SortExp);
            IComparable obj1 = (IComparable)Info.GetValue(x, null);
            IComparable obj2 = (IComparable)Info.GetValue(y, null);

            if (SortDir == SortDirection.Ascending)
                return obj1.CompareTo(obj2);
            else return obj2.CompareTo(obj1);
        }
    }
}
