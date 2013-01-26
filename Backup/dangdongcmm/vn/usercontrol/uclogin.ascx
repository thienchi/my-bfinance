<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uclogin.ascx.cs" Inherits="dangdongcmm.uclogin" %>

    <script type="text/javascript">
    function EndRequestHandler(sender, args) {
        //initrestF();
    }
    </script>

    <div id="login"></div>
    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        
        <ASP:PANEL ID="pnlGetlogin" runat="server" CssClass="tformin" DefaultButton="cmdLogin">
            <div class="FLOUTNOTICE"><ASP:LABEL ID="lblError" runat="server"></ASP:LABEL></div>
            <div class="rowhor">
                <div class="title"><span class="require">*</span> E-mail của bạn</div>
                <div class="boxctrlm"><ASP:TEXTBOX ID="txtUsername" runat="server" MaxLength="255"></ASP:TEXTBOX></div>
            </div>
            <div class="rowhor">
                <div class="title"><span class="require">*</span> Mật khẩu</div>
                <div class="boxctrlm"><ASP:TEXTBOX ID="txtPassword" runat="server" TextMode="Password"></ASP:TEXTBOX></div>
            </div>
            <div class="rowhor">
                <div class="title"></div>
            </div>
            <div class="rowhor">
                <div class="title"></div>
                <div class="boxctrll"><ASP:BUTTON ID="cmdLogin" runat="server" CssClass="buttonwbgs" Text="Đăng nhập" OnClientClick="javascript:return doLogin();" OnClick="cmdLogin_Click" /></div>
            </div>
            <ASP:CHECKBOX ID="chkRememberlogin" runat="server" Visible="false" />
        </ASP:PANEL>
        
        <ASP:PANEL ID="pnlGetpassword" runat="server" Visible="false" CssClass="tformin" DefaultButton="cmdGetpassword" Style="display:none;">
            <div class="FLOUTNOTICE" style="height:18px;"><ASP:LABEL ID="gp_lblError" runat="server" Style="color:#fff;"></ASP:LABEL></div>
            <div id="formGetpassword" runat="server">
            <div class="row">
                <div class="title"></div>
                <div class="boxctrll"><ASP:TEXTBOX ID="gp_txtUsername" runat="server" onfocus="restfIn(this, 'tên tài khoản')" onblur="restfOut(this, 'tên tài khoản');"></ASP:TEXTBOX></div>
            </div>
            <div class="row">
                <div class="title"></div>
                <div class="boxctrll"><ASP:TEXTBOX ID="gp_txtEmail" runat="server" onfocus="restfIn(this, 'e-mail dùng đăng ký tài khoản')" onblur="restfOut(this, 'e-mail dùng đăng ký tài khoản');"></ASP:TEXTBOX></div>
            </div>
            <div class="row">
                <div class="title"></div>
                <div class="boxctrll" style="text-align:right; margin:4px 0 0 10px;"><ASP:BUTTON ID="cmdGetpassword" runat="server" CssClass="buttonwbgm" Text="Quên mật khẩu" OnClientClick="javascript:return doGetpassword();" OnClick="cmdGetpassword_Click" /></div>
            </div>
            <div class="row">
                <div class="title"></div>
                <div class="boxctrlc" style="float:right;text-align:right; top:8px;"><a href="javascript:showFGetlogin();"><u>Đăng nhập</u></a></div>
            </div>
            </div>
    <script type="text/javascript">

    function doGetpassword() {
	    var the, txt = '', msg = '';
        VALIDATA.lblError = '<%=gp_lblError.ClientID %>';
        the = VALIDATA.GetObj('<%=gp_txtUsername.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the)) || !VALIDATA.IsAlpha(VALIDATA.GetVal(the))){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_username);
        }
        the = VALIDATA.GetObj('<%=gp_txtEmail.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the)) || !VALIDATA.IsEmail(VALIDATA.GetVal(the))){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_email);
        }
	    
	    if(VALIDATA.Showerror(txt, msg)) return false;
	    return true;
    }
    
    function Getpassword(){
        ibet888.UtiMailService.Getpassword(Getpassword_Success, onFailed);
    }
    function Getpassword_Success(e) {  }
    function onFailed() {  }

    function showFGetlogin() {
        $('#<%=pnlGetlogin.ClientID %>').show(); $('#<%=pnlGetpassword.ClientID %>').hide();
    }
    function showFGetpassword() {
        $('#<%=pnlGetlogin.ClientID %>').hide(); $('#<%=pnlGetpassword.ClientID %>').show();
    }

    </script>

        </ASP:PANEL>
        
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    
    <script type="text/javascript">
    function initrestF() {
        restFinput('#<%=txtUsername.ClientID %>', 'tên tài khoản');
        restFinput('#<%=txtPassword.ClientID %>', 'mật khẩu');
        $('#<%=txtUsername.ClientID %>').blur();
        $('#<%=txtPassword.ClientID %>').blur();
    }

//    $(function(){
//        initrestF();
//    });
    
    function doLogin() {
        prompt_processing();
	    return true;
    }
    
    </script>
