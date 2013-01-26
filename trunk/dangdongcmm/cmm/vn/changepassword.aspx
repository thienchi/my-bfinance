<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="changepassword.aspx.cs" Inherits="dangdongcmm.cmm.changepassword" %>
<%@ MasterType virtualpath="MasterDefault.master" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ul class="tabs">
	        <li class="first"><a href="#">Đổi Mật Khẩu</a></li>
        </ul>
        <div class="panes">
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="title">Mật khẩu cũ:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtPasswordold" runat="server" TextMode="Password"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Mật khẩu mới:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtPasswordnew" runat="server" TextMode="Password"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Gõ lại mật khẩu mới:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtPasswordcon" runat="server" TextMode="Password"></ASP:TEXTBOX></div>
                </div>
            </div>    
        </div>
        
        <div class="tformin tformincom">
            <br />&nbsp;
            <div class="rowhor">
                <div class="title"></div>
                <div class="boxctrll">
                    <ASP:HIDDENFIELD ID="txtId" runat="server" Value="0" />
                    <fieldset>
                        <legend>
                            <ASP:BUTTON ID="cmdSave" runat="server" CssClass="button" Text="Đổi mật khẩu" OnClientClick="javascript:return doSavedata();" OnClick="cmdSave_Click" />
                        </legend>
                    </fieldset>
                </div>
            </div>
        </div>
        
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    
    <script type="text/javascript">
    $(function() { initabs(); });
    
    function doSave() {
	    
	    return true;
    }

    </script>        
</ASP:CONTENT>
