<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="useru.aspx.cs" Inherits="dangdongcmm.cmm.useru" %>
<%@ MasterType virtualpath="MasterDefault.master" %>
<%@ REGISTER TagPrefix="UC" TagName="Displaysetting" Src="usercontrol/ucdisplaysetting.ascx" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ul class="tabs">
	        <li class="first"><a href="#">Thông tin người dùng</a></li>
	        <li><a href="#">Thiết Lập Hiển Thị</a></li>
        </ul>
        <div class="panes">
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Họ tên:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtName" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Tên truy cập:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtUsername" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Mật khẩu:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtPassword" runat="server"></ASP:TEXTBOX></div>
                    <div class="boxctrls" style="margin-left:5px"><ASP:BUTTON ID="cmdCreatepassword" runat="server" Text="Tạo mật khẩu" CssClass="buttoncmd" OnClick="cmdCreatepassword_Click" /></div>
                </div>
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> E-mail:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtEmail" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Thuộc nhóm</div>
                    <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlGroup" runat="server"></ASP:DROPDOWNLIST></div>
                </div>
            </div>    
            <div class="tformin tformincom">
                <UC:DISPLAYSETTING ID="Displaysetting" runat="server" Showdisplayorder="false" Showicon="false" />
            </div>
        </div>
        
        <div class="noticearea" style="width:400px">
            <b><u>Ghi chú:</u></b>
            <br />"<b>Tên truy cập</b>" viết thường, không có khoảng trắng và không chứa các ký tự đặc biệt
            <br />"<b>Mật khẩu</b>" phải có ít nhất 6 ký tự
        </div>
        
        <div class="tformin tformincom">
            <br />&nbsp;
            <div class="rowhor">
                <div class="title"></div>
                <div class="boxctrll">
                    <ASP:HIDDENFIELD ID="txtId" runat="server" Value="0" />
                    <fieldset>
                        <legend>
                            <ASP:BUTTON ID="cmdSave" runat="server" CssClass="button" Text=" Lưu " OnClientClick="javascript:return doSavedata();" OnClick="cmdSave_Click" />
                            <ASP:BUTTON ID="cmdClear" runat="server" CssClass="button" Text="Nhập lại" OnClick="cmdClear_Click" />
                        </legend>
                        <div>
                            <ASP:CHECKBOX ID="chkSaveoption_golist" runat="server" Text="Lưu thông tin & tiếp tục chỉnh sửa?" />
                        </div>    
                    </fieldset>
                </div>
            </div>
        </div>
        
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    
    <script type="text/javascript">
    $(function() { initabs(); });
    
    function doSave() {
	    var the, txt = '', msg = '';
	    the = VALIDATA.GetObj('<%=txtName.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the))){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Require_name);
        }
	    the = VALIDATA.GetObj('<%=txtUsername.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the)) || !VALIDATA.IsAlpha(VALIDATA.GetVal(the))){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_username);
        }
	    the = VALIDATA.GetObj('<%=txtPassword.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the))){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_password);
        }
	    the = VALIDATA.GetObj('<%=txtEmail.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the)) || !VALIDATA.IsEmail(VALIDATA.GetVal(the))){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_email);
        }
	    
	    if(VALIDATA.Showerror(txt, msg)) return false;
	    return true;
    }

    function Createuser(){
        dangdongcmm.UtiMailService.Createuser(Createuser_Success, onFailed);
    }
    function Createuser_Success(e) {  }
    function onFailed() {  }

    </script>
        
</ASP:CONTENT>
