<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="contactus.aspx.cs" Inherits="dangdongcmm.contactus" %>

<ASP:CONTENT ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link type="text/css" rel="stylesheet" href="../cssscript/form.css" />
    <div class="boxcontact">
    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
            <div class="catname">Liên hệ</div>
            <div><!--#include file="../xhtml/vn/contact.htm" --></div>
            <div class="FLOUTNOTICE"><ASP:LABEL ID="lblError" runat="server"></ASP:LABEL></div>
            <ASP:PANEL ID="pnlForm" runat="server" CssClass="tformin" Style="width:650px;">

                <ASP:TEXTBOX ID="txtName" runat="server" Visible="false" Text="Liên hệ từ bfinance.vn"></ASP:TEXTBOX>
                <div class="rowhor">
                    <div class="title">Họ tên</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtSender_Nickname" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">E-mail</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtSender_Email" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Điện thoại</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtSender_Phone" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Nội dung</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtDescription" runat="server" TextMode="MultiLine" Rows="8" Style="width:100%"></ASP:TEXTBOX></div>
                </div>
                
                <div class="rowhor">
                    <div class="title">&nbsp;</div>
                    <div class="boxctrll"><ASP:BUTTON ID="cmdSubmit" runat="server" Text="Gửi đi" CssClass="buttonwbgs" OnClientClick="javascript:return doFeedback();" OnClick="cmdSubmit_Click" /></div>
                </div>

            <script type="text/javascript" src="../cssscript/validata.js"></script>
            <script type="text/javascript">
            function CT_Validata() {
	            var the, txt = '', msg = '';
	            VALIDATA.lblError = '<%=lblError.ClientID %>';
	            the = VALIDATA.GetObj('<%=txtSender_Email.ClientID %>');
	            if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the)) || !VALIDATA.IsEmail(VALIDATA.GetVal(the))){
		            txt = VALIDATA.Gettxt(txt, the);
		            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_email);
	            }
	            the = VALIDATA.GetObj('<%=txtDescription.ClientID %>');
	            if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the))){
		            txt = VALIDATA.Gettxt(txt, the);
		            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Require_content);
	            }
        	    
	            if(VALIDATA.Showerror(txt, msg)) return false;
	            return true;
            }
            function doFeedback() {
                if(!CT_Validata()) return false;
                return true;
            }

            </script>
            </ASP:PANEL>
            
            <script type="text/javascript">
            function Feedback() {
                dangdongcmm.UtiMailService.Feedback(Feedback_Success, onFailed);
            }
            function Feedback_Success(e) { }
            function onFailed() { }
            </script>    
            
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    </div>
</ASP:CONTENT>
