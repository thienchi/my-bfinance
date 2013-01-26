<%@ Page Language="C#" MasterPageFile="MasterHome.Master" AutoEventWireup="true" CodeBehind="M_password.aspx.cs" Inherits="dangdongcmm.M_password" %>

<ASP:CONTENT ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="title2bound">Đổi mật khẩu: <%=CurrentMember == null ? "" : CurrentMember.Username %></div>

    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        <div class="FLOUTNOTICE"><ASP:LABEL ID="lblError" runat="server"></ASP:LABEL></div>
        <ASP:PANEL ID="pnlChangepassword" runat="server" CssClass="tformin">
            <div class="rowhor">
                <div class="title">Mật khẩu cũ:</div>
                <div class="boxctrlc"><ASP:TEXTBOX ID="txtPasswordold" runat="server" TextMode="Password" MaxLength="255"></ASP:TEXTBOX></div>
            </div>
            <div class="rowhor">
                <div class="title">Mật khẩu mới:</div>
                <div class="boxctrlc"><ASP:TEXTBOX ID="txtPasswordnew" runat="server" TextMode="Password" MaxLength="255"></ASP:TEXTBOX></div>
            </div>
            <div class="rowhor">
                <div class="title">Gõ lại Mật khẩu mới:</div>
                <div class="boxctrlc"><ASP:TEXTBOX ID="txtPasswordcon" runat="server" TextMode="Password" MaxLength="255"></ASP:TEXTBOX></div>
            </div>
            <div class="rowhor">
                <div class="title"></div>
            </div>
            <div class="rowhor">
                <div class="title">&nbsp;</div>
                <div class="boxctrll"><ASP:BUTTON ID="cmdChangepassword" runat="server" CssClass="buttonwbgs" Text="Đổi mật khẩu" OnClick="cmdChangepassword_Click" /></div>
            </div>
        </ASP:PANEL>
    
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
</ASP:CONTENT>
