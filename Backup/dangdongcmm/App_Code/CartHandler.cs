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

using dangdongcmm.utilities;
using dangdongcmm.model;
using dangdongcmm.dal;

namespace dangdongcmm
{
    public class CartHandler : System.Web.UI.Page
    {
        public CartHandler(string lang)
        {
            LANG = lang;
        }

        #region properties
        public string LANG
        {
            get;
            set;
        }
        #endregion

        public object Session_Get(string name)
        {
            if (Session[name] == null)
                return null;
            else
                return Session[name];
        }
        public void Session_Set(string name, object value)
        {
            if (Session[name] == null)
                Session.Add(name, value);
            else
                Session[name] = value;
            return;
        }
        public void Session_Remove(string name)
        {
            if (Session[name] != null)
                Session.Remove(name);
        }

        public bool AddtoCart(int ProductID, long TemporaryID)
        {
            try
            {
                if (Session_Get(Sessionparam.CART) == null)
                    Session_Set(Sessionparam.CART, new CartInfo());

                CartInfo CARTINFO = (CartInfo)Session_Get(Sessionparam.CART);
                CARTINFO.Status = (int)CConstants.State.Status.Actived;
                CARTINFO.Timeupdate = DateTime.Now;
                CARTINFO.Name = ConfigurationSettings.AppSettings["CARTPREFIX"];

                List<CartitemInfo> cartlist = CARTINFO.lCartitem;
                if (cartlist == null)
                    cartlist = new List<CartitemInfo>();

                if (TemporaryID == 0) 
                    goto AddNewItem;
                else
                {
                    int index = this.IsinCart(cartlist, TemporaryID);
                    if (index == -1) 
                        goto AddNewItem;
                    else
                    {
                        CartitemInfo cartitem = (CartitemInfo)cartlist[index];
                        cartlist[index] = cartitem;
                        goto Return;
                    }
                }

            AddNewItem:
                {
                    CartitemInfo cartitem = this.AssigntoCartItem(ProductID);
                    cartlist.Add(cartitem);
                    CARTINFO.Amount += cartitem.Amount;
                }

            Return:
                {
                    CARTINFO.lCartitem = cartlist;
                    if (Session_Get(Sessionparam.WEBUSERLOGIN) != null)
                    {
                        MemberInfo member = (MemberInfo)Session_Get(Sessionparam.WEBUSERLOGIN);
                        CARTINFO.Memberid = member.Id;
                        (new CCart(LANG)).Save(CARTINFO);
                    }
                    Session_Set(Sessionparam.CART, CARTINFO);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int IsinCart(List<CartitemInfo> cartlist, long TemporaryID)
        {
            if (cartlist == null || cartlist.Count == 0) return -1;

            int count = -1;
            foreach (CartitemInfo cartitem in cartlist)
            {
                count++;
                if (cartitem.TemporaryID == TemporaryID)
                    return count;
            }
            return -1;
        }
        
        public bool AddtoCart(int ProductID, int quantity)
        {
            try
            {
                if (Session_Get(Sessionparam.CART) == null)
                    Session_Set(Sessionparam.CART, new CartInfo());
                
                CartInfo CARTINFO = (CartInfo)Session_Get(Sessionparam.CART);
                List<CartitemInfo> cartlist = CARTINFO.lCartitem;
                if (cartlist == null)
                    cartlist = new List<CartitemInfo>();
                
                int index = this.IsinCart(cartlist, ProductID);
                if (index != -1)
                {
                    CartitemInfo cartitem = (CartitemInfo)cartlist[index];
                    cartitem.Quantity += quantity;
                    cartitem.Amount = cartitem.iProduct.Price * cartitem.Quantity;
                    cartlist[index] = cartitem;

                    CARTINFO.Amount += (cartitem.iProduct.Price * quantity);
                }
                else
                {
                    CartitemInfo cartitem = this.AssigntoCartItem(ProductID);
                    cartitem.Quantity = quantity;
                    cartitem.Amount = cartitem.iProduct.Price * cartitem.Quantity;
                    cartlist.Add(cartitem);

                    CARTINFO.Amount += cartitem.Amount;
                }

                CARTINFO.lCartitem = cartlist;
                Session_Set(Sessionparam.CART, CARTINFO);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool RemovefromCart(int index)
        {
            try
            {
                if (Session_Get(Sessionparam.CART) == null) return false;

                CartInfo CARTINFO = (CartInfo)Session_Get(Sessionparam.CART);
                List<CartitemInfo> cartlist = CARTINFO.lCartitem;
                if (cartlist == null) return false;

                CartitemInfo cartitem = (CartitemInfo)cartlist[index];
                cartlist.RemoveAt(index);

                CARTINFO.Amount -= cartitem.Amount;
                CARTINFO.lCartitem = cartlist;
                Session_Set(Sessionparam.CART, CARTINFO);

                //if login remove from database
                if (Session_Get(Sessionparam.WEBUSERLOGIN) != null)
                {
                    if (CARTINFO.Id != 0)
                    {
                        (new CCart(LANG)).Updatenum(CARTINFO.Id.ToString(), "amount", CARTINFO.Amount);
                        (new CCartitem(LANG)).Deleteone(cartitem.Id);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateShippingfee(double fee)
        {
            try
            {
                if (Session_Get(Sessionparam.CART) == null) return false;

                CartInfo CARTINFO = (CartInfo)Session_Get(Sessionparam.CART);
                CARTINFO.Shippingfee = fee;
                Session_Set(Sessionparam.CART, CARTINFO);
                if (CARTINFO.Id != 0)
                    (new CCart(LANG)).Updatenum(CARTINFO.Id.ToString(), "shippingfee", CARTINFO.Shippingfee);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateDiscountfee(string couponcode, double fee)
        {
            try
            {
                if (Session_Get(Sessionparam.CART) == null) return false;

                CartInfo CARTINFO = (CartInfo)Session_Get(Sessionparam.CART);
                CARTINFO.Couponcode = couponcode;
                CARTINFO.Discountfee = fee;
                Session_Set(Sessionparam.CART, CARTINFO);
                if (CARTINFO.Id != 0)
                {
                    CCart DALCart = new CCart(LANG);
                    DALCart.Updatenum(CARTINFO.Id.ToString(), "discountfee", CARTINFO.Discountfee);
                    DALCart.Updatestr(CARTINFO.Id.ToString(), "couponcode", CARTINFO.Couponcode);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int IsinCart(List<CartitemInfo> cartlist, int ProductID)
        {
            if (cartlist == null || cartlist.Count == 0) return -1;

            int count = -1;
            foreach (CartitemInfo cartitem in cartlist)
            {
                count++;
                if (cartitem.ProductID == ProductID)
                    return count;
            }
            return -1;
        }
        private CartitemInfo AssigntoCartItem(int ProductID)
        {
            ProductInfo info = (new CProduct(LANG)).Getinfo(ProductID);
            CartitemInfo cartitem = new CartitemInfo();
            cartitem.TemporaryID = DateTime.Now.Ticks;
            cartitem.Quantity = 1;
            cartitem.ProductID = ProductID;
            cartitem.Productname = info.Name;
            cartitem.Amount = info.Price;
            cartitem.iProduct = info;
            
            return cartitem;
        }
    }
}
