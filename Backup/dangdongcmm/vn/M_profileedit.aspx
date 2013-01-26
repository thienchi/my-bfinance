<%@ Page Language="C#" MasterPageFile="MasterHome.Master" AutoEventWireup="true" CodeBehind="M_profileedit.aspx.cs" Inherits="dangdongcmm.M_profileedit" %>

<ASP:CONTENT ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="title2bound">Sửa thông tin cá nhân: <ASP:LABEL ID="lblFullname" runat="server"></ASP:LABEL></div>
    
    <ASP:UPDATEPANEL ID="UpdatePanel2" runat="server">
        <CONTENTTEMPLATE>
        <div class="FLOUTNOTICE"><ASP:LABEL ID="lblError" runat="server"></ASP:LABEL></div>
        <ASP:PANEL ID="pnlForm" runat="server" CssClass="tformin" Style="width:680px;">
            
            <div class="rowhor">
                <div class="title">Họ tên</div>
                <div class="boxctrlc"><ASP:TEXTBOX ID="txtFullname" runat="server" MaxLength="255"></ASP:TEXTBOX></div>
            </div>
            <div class="rowhor">
                <div class="title">Địa chỉ</div>
                <div class="boxctrll"><ASP:TEXTBOX ID="txtAddress" runat="server"></ASP:TEXTBOX></div>
            </tr>
            <div class="rowhor">
                <div class="title">Điện thoại</div>
                <div class="boxctrlc"><ASP:TEXTBOX ID="txtPhone" runat="server" MaxLength="50"></ASP:TEXTBOX></div>
            </div>
            <ASP:TEXTBOX ID="txtEmail" runat="server" MaxLength="255" Visible="false"></ASP:TEXTBOX>
            <div class="rowhor">
                <div class="title">Yahoo <img src="../images/yahoo-16x16.png" alt="" align="middle" /></div>
                <div class="boxctrlc"><ASP:TEXTBOX ID="txtYahoo" runat="server" MaxLength="255"></ASP:TEXTBOX></div>
                <div class="boxctrlc" style="width:150px; margin-left:30px;">
                    <ASP:RADIOBUTTONLIST ID="radYahoosh" runat="server" CssClass="ctrlrad" RepeatDirection="Horizontal">
                        <ASP:LISTITEM Value="1" Selected="True">Hiện</ASP:LISTITEM>
                        <ASP:LISTITEM Value="0">Ẩn</ASP:LISTITEM>
                    </ASP:RADIOBUTTONLIST>
                </div>
            </div>
            <div class="rowhor">
                <div class="title">Skype <img src="../images/skype-16x16.png" alt="" align="middle" /></div>
                <div class="boxctrlc"><ASP:TEXTBOX ID="txtSkype" runat="server" MaxLength="255"></ASP:TEXTBOX></div>
                <div class="boxctrlc" style="width:150px; margin-left:30px;">
                    <ASP:RADIOBUTTONLIST ID="radSkypesh" runat="server" CssClass="ctrlrad" RepeatDirection="Horizontal">
                        <ASP:LISTITEM Value="1" Selected="True">Hiện</ASP:LISTITEM>
                        <ASP:LISTITEM Value="0">Ẩn</ASP:LISTITEM>
                    </ASP:RADIOBUTTONLIST>
                </div>
            </div>
            <div class="rowhor">
                <div class="title"></div>
            </div>
            <div class="rowhor">
                <div class="title"><span class="require">*</span> Mật khẩu để xác thực</div>
                <div class="boxctrlc"><ASP:TEXTBOX ID="txtPassword" runat="server" TextMode="Password"></ASP:TEXTBOX></div>
            </div>
            <div class="rowhor">
                <div class="title"></div>
            </div>
            <div class="rowhor">
                <div class="title">&nbsp;</div>
                <div class="boxctrll"><ASP:BUTTON ID="cmdUpdate" runat="server" CssClass="buttonwbgs" Text="Cập nhật" OnClientClick="javascript:return doUpdate();" OnClick="cmdUpdate_Click" /></div>
            </div>
            
        </ASP:PANEL>

        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>

    <script type="text/javascript" src="../cssscript/validata.js"></script>
    <script type="text/javascript">
    function doUpdate() {
	    var the, txt = '', msg = '';
        VALIDATA.lblError = '<%=lblError.ClientID %>';
        
	    if(VALIDATA.Showerror(txt, msg)) return false;
	    return true;
    }
    
    function Registerchanged(){
        setTimeout('location.href="M_profile.aspx"', 2000);
        dangdongcmm.UtiMailService.Registerchanged(Register_Success, onFailed);
    }
    function Register_Success(e) {  }
    function onFailed() {  }
    
    </script>
    
</ASP:CONTENT>
