using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

using dangdongcmm.utilities;

namespace dangdongcmm.model
{
    /// <summary>
    /// Summary description for GeneralInfo.
    /// </summary>
    public class Info
    {
        public Info()
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
        public string Code
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string Filepreview
        {
            get;
            set;
        }
        public int Orderd
        {
            get;
            set;
        }
        public int Status
        {
            get;
            set;
        }
        public int Markas
        {
            get;
            set;
        }
        public string Iconex
        {
            get;
            set;
        }
        public DateTime Timecreate
        {
            get;
            set;
        }
        public DateTime Timeupdate
        {
            get;
            set;
        }
        public string Username
        {
            get;
            set;
        }
        public long Rownumber
        {
            get;
            set;
        }

        public string eFilepreviewurl
        {
            get
            {
                return CFunctions.Get_Filepreviewurl(Filepreview);
            }
        }
        public string eFilepreview
        {
            get
            {
                return CFunctions.Get_Filepreview(Filepreview);
            }
        }
        public string eFilepreviewthumb
        {
            get
            {
                return CConstants.FILEUPLOAD_THUMBNAIL && !CFunctions.isExternallink(Filepreview) ? CFunctions.Get_Filepreview(Filepreview.Insert(Filepreview.LastIndexOf("/") + 1, "thumb_")) : eFilepreview;
            }
        }
        public string eStatus
        {
            get
            {
                return CFunctions.Iconforstatus(Status, Id);
            }
        }
        public string eMarkas
        {
            get
            {
                return CFunctions.Iconformarkas(Markas, Id);
            }
        }
        public string eIconex
        {
            get
            {
                return CFunctions.Iconforiconex(Iconex, Id);
            }
        }
        public string eTimeupdate
        {
            get
            {
                return CFunctions.Get_Datetimetext(Timeupdate);
            }
        }
        public string eTimeupdateshort
        {
            get
            {
                return Timeupdate.Equals(new DateTime(0)) ? string.Empty : Timeupdate.ToString(CConstants.FORMAT_DATE);
            }
        }
        public string eTimeupdateago
        {
            get
            {
                return CFunctions.Get_DateAgofromToday(Timeupdate);
            }
        }
        #endregion

        #region methods
        public void set_Langindex(int langindex)
        {
            return;
        }
        #endregion
    }

    /// <summary>
    /// Summary description for AccesscounterInfo.
    /// </summary>
    public class AccesscounterInfo
    {
        public AccesscounterInfo()
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
        public int Counter
        {
            get;
            set;
        }

        public int Counter_Yesterday
        {
            get;
            set;
        }
        public int Counter_ThisDate
        {
            get;
            set;
        }
        public int Counter_ThisWeek
        {
            get;
            set;
        }
        public int Counter_ThisMonth
        {
            get;
            set;
        }
        public int Counter_Total
        {
            get;
            set;
        }
        #endregion

