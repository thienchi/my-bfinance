<%@ Page Language="C#" MasterPageFile="MasterHome.Master" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="dangdongcmm.register" %>

<ASP:CONTENT ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
    <CONTENTTEMPLATE>

        <div class="title2bound">Đăng ký thành viên</div>
        <ASP:PANEL ID="pnlForm" runat="server" CssClass="tformin" DefaultButton="cmdRegister" Style="width:680px;">
            <div class="FLOUTNOTICE"><ASP:LABEL ID="lblError" runat="server"></ASP:LABEL></div>
            <div class="rowhor">
                <div class="title"><span class="require">*</span> E-mail của bạn</div>
                <div class="boxctrlc"><ASP:TEXTBOX ID="txtUsername" runat="server" MaxLength="255" Style="text-transform:lowercase"></ASP:TEXTBOX></div>
                <div class="boxctrlc note" style="margin-left:10px;">là tên đăng nhập sau này</div>
            </div>
            <div class="rowhor">
                <div class="title"><span class="require">*</span> Mật khẩu</div>
                <div class="boxctrlc"><ASP:TEXTBOX ID="txtPassword" runat="server" TextMode="Password" MaxLength="255"></ASP:TEXTBOX></div>
            </div>                        
            <div class="rowhor">
                <div class="title"><span class="require">*</span> Gõ lại mật khẩu</div>
                <div class="boxctrlc"><ASP:TEXTBOX ID="txtPasswordretype" runat="server" TextMode="Password" MaxLength="255"></ASP:TEXTBOX></div>
            </div>
            <div class="rowhor">
                <div class="title"></div>
            </div>
            <div class="rowhor">
                <div class="title">Họ tên</div>
                <div class="boxctrlc"><ASP:TEXTBOX ID="txtName" runat="server"></ASP:TEXTBOX></div>
            </tr>
            <div class="rowhor">
                <div class="title">Địa chỉ</div>
                <div class="boxctrll"><ASP:TEXTBOX ID="txtAddress" runat="server"></ASP:TEXTBOX></div>
            </tr>
            <div class="rowhor">
                <div class="title">Điện thoại</div>
                <div class="boxctrlc"><ASP:TEXTBOX ID="txtPhone" runat="server" MaxLength="50" onkeypress="javascript:return UINumber_In(event);"></ASP:TEXTBOX></div>
            </div>
            <ASP:DROPDOWNLIST ID="ddlNational" runat="server" Visible="false" Enabled="false" AutoPostBack="true" OnSelectedIndexChanged="ddlNational_SelectedIndexChanged"></ASP:DROPDOWNLIST>
            <ASP:DROPDOWNLIST ID="ddlCity" runat="server" Visible="false"></ASP:DROPDOWNLIST>
            <ASP:TEXTBOX ID="txtEmail" runat="server" MaxLength="255" Visible="false"></ASP:TEXTBOX>
            <div class="rowhor">
                <div class="title"></div>
            </div>
            <div class="rowhor">
                <div class="title">Mã bảo vệ</div>
                <div class="boxctrls"><ASP:TEXTBOX ID="txtCaptcha" runat="server" CssClass="captcha"></ASP:TEXTBOX></div>
                <div class="boxctrls" style="margin:1px 0 0 20px; background-color:#aaa; text-align:center; width:55px;"><ASP:IMAGE ID="imgCaptcha" runat="server" ImageUrl="~/commup/captcha/NDE5OA==.png" /></div>
                <div class="boxctrls" style="margin:3px 0 0 5px;"><ASP:IMAGEBUTTON ID="cmdCaptcha" runat="server" ImageUrl="~/images/reload.png" OnClick="cmdCaptcha_Click" /></div>
            </div>
            <div class="rowhor">
                <div class="title"></div>
            </div>
            <div class="rowhor">
                <div class="title"></div>
                <div class="boxctrll">
                    <ASP:BUTTON ID="cmdRegister" runat="server" CssClass="buttonwbgs" Text="Đăng Ký" OnClientClick="javascript:return doRegister();" OnClick="cmdRegister_Click" />
                </div>
            </div>
            <div id="hintBox"><div class="arrow"></div><div class="hintcontent"></div></div>
        </ASP:PANEL>
        
        <ASP:PANEL ID="pnlNotice" runat="server" Visible="false" EnableViewState="false" CssClass="tformin">
            <div class="COMMONNOTICE">
                <div class="padding10">
                    Bạn Đã Đăng Ký Thành Công.
                    <br>
                    <h3>Chào mừng bạn đến với Fivemind.vn!</h3>
                </div>
            </div>    
        </ASP:PANEL>

    <script type="text/javascript" src="../cssscript/validata.js"></script>
    <script type="text/javascript">
    function doRegister() {
	    var the, txt = '', msg = '';
        VALIDATA.lblError = '<%=lblError.ClientID %>';
        the = VALIDATA.GetObj('<%=txtUsername.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the)) || !VALIDATA.IsEmail(VALIDATA.GetVal(the)) || VALIDATA.GetVal(the).length > 255){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_email);
        }
        the = VALIDATA.GetObj('<%=txtPassword.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the))){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_password);
        }
        var theretype = VALIDATA.GetObj('<%=txtPasswordretype.ClientID %>');
        if(VALIDATA.GetVal(the) != VALIDATA.GetVal(theretype)){
            txt = VALIDATA.Gettxt(txt, theretype);
            msg = VALIDATA.Getmsg(msg, theretype, VALIDATA.Error, Definephrase.Invalid_passwordretype);
        }
        
	    if(VALIDATA.Showerror(txt, msg)) return false;
	    prompt_processing();
	    return true;
    }
    
    function Register(){
        dangdongcmm.UtiMailService.Register(Register_Success, onFailed);
    }
    function Register_Success(e) {  }
    function onFailed() {  }
    </script>

    </CONTENTTEMPLATE>
</ASP:UPDATEPANEL>
    
</ASP:CONTENT>