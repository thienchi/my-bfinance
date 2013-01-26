<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="settingsite_email.aspx.cs" Inherits="dangdongcmm.cmm.settingsite_email" %>
<%@ MasterType virtualpath="MasterDefault.master" %>
<%@ REGISTER TagPrefix="AjaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ul class="tabs">
	        <li class="first"><a href="#">Cài đặt máy chủ e-mail để gửi/ nhận e-mail thông qua website</a></li>
        </ul>
        <div class="panes">
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> SMTP Mail Server:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="mail_txtSMTPServer" runat="server"></ASP:TEXTBOX></div>
                    <div class="boxctrls">&nbsp; &nbsp; Cổng: <ASP:TEXTBOX ID="mail_txtSMTPPort" runat="server" Style="width:60px;"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"></div>
                    <div class="boxctrlc"><ASP:CHECKBOX ID="mail_chkUseSSL" runat="server" Text="Dùng SSL?" /></div>
                </div>
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> E-mail đăng nhập:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="mail_txtUsername" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Mật khẩu:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="mail_txtPassword" runat="server" TextMode="Password" Enabled="false"></ASP:TEXTBOX></div> <div class="boxctrls">&nbsp; &nbsp; <ASP:CHECKBOX ID="mail_chkChangepassword" runat="server" Text="Change?" AutoPostBack="true" OnCheckedChanged="mail_chkChangepassword_CheckedChanged" /></div>
                </div>
                <div class="rowhor">
                    <div class="title">E-mail nhận:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="mail_txtReceiver" runat="server"></ASP:TEXTBOX></div><div class="boxctrlc note">&nbsp; &nbsp; Nếu nhiều e-mail thì cách nhau bằng dấu phẩy (,)</div>
                </div>
            </div>
        </div>

        <div class="tformin tformincom">
            <br />&nbsp;
            <div class="rowhor">
                <div class="title"></div>
                <div class="boxctrll">
                    <AJAXTOOLKIT:FILTEREDTEXTBOXEXTENDER ID="FilteredTextBoxExtender1" runat="server" TargetControlID="mail_txtSMTPPort"
                        FilterType="Custom" FilterMode="ValidChars" ValidChars="1234567890" />
                    <ASP:HIDDENFIELD ID="mail_txtId" runat="server" Value="0" />
                    <fieldset>
                        <legend>
                            <ASP:BUTTON ID="mail_cmdSave" runat="server" CssClass="button" Text="Lưu thiết lập" OnClientClick="javascript:return doSavedata();" OnClick="mail_cmdSave_Click" />
                        </legend>
                    </fieldset>
                </div>
            </div>
        </div>
        
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    
    <script type="text/javascript">
    $(function() { initabs(); });
    function doSave() { return true; }
    </script>
    
</ASP:CONTENT>