        #region methods
        public AccesscounterInfo copy()
        {
            return (AccesscounterInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for CatalogueInfo.
    /// </summary>
    public class CategoryInfo : Info
    {
        public CategoryInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public string Note
        {
            get;
            set;
        }
        public int Cid
        {
            get;
            set;
        }
        public int Pis
        {
            get;
            set;
        }
        public int Pid
        {
            get;
            set;
        }
        public int Depth
        {
            get;
            set;
        }
        public string Url
        {
            get;
            set;
        }
        public int Customizeid
        {
            get;
            set;
        }
        
        public int ePis
        {
            get
            {
                return Pis == 0 ? 0 : (Pis - 1);
            }
        }
        public string eName
        {
            get
            {
                return CFunctions.install_urlname(Name);
            }
        }
        public string eUrl
        {
            get
            {
                return this.geteUrl();// ((Cid == Webcmm.Id.News ? "n" : "")) + Id.ToString() + "d0d=" + eName;
            }
        }
        #endregion
        public string eUrl2
        {
            get
            {
                return CFunctions.install_urlname(Name)+".aspx";// ((Cid == Webcmm.Id.News ? "n" : "")) + Id.ToString() + "d0d=" + eName;
            }
        }
        public int dictCount
        {
            get; set;
        }
        public string dictCountStr
        {
            get
            {
                return dictCount.ToString();
            }

        }
        public string eUrlDictionary
        {
            get
            {
                return "tu-dien-thuat-ngu-vn/" + CFunctions.install_urlname(Name);
            }
        }
        #region methods
        public CategoryInfo copy()
        {
            return (CategoryInfo)this.MemberwiseClone();
        }
        private string geteUrl()
        {
            string vlreturn = "";
            switch (Cid)
            {
                case Webcmm.Id.News:
                    vlreturn = "n";
                    break;
                case Webcmm.Id.Product:
                    vlreturn = "p";
                    break;
                case Webcmm.Id.Libraries:
                    vlreturn = "l";
                    break;
            }
            vlreturn += Id.ToString() + "d0d=" + eName;
            return vlreturn;
        }
        #endregion
    }

    /// <summary>
    /// Summary description for CataloguetypeofInfo.
    /// </summary>
    public class CategorytypeofInfo : Info
    {
        public CategorytypeofInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public string Note
        {
            get;
            set;
        }
        #endregion

        #region methods
        public CategorytypeofInfo copy()
        {
            return (CategorytypeofInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for CatalogueInfo.
    /// </summary>
    public class CategoryattrInfo : Info
    {
        public CategoryattrInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public string Note
        {
            get;
            set;
        }
        public int Cid
        {
            get;
            set;
        }
        public int Pis
        {
            get;
            set;
        }
        public int Pid
        {
            get;
            set;
        }
        public int Depth
        {
            get;
            set;
        }
        public string Url
        {
            get;
            set;
        }

        public int ePis
        {
            get
            {
                return Pis == 0 ? 0 : (Pis - 1);
            }
        }
        public string eName
        {
            get
            {
                return CFunctions.install_urlname(Name);
            }
        }
        public string eUrl
        {
            get
            {
                return this.geteUrl();// ((Cid == Webcmm.Id.News ? "n" : "")) + Id.ToString() + "d0d=" + eName;
            }
        }
        #endregion

        #region methods
        public CategoryattrInfo copy()
        {
            return (CategoryattrInfo)this.MemberwiseClone();
        }
        private string geteUrl()
        {
            string vlreturn = "";
            switch (Cid)
            {
                case Webcmm.Id.News:
                    vlreturn = "n";
                    break;
                case Webcmm.Id.Product:
                    vlreturn = "p";
                    break;
                case Webcmm.Id.Libraries:
                    vlreturn = "l";
                    break;
            }
            vlreturn += Id.ToString() + "d0d=" + eName;
            return vlreturn;
        }
        #endregion
    }


    /// <summary>
    /// Summary description for CurrencyInfo.
    /// </summary>
    public class CartInfo : Info
    {
        public CartInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public int Memberid
        {
            get;
            set;
        }
        public int Paymenttype
        {
            get;
            set;
        }
        public double Amount
        {
            get;
            set;
        }
        public double Shippingfee
        {
            get;
            set;
        }
        public double Discountfee
        {
            get;
            set;
        }
        public double Totalfee
        {
            get
            {
                return Amount + Shippingfee - Discountfee;
            }
        }
        public string Note
        {
            get;
            set;
        }
        public DateTime Timecomplete
        {
            get;
            set;
        }
        public string Lang
        {
            get;
            set;
        }
        public int Checkout
        {
            get;
            set;
        }

        public string Shipping_Name
        {
            get;
            set;
        }
        public string Shipping_Address
        {
            get;
            set;
        }
        public string Shipping_Address2
        {
            get;
            set;
        }
        public string Shipping_City
        {
            get;
            set;
        }
        public string Shipping_District
        {
            get;
            set;
        }
        public int Shipping_Nationalid
        {
            get;
            set;
        }
        public string Shipping_Zipcode
        {
            get;
            set;
        }
        public string Shipping_Phone
        {
            get;
            set;
        }
        public string Shipping_Email
        {
            get;
            set;
        }

        public string Billing_Name
        {
            get;
            set;
        }
        public string Billing_Address
        {
            get;
            set;
        }
        public string Billing_Address2
        {
            get;
            set;
        }
        public string Billing_City
        {
            get;
            set;
        }
        public string Billing_District
        {
            get;
            set;
        }
        public int Billing_Nationalid
        {
            get;
            set;
        }
        public string Billing_Zipcode
        {
            get;
            set;
        }
        public string Billing_Phone
        {
            get;
            set;
        }
        public string Billing_Email
        {
            get;
            set;
        }

        public List<CartitemInfo> lCartitem
        {
            get;
            set;
        }
        public MemberInfo iMember
        {
            get;
            set;
        }
        public CategoryInfo iPayment
        {
            get;
            set;
        }

        public string Couponcode
        {
            get;
            set;
        }
        public string eAmount
        {
            get
            {
                return CFunctions.Set_Currency(Amount);
            }
        }
        public string eShippingfee
        {
            get
            {
                return CFunctions.Set_Currency(Shippingfee);
            }
        }
        public string eDiscountfee
        {
            get
            {
                return CFunctions.Set_Currency(Discountfee);
            }
        }
        public string eTotalfee
        {
            get
            {
                return CFunctions.Set_Currency(Totalfee);
            }
        }
        public string Membername
        {
            get;
            set;
        }
        public string Paymentname
        {
            get;
            set;
        }
        public string Shipping_Nationalname
        {
            get;
            set;
        }
        public string Billing_Nationalname
        {
            get;
            set;
        }
        #endregion

        #region methods
        public CartInfo copy()
        {
            return (CartInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for CurrencyInfo.
    /// </summary>
    public class CartitemInfo : Info
    {
        public CartitemInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public int CartID
        {
            get;
            set;
        }
        public int ProductID
        {
            get;
            set;
        }
        public string Productname
        {
            get;
            set;
        }
        public int Quantity
        {
            get;
            set;
        }
        public double Amount
        {
            get;
            set;
        }
        public long TemporaryID
        {
            get;
            set;
        }

        public ProductInfo iProduct
        {
            get;
            set;
        }
        public string eAmount
        {
            get
            {
                return CFunctions.Set_Currency(Amount);
            }
        }
        #endregion

        #region methods
        public CartitemInfo copy()
        {
            return (CartitemInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for CommentInfo.
    /// </summary>
    public class PaymentgateInfo : Info
    {
        public PaymentgateInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public string Description
        {
            get;
            set;
        }

        #endregion

        #region methods
        public PaymentgateInfo copy()
        {
            return (PaymentgateInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for CommentInfo.
    /// </summary>
    public class CommentInfo : Info
    {
        public CommentInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public string Introduce
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public int Viewcounter
        {
            get;
            set;
        }
        public int Rating
        {
            get;
            set;
        }
        public int Iid
        {
            get;
            set;
        }
        public int Belongto
        {
            get;
            set;
        }
        public string Sender_Name
        {
            get;
            set;
        }
        public string Sender_Address
        {
            get;
            set;
        }
        public string Sender_Email
        {
            get;
            set;
        }
        public string Sender_Phone
        {
            get;
            set;
        }
        
        #endregion

        #region methods
        public CommentInfo copy()
        {
            return (CommentInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for MeLocationInfo.
    /// </summary>
    public class MeCreditcardInfo
    {
        public MeCreditcardInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private string fullname;

        #region properties
        public int Id
        {
            get;
            set;
        }
        public string Firstname
        {
            get;
            set;
        }
        public string Lastname
        {
            get;
            set;
        }
        public string Cardnumber
        {
            get;
            set;
        }
        public string Cardtype
        {
            get;
            set;
        }
        public int Expirationmonth
        {
            get;
            set;
        }
        public int Expirationyear
        {
            get;
            set;
        }
        public string Address
        {
            get;
            set;
        }
        public string City
        {
            get;
            set;
        }
        public string State
        {
            get;
            set;
        }
        public string Zipcode
        {
            get;
            set;
        }
        public int Memberid
        {
            get;
            set;
        }
        public int Status
        {
            get;
            set;
        }
        public double Balance
        {
            get;
            set;
        }
        public long Rownumber
        {
            get;
            set;
        }

        public string Fullname
        {
            get
            {
                return Firstname + " " + Lastname;
            }
            set
            {
                fullname = value;
            }
        }
        public string eBalance
        {
            get
            {
                return CFunctions.Set_Currency(Balance);
            }
        }
        #endregion

        #region methods
        public MeCreditcardInfo copy()
        {
            return (MeCreditcardInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for CommentInfo.
    /// </summary>
    public class MeCreditcardHistoryInfo
    {
        public MeCreditcardHistoryInfo()
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
        public int Memberid
        {
            get;
            set;
        }
        public int Creditcardid
        {
            get;
            set;
        }
        public double Balance
        {
            get;
            set;
        }
        public double Amount
        {
            get;
            set;
        }
        public int Addorsub
        {
            get;
            set;
        }
        public string Note
        {
            get;
            set;
        }
        public int Status
        {
            get;
            set;
        }
        public DateTime Timeupdate
        {
            get;
            set;
        }
        public long Rownumber
        {
            get;
            set;
        }

        public string eTimeupdate
        {
            get
            {
                return CFunctions.Get_Datetimetext(Timeupdate);
            }
        }
        public string eTimeupdateago
        {
            get
            {
                return CFunctions.Get_DateAgofromToday(Timeupdate);
            }
        }
        public string eBalance
        {
            get
            {
                return CFunctions.Set_Currency(Balance);
            }
        }
        public string eAmount
        {
            get
            {
                return CFunctions.Set_Currency(Amount);
            }
        }

        public MemberInfo iMember
        {
            get;
            set;
        }
        #endregion

        #region methods

        public MeCreditcardHistoryInfo copy()
        {
            return (MeCreditcardHistoryInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for CommentInfo.
    /// </summary>
    public class TransactionInfo
    {
        public TransactionInfo()
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
        public string Code
        {
            get;
            set;
        }
        public int Memberid
        {
            get;
            set;
        }
        public double Balance
        {
            get;
            set;
        }
        public double Amount
        {
            get;
            set;
        }
        public int Addorsub
        {
            get;
            set;
        }
        public string Note
        {
            get;
            set;
        }
        public string Bankname
        {
            get;
            set;
        }
        public string Accountname
        {
            get;
            set;
        }
        public string Accountnumber
        {
            get;
            set;
        }
        public DateTime Timeupdate
        {
            get;
            set;
        }
        public long Rownumber
        {
            get;
            set;
        }

        public string eTimeupdate
        {
            get
            {
                return CFunctions.Get_Datetimetext(Timeupdate);
            }
        }
        public string eBalance
        {
            get
            {
                return CFunctions.Set_Currency(Balance);
            }
        }
        public string eAmount
        {
            get
            {
                return CFunctions.Set_Currency(Amount);
            }
        }
        
        public MemberInfo iMember
        {
            get;
            set;
        }
        #endregion

        #region methods

        public TransactionInfo copy()
        {
            return (TransactionInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for FeedbackInfo.
    /// </summary>
    public class FeedbackInfo
    {
        public FeedbackInfo()
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
        public string Sender_Name
        {
            get;
            set;
        }
        public string Sender_Address
        {
            get;
            set;
        }
        public string Sender_Email
        {
            get;
            set;
        }
        public string Sender_Phone
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public int Status
        {
            get;
            set;
        }
        public int Markas
        {
            get;
            set;
        }
        public int Pis
        {
            get;
            set;
        }
        public int Pid
        {
            get;
            set;
        }
        public int Depth
        {
            get;
            set;
        }
        public DateTime Timeupdate
        {
            get;
            set;
        }
        public int Viewcounter
        {
            get;
            set;
        }
        public long Rownumber
        {
            get;
            set;
        }

        public string eTimeupdate
        {
            get
            {
                return CFunctions.Get_Datetimetext(Timeupdate);
            }
        }
        public string eTimeupdateshort
        {
            get
            {
                return Timeupdate.Equals(new DateTime(0)) ? string.Empty : Timeupdate.ToString(CConstants.FORMAT_DATE);
            }
        }
        public int ePis
        {
            get
            {
                return Pis == 0 ? 0 : (Pis - 1);
            }
        }
        #endregion

        #region methods
        public FeedbackInfo copy()
        {
            return (FeedbackInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for FileattachInfo.
    /// </summary>
    public class FileattachInfo
    {
        public FileattachInfo()
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
        public string Path
        {
            get;
            set;
        }
        public string Note
        {
            get;
            set;
        }
        public string Subject
        {
            get;
            set;
        }
        public string Url
        {
            get;
            set;
        }
        public long Sized
        {
            get;
            set;
        }
        public int Typed
        {
            get;
            set;
        }
        public int Orderd
        {
            get;
            set;
        }
        public int Status
        {
            get;
            set;
        }
        public int Markas
        {
            get;
            set;
        }
        public string Iconex
        {
            get;
            set;
        }
        public DateTime Timeupdate
        {
            get;
            set;
        }
        public string Username
        {
            get;
            set;
        }
        public int Belongto
        {
            get;
            set;
        }
        public int Iid
        {
            get;
            set;
        }
        public long Rownumber
        {
            get;
            set;
        }

        public string eStatus
        {
            get
            {
                return CFunctions.Iconforstatus(Status, Id);
            }
        }
        public string eMarkas
        {
            get
            {
                return CFunctions.Iconformarkas(Markas, Id);
            }
        }
        public string eIconex
        {
            get
            {
                return CFunctions.Iconforiconex(Iconex, Id);
            }
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
        public string eFileattach
        {
            get
            {
                return CFunctions.Get_Filepreview(Path);
            }
        }
        public string eFileattachcmm
        {
            get
            {
                return CFunctions.Get_Filepreview_cmm(Path);
            }
        }
        public string ePath
        {
            get
            {
                return CFunctions.MBEncrypt(Path);
            }
        }
        public string eSized
        {
            get
            {
                return Sized > 1048576 ? (Sized / 1048576 + " MB") : (Sized > 1024 ? (Math.Round(decimal.Divide(Sized, 1024), 2) + " KB") : Sized + " bytes");
            }
        }

        public string eFileattachthumb
        {
            get
            {
                return CFunctions.Get_Filepreview(ePaththumb);
            }
        }
        public string ePaththumb
        {
            get
            {
                return CConstants.FILEUPLOAD_THUMBNAIL && !CFunctions.isExternallink(Path) ? Path.Insert(Path.LastIndexOf("/") + 1, "thumb_") : Path;
            }
        }
        #endregion

        #region methods
        public FileattachInfo copy()
        {
            return (FileattachInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for GeneralInfo.
    /// </summary>
    public class GeneralInfo : Info
    {
        public GeneralInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public string Introduce
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public int Cid
        {
            get;
            set;
        }
        public string Cname
        {
            get;
            set;
        }
        public int Pis
        {
            get;
            set;
        }
        public int Pid
        {
            get;
            set;
        }
        public int Depth
        {
            get;
            set;
        }
        public int Viewcounter
        {
            get;
            set;
        }
        public int Belongto
        {
            get;
            set;
        }
        public string Belongtoname
        {
            get;
            set;
        }
        public int isDictionary
        {
            get;
            set;
        }
        public int ePis
        {
            get
            {
                return Pis == 0 ? 0 : (Pis - 1);
            }
        }
        public string eName
        {
            get
            {
                return CFunctions.install_urlname(Name);
            }
        }
        public string eUrl
        {
            get
            {
                string vlreturn = string.Empty;
                switch (Belongto)
                {
                    case Webcmm.Id.News:
                        vlreturn = "n";
                        break;
                    case Webcmm.Id.Product:
                        vlreturn = "p";
                        break;
                    case Webcmm.Id.Libraries:
                        vlreturn = "l";
                        break;
                }
                vlreturn += Cid.ToString() + "d" + Id.ToString() + "d=" + eName;
                return vlreturn;
            }
        }
        public string eUrl2
        {
            get
            {
                string cateLink = CFunctions.install_urlname(Cname).Replace(".aspx","");
                if (isDictionary == 1)
                {
                    return "tu-dien-thuat-ngu-vn/" + cateLink + "/" + eName;
                }
                return cateLink + "/" + eName ;
            }
        }

        #region methods
        public GeneralInfo copy()
        {
            return (GeneralInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for LibrariesInfo.
    /// </summary>
    public class LibrariesInfo : Info
    {
        public LibrariesInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public string Introduce
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public int Cid
        {
            get;
            set;
        }
        public string Cname
        {
            get;
            set;
        }
        public int Allowcomment
        {
            get;
            set;
        }
        public int Album
        {
            get;
            set;
        }
        public int Viewcounter
        {
            get;
            set;
        }
        public string Relateditem
        {
            get;
            set;
        }

        public List<FileattachInfo> lFileattach
        {
            get;
            set;
        }

        public string eName
        {
            get
            {
                return CFunctions.install_urlname(Name);
            }
        }
        public string eUrl
        {
            get
            {
                return "l" + Cid.ToString() + "d" + Id.ToString() + "d=" + eName;
            }
        }

        public int Rating
        {
            get;
            set;
        }
        #endregion

        #region methods
        public LibrariesInfo copy()
        {
            return (LibrariesInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for MenuInfo.
    /// </summary>
    public class MenuInfo : Info
    {
        public MenuInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public string Note
        {
            get;
            set;
        }
        public string Navigateurl
        {
            get;
            set;
        }
        public string Tooltip
        {
            get;
            set;
        }
        public string Attributes
        {
            get;
            set;
        }
        public int ApplyAttributesChild
        {
            get;
            set;
        }
        public int Visible
        {
            get;
            set;
        }
        public int Cid
        {
            get;
            set;
        }
        public int Pis
        {
            get;
            set;
        }
        public int Pid
        {
            get;
            set;
        }
        public int Depth
        {
            get;
            set;
        }
        public int Cataloguetypeofid
        {
            get;
            set;
        }
        public int Catalogueid
        {
            get;
            set;
        }
        public int Insertcatalogue
        {
            get;
            set;
        }
        public int Noroot
        {
            get;
            set;
        }
        
        public int ePis
        {
            get
            {
                return Pis == 0 ? 0 : (Pis - 1);
            }
        }
        #endregion

        #region methods
        public MenuInfo copy()
        {
            return (MenuInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for MenutypeofInfo.
    /// </summary>
    public class MenutypeofInfo
    {
        public MenutypeofInfo()
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
        public string Code
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string Path
        {
            get;
            set;
        }
        public string Note
        {
            get;
            set;
        }
        public int Insertbreak
        {
            get;
            set;
        }
        public int Status
        {
            get;
            set;
        }
        public DateTime Timeupdate
        {
            get;
            set;
        }
        public string Username
        {
            get;
            set;
        }

        public string eStatus
        {
            get
            {
                return CFunctions.Iconforstatus(Status, Id);
            }
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
        public MenutypeofInfo copy()
        {
            return (MenutypeofInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for MemberInfo.
    /// </summary>
    public class MemberInfo
    {
        public MemberInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private string fullname;

        #region properties
        public int Id
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
        public string Fullname
        {
            get
            {
                return Firstname + " " + Lastname;
            }
            set
            {
                fullname = value;
            }
        }
        public string Nickname
        {
            get
            {
                return Firstname + " " + (CFunctions.IsNullOrEmpty(Lastname) ? "" : Lastname.Substring(0, 1)) + ".";
            }
        }
        public string Firstname
        {
            get;
            set;
        }
        public string Lastname
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        public string PIN
        {
            get;
            set;
        }
        public int Status
        {
            get;
            set;
        }
        public int Markas
        {
            get;
            set;
        }
        public DateTime Timecreate
        {
            get;
            set;
        }
        public DateTime Timeupdate
        {
            get;
            set;
        }
        public int Logincache
        {
            get;
            set;
        }
        public int Loginfirst
        {
            get;
            set;
        }
        public int Autosave
        {
            get;
            set;
        }
        public string Temporarycode
        {
            get;
            set;
        }
        public int Grouptype
        {
            get;
            set;
        }
        public string Filepreview
        {
            get;
            set;
        }
        public long Rownumber
        {
            get;
            set;
        }

        public int Ranking
        {
            get;
            set;
        }
        public int Vote
        {
            get;
            set;
        }
        public int Ratingweight
        {
            get;
            set;
        }

        public MeProfileInfo iProfile
        {
            get;
            set;
        }
        
        public string eFilepreview
        {
            get
            {
                return CFunctions.Get_Filepreview(Filepreview);
            }
        }
        public string eStatus
        {
            get
            {
                return CFunctions.Iconforstatus(Status, Id);
            }
        }
        public string eMarkas
        {
            get
            {
                return CFunctions.Iconformarkas(Markas, Id);
            }
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

        public string eLinkname
        {
            get
            {
                return "<b><a href=\"" + CConstants.WEBSITE + "/en/" + (Grouptype == 0 ? "taskclient" : "taskrunner") + ".aspx?iid=" + Id + "\">" + Nickname + "</a></b>";
            }
        }
        public string eLinkimage
        {
            get
            {
                return "<div class=\"bimg\"><a href=\"" + CConstants.WEBSITE + "/en/" + (Grouptype == 0 ? "taskclient" : "taskrunner") + ".aspx?iid=" + Id + "\">" + (CFunctions.IsNullOrEmpty(Filepreview) ? "<img src=\"../images/avatar-def.gif\" alt=\"No avatar\" />" : eFilepreview) + "</a>"
                    + (Ranking == 0 || Grouptype == 0 ? "" : "<div class=\"rank\">" + Ranking + "</div>")
                    + "</div>";
            }
        }
        public string eVote
        {
            get
            {
                return Vote == 0 ? "" : "<div class='vote weight" + Ratingweight + "'>(" + Vote + " ratings)</div>";
            }
        }
        #endregion

        #region methods
        public MemberInfo copy()
        {
            return (MemberInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for MemberInfo.
    /// </summary>
    public class MeProfileInfo
    {
        public MeProfileInfo()
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
        public long Rownumber
        {
            get;
            set;
        }
        public string Address
        {
            get;
            set;
        }
        public string Zipcode
        {
            get;
            set;
        }
        public string State
        {
            get;
            set;
        }
        public int Nationalid
        {
            get;
            set;
        }
        public string Nationalname
        {
            get;
            set;
        }
        public int Cityid
        {
            get;
            set;
        }
        public string Cityname
        {
            get;
            set;
        }
        public int Districtid
        {
            get;
            set;
        }
        public string Districtname
        {
            get;
            set;
        }
        public string Phone
        {
            get;
            set;
        }
        
        public string About
        {
            get;
            set;
        }
        public string Blog
        {
            get;
            set;
        }
        public string Homepage
        {
            get;
            set;
        }
        public string Facebook
        {
            get;
            set;
        }
        public string Twitter
        {
            get;
            set;
        }
        public string Youtube
        {
            get;
            set;
        }
        public string Flickr
        {
            get;
            set;
        }
        public string Skype
        {
            get;
            set;
        }
        public string Yahoo
        {
            get;
            set;
        }
        public DateTime Birthday
        {
            get;
            set;
        }
        public string Profession
        {
            get;
            set;
        }

        public string Blogtext
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Blog) ? "" : Blog.Remove(0, 1);
            }
        }
        public string Homepagetext
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Homepage) ? "" : Homepage.Remove(0, 1);
            }
        }
        public string Facebooktext
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Facebook) ? "" : Facebook.Remove(0, 1);
            }
        }
        public string Twittertext
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Twitter) ? "" : Twitter.Remove(0, 1);
            }
        }
        public string Youtubetext
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Youtube) ? "" : Youtube.Remove(0, 1);
            }
        }
        public string Flickrtext
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Flickr) ? "" : Flickr.Remove(0, 1);
            }
        }
        public string Skypetext
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Skype) ? "" : Skype.Remove(0, 1);
            }
        }
        public string Yahootext
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Yahoo) ? "" : Yahoo.Remove(0, 1);
            }
        }
        public string Blogsh
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Blog) ? "0" : Blog.Substring(0, 1);
            }
        }
        public string Homepagesh
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Homepage) ? "0" : Homepage.Substring(0, 1);
            }
        }
        public string Facebooksh
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Facebook) ? "0" : Facebook.Substring(0, 1);
            }
        }
        public string Twittersh
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Twitter) ? "0" : Twitter.Substring(0, 1);
            }
        }
        public string Youtubesh
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Youtube) ? "0" : Youtube.Substring(0, 1);
            }
        }
        public string Flickrsh
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Flickr) ? "0" : Flickr.Substring(0, 1);
            }
        }
        public string Skypesh
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Skype) ? "0" : Skype.Substring(0, 1);
            }
        }
        public string Yahoosh
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Yahoo) ? "0" : Yahoo.Substring(0, 1);
            }
        }

        public string eAbout
        {
            get
            {
                return About.Length < 155 ? About : About.Substring(0, 155) + "...";
            }
        }
        #endregion

        #region methods
        public MeProfileInfo copy()
        {
            return (MeProfileInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for NewsInfo.
    /// </summary>
    public class NewsInfo : Info
    {
        public NewsInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private string categoryid;
        private string categoryattrid;

        #region properties
        public string Introduce
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public string Tag
        {
            get;
            set;
        }
        public int Cid
        {
            get;
            set;
        }
        public string Cname
        {
            get;
            set;
        }
        public int Allowcomment
        {
            get;
            set;
        }
        public string Author
        {
            get;
            set;
        }
        public DateTime Timeexpire
        {
            get;
            set;
        }
        public int Viewcounter
        {
            get;
            set;
        }
        public string Url
        {
            get;
            set;
        }
        public string Relateditem
        {
            get;
            set;
        }

        public List<CategoryInfo> lCategory
        {
            get;
            set;
        }
        public List<CategoryattrInfo> lCategoryattr
        {
            get;
            set;
        }
        public string CNameLink
        {
            get
            {
                //<a href="/<%#Eval("CNameLink")%>"><%#Eval("CName")%></a>
                string kq = "<a href=\"/";
                kq += CFunctions.install_urlname(Cname);
                kq += "\">";
                kq += Cname;
                kq += "</a>";
                foreach (var item in lCategory)
                {
                    if (!item.Name.Equals(Cname))
                    {
                        kq += "<b>&nbsp;>>&nbsp;</b><a href=\"/";
                        kq += CFunctions.install_urlname(item.Name);
                        kq += "\">";
                        kq += item.Name;
                        kq += "</a>";
                    }                    
                }
                return kq;
            }
            set
            {
                CNameLink = value;
            }
        }
        public string UrlQuoute
        {
            get
            {
                //string cateLink = CFunctions.install_urlname(Cname).Replace(".aspx", "");
                string link = Url;
                //link = CFunctions.Convert_Chuoi_Khong_Dau(link);
                if(link != null && !link.Trim().Equals(""))
                {
                    int i = link.IndexOf("=");
                    link = link.Substring(i + 1);
                    link = link.Replace("http://bfinance.vn/", "");
                }
                link = link.Replace("“", "");
                link = link.Replace("”", "").ToLower();
                link = link.Replace('”', ' ');
                link = link.Replace('“', ' ');
                link = link.Replace("%e2%80%9c", "");
                link = link.Replace("%e2%80%a6", "");
                link = link.Replace("%e2%80%9d", "");
                
                link = link.Trim();
                return link;
            }
            set
            {
                UrlQuoute = value;
            }
        }
        public string NameTrimmed
        {
            get
            {
                string kq = Name;
                if (kq != null && kq.Length > 200)
                {
                    kq = kq.Substring(0, 200) + " ...";
                }
                return kq;
            }
            set
            {
                NameTrimmed = value;
            }
        }
        public string Categoryid
        {
            get
            {
                if (!CFunctions.IsNullOrEmpty(categoryid)) return categoryid;
                
                string vlreturn = string.Empty;
                if (lCategory != null && lCategory.Count > 0)
                {
                    foreach (CategoryInfo catinfo in lCategory)
                    {
                        vlreturn += catinfo.Id + ",";
                    }
                    if (vlreturn != string.Empty)
                        vlreturn = vlreturn.Remove(vlreturn.LastIndexOf(','));
                }
                return vlreturn;
            }
            set
            {
                categoryid = value;
            }
        }
        public string Categoryattrid
        {
            get
            {
                if (!CFunctions.IsNullOrEmpty(categoryattrid)) return categoryattrid;

                string vlreturn = string.Empty;
                if (lCategoryattr != null && lCategoryattr.Count > 0)
                {
                    foreach (CategoryattrInfo catinfo in lCategoryattr)
                    {
                        vlreturn += catinfo.Id + ",";
                    }
                    if (vlreturn != string.Empty)
                        vlreturn = vlreturn.Remove(vlreturn.LastIndexOf(','));
                }
                return vlreturn;
            }
            set
            {
                categoryattrid = value;
            }
        }
        public string eCategory
        {
            get
            {
                string vlreturn = string.Empty;
                if (lCategory != null && lCategory.Count > 0)
                {
                    foreach (CategoryInfo catinfo in lCategory)
                    {
                        if (catinfo.Pid != 0)
                            vlreturn += "<a href=\"n" + catinfo.Id + "d0d=" + CFunctions.install_urlname(catinfo.Name) + "\">" + catinfo.Name + "</a>, ";
                    }
                    if (vlreturn != string.Empty)
                        vlreturn = vlreturn.Remove(vlreturn.LastIndexOf(','));
                }
                return vlreturn;
            }
        }
        
        public string eTimeexpire
        {
            get
            {
                return Timeexpire.Equals(new DateTime(0)) ? string.Empty : Timeexpire.ToString(CConstants.FORMAT_DATE);
            }
        }
        public string eName
        {
            get
            {
                return CFunctions.install_urlname(Name);
            }
        }
        public string eUrl2
        {
            get
            {
                //return "n" + Cid.ToString() + "d" + Id.ToString() + "d=" + eName;
                string cateLink = CFunctions.install_urlname(Cname).Replace(".aspx","");
                /*if (lCategory != null && lCategory.Count > 0)
                {
                    cateLink = lCategory[0].eName.Replace(".aspx", "");
                }*/
                return cateLink + "/" + eName;

            }
        }
        public string eUrl
        {
            get
            {
                return "n" + Cid.ToString() + "d" + Id.ToString() + "d=" + eName;
            }
        }
        public string eTag
        {
            get
            {
                string vlreturn = string.Empty;
                if (!CFunctions.IsNullOrEmpty(Tag))
                {
                    string[] tags = Tag.Split(',');
                    if (tags != null && tags.Length > 0)
                    {
                        foreach (string key in tags)
                        {
                            if (!CFunctions.IsNullOrEmpty(key))
                            {
                                string keya = "<a href=\"/search.aspx?searchin=" + Webcmm.Id.News + "&cid=&keywords=" + System.Uri.EscapeUriString("\"" + key.Trim() + "\"") + "\">" + key + "</a>";
                                vlreturn += keya + ", ";
                            }
                        }
                    }
                    if (vlreturn != string.Empty)
                        vlreturn = vlreturn.Remove(vlreturn.LastIndexOf(','));
                }
                return vlreturn;
            }
        }
        public string eTag2
        {
            get
            {
                string vlreturn = string.Empty;
                if (!CFunctions.IsNullOrEmpty(Tag))
                {
                    string[] tags = Tag.Split(',');
                    if (tags != null && tags.Length > 0)
                    {
                        foreach (string key in tags)
                        {
                            if (!CFunctions.IsNullOrEmpty(key))
                            {
                                string keya = "<a href=\"/tim-kiem-" + Webcmm.Id.News + "-" + System.Uri.EscapeUriString("" + key.Trim() + "") + ".aspx\">" + key + "</a>";
                                vlreturn += keya + ", ";
                            }
                        }
                    }
                    if (vlreturn != string.Empty)
                        vlreturn = vlreturn.Remove(vlreturn.LastIndexOf(','));
                }
                return vlreturn;
            }
        }
        public int Rating
        {
            get;
            set;
        }
        #endregion

        #region methods
        public NewsInfo copy()
        {
            return (NewsInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for RSSResourceInfo.
    /// </summary>
    public class RSSResourceInfo : Info
    {
        public RSSResourceInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public string WebsiteUrl
        {
            get;
            set;
        }
        public string RSSUrl
        {
            get;
            set;
        }
        public int Cid
        {
            get;
            set;
        }
        public string Cname
        {
            get;
            set;
        }
        public DateTime Timelastestget
        {
            get;
            set;
        }
        public string Nodecontent
        {
            get;
            set;
        }
        public string Nodetitle
        {
            get;
            set;
        }
        public string Nodeintroduce
        {
            get;
            set;
        }
        
        public string eTimelastestget
        {
            get
            {
                return CFunctions.Get_Datetimetext(Timelastestget);
            }
        }

        #endregion

        #region methods
        public RSSResourceInfo copy()
        {
            return (RSSResourceInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for ProductInfo.
    /// </summary>
    public class ProductInfo : Info
    {
        public ProductInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private string categoryid;
        private string categoryattrid;

        #region properties
        public string Introduce
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public string Features
        {
            get;
            set;
        }
        public int Cid
        {
            get;
            set;
        }
        public string Cname
        {
            get;
            set;
        }
        public int Viewcounter
        {
            get;
            set;
        }
        public string Provider
        {
            get;
            set;
        }
        public string Advertise
        {
            get;
            set;
        }
        public double Price
        {
            get;
            set;
        }
        public double Pricealter
        {
            get;
            set;
        }
        public int Allowcomment
        {
            get;
            set;
        }
        public int Album
        {
            get;
            set;
        }
        public string Relateditem
        {
            get;
            set;
        }
        public string Relatednews
        {
            get;
            set;
        }
        public int Hascustomize
        {
            get;
            set;
        }

        public List<FileattachInfo> lFileattach
        {
            get;
            set;
        }
        public List<CategoryInfo> lCategory
        {
            get;
            set;
        }
        public List<CategoryattrInfo> lCategoryattr
        {
            get;
            set;
        }
        public string Categoryid
        {
            get
            {
                if (!CFunctions.IsNullOrEmpty(categoryid)) return categoryid;
                
                string vlreturn = string.Empty;
                if (lCategory != null && lCategory.Count > 0)
                {
                    foreach (CategoryInfo catinfo in lCategory)
                    {
                        vlreturn += catinfo.Id + ",";
                    }
                    if (vlreturn != string.Empty)
                        vlreturn = vlreturn.Remove(vlreturn.LastIndexOf(','));
                }
                return vlreturn;
            }
            set
            {
                categoryid = value;
            }
        }
        public string Categoryattrid
        {
            get
            {
                if (!CFunctions.IsNullOrEmpty(categoryattrid)) return categoryattrid;
                
                string vlreturn = string.Empty;
                if (lCategoryattr != null && lCategoryattr.Count > 0)
                {
                    foreach (CategoryattrInfo catinfo in lCategoryattr)
                    {
                        vlreturn += catinfo.Id + ",";
                    }
                    if (vlreturn != string.Empty)
                        vlreturn = vlreturn.Remove(vlreturn.LastIndexOf(','));
                }
                return vlreturn;
            }
            set
            {
                categoryattrid = value;
            }
        }
        public string eCategory
        {
            get
            {
                string vlreturn = string.Empty;
                if (lCategory != null && lCategory.Count > 0)
                {
                    foreach (CategoryInfo catinfo in lCategory)
                    {
                        if (catinfo.Pid != 0)
                            vlreturn += catinfo.Name + ", ";
                    }
                    if (vlreturn != string.Empty)
                        vlreturn = vlreturn.Remove(vlreturn.LastIndexOf(','));
                }
                return vlreturn;
            }
        }

        public string ePrice
        {
            get
            {
                return CFunctions.Set_Currency(Price);
            }
        }
        public string ePricealter
        {
            get
            {
                return Pricealter == 0 ? "" : CFunctions.Set_Currency(Pricealter);
            }
        }
        public string eName
        {
            get
            {
                return CFunctions.install_urlname(Name);
            }
        }
        public string eUrl
        {
            get
            {
                return "p" + Cid.ToString() + "d" + Id.ToString() + "d=" + eName;
            }
        }

        public int Rating
        {
            get;
            set;
        }
        #endregion

        #region methods
        public ProductInfo copy()
        {
            return (ProductInfo)this.MemberwiseClone();
        }

        #endregion
    }

    /// <summary>
    /// Summary description for ProductcategoryInfo.
    /// </summary>
    public class ItemcategoryInfo
    {
        public ItemcategoryInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public int Iid
        {
            get;
            set;
        }
        public int Categoryid
        {
            get;
            set;
        }
        public int Belongto
        {
            get;
            set;
        }
        #endregion

        #region methods
        public ItemcategoryInfo copy()
        {
            return (ItemcategoryInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for ProductcategoryattrInfo.
    /// </summary>
    public class ItemcategoryattrInfo
    {
        public ItemcategoryattrInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public int Iid
        {
            get;
            set;
        }
        public int Categoryid
        {
            get;
            set;
        }
        public int Belongto
        {
            get;
            set;
        }
        #endregion

        #region methods
        public ItemcategoryattrInfo copy()
        {
            return (ItemcategoryattrInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for StaticcontentInfo.
    /// </summary>
    public class StaticcontentInfo
    {
        public StaticcontentInfo()
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
        public string Filepath
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string Code
        {
            get;
            set;
        }
        public int Separatepage
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public int Status
        {
            get;
            set;
        }
        public DateTime Timeupdate
        {
            get;
            set;
        }
        public string Username
        {
            get;
            set;
        }
        public long Rownumber
        {
            get;
            set;
        }

        public string eStatus
        {
            get
            {
                return CFunctions.Iconforstatus(Status, Id);
            }
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
        public StaticcontentInfo copy()
        {
            return (StaticcontentInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for SymbolInfo.
    /// </summary>
    public class SymbolInfo
    {
        public SymbolInfo()
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
        public string Path
        {
            get;
            set;
        }
        public string Note
        {
            get;
            set;
        }
        public int Sized
        {
            get;
            set;
        }
        public int Status
        {
            get;
            set;
        }
        public int Orderd
        {
            get;
            set;
        }
        public DateTime Timeupdate
        {
            get;
            set;
        }
        public string Username
        {
            get;
            set;
        }
        public long Rownumber
        {
            get;
            set;
        }

        public string eFilepreview
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Path) ? Name : "<img src='../" + Path + "' />";
            }
        }
        public string eFilepreviewcmm
        {
            get
            {
                return CFunctions.IsNullOrEmpty(Path) ? Name : "<img src='../../" + Path + "' />";
            }
        }
        public string eStatus
        {
            get
            {
                return CFunctions.Iconforstatus(Status, Id);
            }
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
        public SymbolInfo copy()
        {
            return (SymbolInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for UserInfo.
    /// </summary>
    public class UserInfo
    {
        public UserInfo()
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
        public string Name
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        public int Status
        {
            get;
            set;
        }
        public int Markas
        {
            get;
            set;
        }
        public DateTime Timeupdate
        {
            get;
            set;
        }
        public int Pis
        {
            get;
            set;
        }
        public int Pid
        {
            get;
            set;
        }
        public int Depth
        {
            get;
            set;
        }
        public int Logincache
        {
            get;
            set;
        }
        public int Loginfirst
        {
            get;
            set;
        }
        public long Rownumber
        {
            get;
            set;
        }

        public UserrightInfo iRight
        {
            get;
            set;
        }

        public string eStatus
        {
            get
            {
                return CFunctions.Iconforstatus(Status, Id);
            }
        }
        public string eMarkas
        {
            get
            {
                return CFunctions.Iconformarkas(Markas, Id);
            }
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
        public int ePis
        {
            get
            {
                return Pis == 0 ? 0 : (Pis - 1);
            }
        }
        #endregion

        #region methods
        public UserInfo copy()
        {
            return (UserInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for UserrightInfo.
    /// </summary>
    public class UserrightInfo
    {
        public UserrightInfo()
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
        public string R_new
        {
            get;
            set;
        }
        public string R_upt
        {
            get;
            set;
        }
        public string R_del
        {
            get;
            set;
        }
        public string R_sys
        {
            get;
            set;
        }
        #endregion

        #region methods
        public UserrightInfo copy()
        {
            return (UserrightInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for SupportonlineInfo.
    /// </summary>
    public class SupportonlineInfo : Info
    {
        public SupportonlineInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public int Type
        {
            get;
            set;
        }
        public string Note
        {
            get;
            set;
        }
        #endregion

        #region methods
        public SupportonlineInfo copy()
        {
            return (SupportonlineInfo)this.MemberwiseClone();
        }
        #endregion
    }
    
    /// <summary>
    /// Summary description for SearchInfo.
    /// </summary>
    public class SearchInfo
    {
        public SearchInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region properties
        public string Keywords
        {
            get;
            set;
        }
        public int Cid
        {
            get;
            set;
        }
        public DateTime Datefr
        {
            get;
            set;
        }
        public DateTime Dateto
        {
            get;
            set;
        }
        
        public string eDatefr
        {
            get
            {
                return Datefr.Equals(new DateTime(0)) ? string.Empty : Datefr.ToString(CConstants.FORMAT_DATE);
            }
        }
        public string eDateto
        {
            get
            {
                return Dateto.Equals(new DateTime(0)) ? string.Empty : Dateto.ToString(CConstants.FORMAT_DATE);
            }
        }

        public string Searchquery
        {
            get;
            set;
        }
        public string Setof_Category
        {
            get;
            set;
        }
        #endregion

        #region methods
        public SearchInfo copy()
        {
            return (SearchInfo)this.MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for BannerslideInfo.
    /// </summary>
    public class BannerInfo
    {
        public BannerInfo()
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
        public string Code
        {
            get;
            set;
        }
        public int Album
        {
            get;
            set;
        }
        public int Status
        {
            get;
            set;
        }
        public DateTime Timeupdate
        {
            get;
            set;
        }
        public string Username
        {
            get;
            set;
        }
        public long Rownumber
        {
            get;
            set;
        }

        public List<FileattachInfo> lFileattach
        {
            get;
            set;
        }
        
        public string eStatus
        {
            get
            {
                return CFunctions.Iconforstatus(Status, Id);
            }
        }
        public string eTimeupdate
        {
            get
            {
                return Timeupdate.Equals(new DateTime(0)) ? string.Empty : Timeupdate.ToString(CConstants.FORMAT_TIME) + " " + Timeupdate.ToString(CConstants.FORMAT_DATE);
            }
        }
        #endregion

        #region methods
        public BannerInfo copy()
        {
            return (BannerInfo)this.MemberwiseClone();
        }
        #endregion
    }


    /// <summary>
    /// Summary description for VideoInfo.
    /// </summary>
    public class VideoInfo : Info
    {
        public VideoInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private string categoryid;
        private string categoryattrid;

        #region properties
        public string Description
        {
            get;
            set;
        }
        public string Tag
        {
            get;
            set;
        }
        public int Cid
        {
            get;
            set;
        }
        public string Cname
        {
            get;
            set;
        }
        public int Allowcomment
        {
            get;
            set;
        }
        public string Author
        {
            get;
            set;
        }
        public int Viewcounter
        {
            get;
            set;
        }
        public string Sourcetype
        {
            get;
            set;
        }
        public string Url
        {
            get;
            set;
        }
        
        public List<CategoryInfo> lCategory
        {
            get;
            set;
        }
        public List<CategoryattrInfo> lCategoryattr
        {
            get;
            set;
        }
        public string Categoryid
        {
            get
            {
                if (!CFunctions.IsNullOrEmpty(categoryid)) return categoryid;

                string vlreturn = string.Empty;
                if (lCategory != null && lCategory.Count > 0)
                {
                    foreach (CategoryInfo catinfo in lCategory)
                    {
                        vlreturn += catinfo.Id + ",";
                    }
                    if (vlreturn != string.Empty)
                        vlreturn = vlreturn.Remove(vlreturn.LastIndexOf(','));
                }
                return vlreturn;
            }
            set
            {
                categoryid = value;
            }
        }
        public string Categoryattrid
        {
            get
            {
                if (!CFunctions.IsNullOrEmpty(categoryattrid)) return categoryattrid;
                
                string vlreturn = string.Empty;
                if (lCategoryattr != null && lCategoryattr.Count > 0)
                {
                    foreach (CategoryattrInfo catinfo in lCategoryattr)
                    {
                        vlreturn += catinfo.Id + ",";
                    }
                    if (vlreturn != string.Empty)
                        vlreturn = vlreturn.Remove(vlreturn.LastIndexOf(','));
                }
                return vlreturn;
            }
            set
            {
                categoryattrid = value;
            }
        }
        public string eCategory
        {
            get
            {
                string vlreturn = string.Empty;
                if (lCategory != null && lCategory.Count > 0)
                {
                    foreach (CategoryInfo catinfo in lCategory)
                    {
                        if (catinfo.Pid != 0)
                            vlreturn += catinfo.Name + ", ";
                    }
                    if (vlreturn != string.Empty)
                        vlreturn = vlreturn.Remove(vlreturn.LastIndexOf(','));
                }
                return vlreturn;
            }
        }

        public string eName
        {
            get
            {
                return CFunctions.install_urlname(Name);
            }
        }
        public string eUrl
        {
            get
            {
                return "v" + Cid.ToString() + "d" + Id.ToString() + "d=" + eName;
            }
        }
        public string eTag
        {
            get
            {
                string vlreturn = string.Empty;
                if (!CFunctions.IsNullOrEmpty(Tag))
                {
                    string[] tags = Tag.Split(',');
                    if (tags != null && tags.Length > 0)
                    {
                        foreach (string key in tags)
                        {
                            if (!CFunctions.IsNullOrEmpty(key))
                            {
                                string keya = "<a href=\"search.aspx?searchin=" + Webcmm.Id.News + "&cid=0&keywords=" + System.Uri.EscapeUriString("\"" + key.Trim() + "\"") + "\">" + key + "</a>";
                                vlreturn += keya + ", ";
                            }
                        }
                    }
                    if (vlreturn != string.Empty)
                        vlreturn = vlreturn.Remove(vlreturn.LastIndexOf(','));
                }
                return vlreturn;
            }
        }

        public int Rating
        {
            get;
            set;
        }
        #endregion

        #region methods
        public VideoInfo copy()
        {
            return (VideoInfo)this.MemberwiseClone();
        }
        #endregion
    }

}

#endregion