using System;
using System.Collections.Generic;
using System.Text;

namespace dangdongcmm.model
{
    public class Definephrase
    {
        #region variables
        //public const string Ffile = "../xhtml/Definephrase.xml";

        public const string Require = "_Require";
        public const string Require_code = "_Require_code";
        public const string Require_name = "_Require_name";
        public const string Require_namefl = "_Require_namefl";
        public const string Require_username = "_Require_username";
        public const string Require_password = "_Require_password";
        public const string Require_email = "_Require_email";
        public const string Require_phone = "_Require_phone";
        public const string Require_address = "_Require_address";
        public const string Require_national = "_Require_national";
        public const string Require_city = "_Require_city";
        public const string Require_delivery_name = "_Require_delivery_name";
        public const string Require_delivery_address = "_Require_delivery_address";
        public const string Require_delivery_national = "_Require_delivery_national";
        public const string Require_delivery_city = "_Require_delivery_city";
        
        public const string Require_title = "_Require_title";
        public const string Require_content = "_Require_content";
        public const string Require_catalogue = "_Require_catalogue";
        public const string Require_catalogueinfo = "_Require_catalogueinfo";
        public const string Require_keyword = "_Require_keyword";
        public const string Require_menu = "_Require_menu";

        public const string Invalid = "_Invalid";
        public const string Invalid_right = "_Invalid_right";
        public const string Invalid_data = "_Invalid_data";
        public const string Invalid_int = "_Invalid_int";
        public const string Invalid_datetime = "_Invalid_datetime";
        public const string Invalid_filesize = "_Invalid_filesize";
        public const string Invalid_filepreviewsize = "_Invalid_filepreviewsize";
        public const string Invalid_fileurl = "_Invalid_fileurl";
        public const string Invalid_file = "_Invalid_file";
        public const string Invalid_filetype_image = "_Invalid_filetype_image";
        public const string Notice_fileupload_duplicate = "_Notice_fileupload_duplicate";
        public const string Notice_fileupload_done = "_Notice_fileupload_done";
        public const string Notice_fileupload_error = "_Notice_fileupload_error";
        public const string Invalid_username = "_Invalid_username";
        public const string Invalid_username_notexist = "_Invalid_username_notexist";
        public const string Invalid_username_email = "_Invalid_username_email";
        public const string Invalid_password = "_Invalid_password";
        public const string Invalid_passwordretype = "_Invalid_passwordretype";
        public const string Invalid_email = "_Invalid_email";
        public const string Login_invalid = "_Login_invalid";
        public const string Login_restricted = "_Login_restricted";
        public const string Login_expired = "_Login_expired";
        
        public const string Cart_empty = "_Cart_empty";
        public const string Cart_checkout_done = "_Cart_checkout_done";
        public const string Cart_checkout_error = "_Cart_checkout_error";
        public const string Cart_listtemplate = "_Cart_listtemplate";
        
        public const string Exist_username = "_Exist_username";
        public const string Exist_email = "_Exist_email";
        public const string Exist_phone = "_Exist_phone";
        public const string Comment_dupemail = "_Comment_dupemail";
        public const string Comment_dupphone = "_Comment_dupphone";
        
        public const string Remove_confirm = "_Remove_confirm";
        public const string Remove_denied = "_Remove_denied";
        public const string Remove_completed = "_Remove_completed";
        public const string Remove_error = "_Remove_error";

        public const string Takeinfo_completed = "_Takeinfo_completed";
        public const string Takeinfo_error = "_Takeinfo_error";
        public const string Save_notice = "_Save_notice";
        public const string Save_completed = "_Save_completed";
        public const string Save_error = "_Save_error";
        public const string Savemultilang_completed = "_Savemultilang_completed";
        public const string Savemultilang_error = "_Savemultilang_error";
        public const string Copy_completed = "_Copy_completed";
        public const string Copy_error = "_Copy_error";
        public const string Move_completed = "_Move_completed";
        public const string Move_error = "_Move_error";
        public const string Saveorderd_completed = "_Saveorderd_completed";
        public const string Saveorderd_error = "_Saveorderd_error";
        public const string Removefilepreview_confirm = "_Removefilepreview_confirm";
        public const string Removefilepreview_completed = "_Removefilepreview_completed";
        public const string Removefilepreview_error = "_Removefilepreview_error";
        public const string Savemenu_completed = "_Savemenu_completed";
        public const string Savemenu_error = "_Savemenu_error";
        
        public const string Firstitem_ddl = "_Firstitem_ddl";
        public const string Firstitem_other = "_Firstitem_other";

        public const string Notice_file_notexistcontinue = "_Notice_file_notexistcontinue";
        public const string Notice_file_loaded = "_Notice_file_loaded";
        public const string Notice_gotoUrldelayl = "_Notice_gotoUrldelayl";
        public const string Notice_noselecteditem = "_Notice_noselecteditem";
        public const string Display_pis = "_Display_pis";
        public const string Display_havesub = "_Display_havesub";
        public const string Display_havesub_news = "_Display_havesub_news";
        public const string Display_havesub_product = "_Display_havesub_product";
        public const string Display_havesub_video = "_Display_havesub_video";
        public const string Display_havesub_poll = "_Display_havesub_poll";
        public const string Display_havesub_feedback = "_Display_havesub_feedback";
        public const string Display_havesub_attr = "_Display_havesub_attr";

        public const string Interface_login_title = "_Interface_login_title";
        public const string Interface_login_username = "_Interface_login_username";
        public const string Interface_login_password = "_Interface_login_password";
        public const string Interface_login_submit = "_Interface_login_submit";
        public const string Interface_login_remember = "_Interface_login_remember";
        public const string Interface_login_getpassword = "_Interface_login_getpassword";
        public const string Interface_login_invalid = "_Interface_login_invalid";
        public const string Interface_login_copyright = "_Interface_login_copyright";
        public const string Changepassword_invalid_passwordold = "_Changepassword_invalid_passwordold";
        public const string Changepassword_invalid_passwordcon = "_Changepassword_invalid_passwordcon";
        public const string Changepassword_done = "_Changepassword_done";
        public const string Changepassword_error = "_Changepassword_error";
        public const string Getpassword_verify = "_Getpassword_verify";
        public const string Getpassword_done = "_Getpassword_done";
        public const string Getpassword_error = "_Getpassword_error";

        public const string Confirm_error = "_Confirm_error";
        public const string Confirm_register_done = "_Confirm_register_done";
        public const string Problem_restricted = "_Problem_restricted";
        public const string Problem_notfound = "_Problem_notfound";

        public const string Createaccount_done = "_Createaccount_done";
        public const string Createaccount_error = "_Createaccount_error";
        public const string Feedback_done = "_Feedback_done";
        public const string Feedback_error = "_Feedback_error";
        public const string Account_Update_done = "_Account_Update_done";

        public const string Payment_done = "_Payment_done";
        public const string Payment_message_mail = "_Payment_message_mail";
        public const string Payment_message_historysub = "_Payment_message_historysub";
        public const string Payment_message_historyadd = "_Payment_message_historyadd";
        public const string Payment_recharge_done = "_Payment_recharge_done";
        public const string Payment_withdraw_done = "_Payment_withdraw_done";
        public const string Payment_error = "_Payment_error";

        public const string Invalid_captcha = "_Invalid_captcha";
        public const string Invalid_pin = "_Invalid_pin";


        public const string Getcoupon_done = "_Getcoupon_done";
        public const string Getcoupon_error = "_Getcoupon_error";
        
        #endregion
    }
}
