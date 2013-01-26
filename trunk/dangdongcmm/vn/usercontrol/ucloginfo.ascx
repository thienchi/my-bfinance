<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucloginfo.ascx.cs" Inherits="dangdongcmm.ucloginfo" EnableViewState="false" %>

<ASP:PANEL ID="pnlLogin" runat="server" Visible="false" CssClass="loginfo">

        <div id="menuoptions">
            <ul class="horizontal">
                <li>
                    <span class="wrap"><%=CurrentMember == null ? "" : CurrentMember.Username %></span>
                    <ul class="vertical">
                        <li><a href="M_profile.aspx">Thông tin cá nhân</a></li>
                        <li><a href="M_password.aspx">Đổi mật khẩu</a></li>
                        <li class="last"><ASP:LINKBUTTON ID="lnkSignout" runat="server" OnClick="lnkSignout_Click" Text="Thoát ra"></ASP:LINKBUTTON></li>
                    </ul>
                </li>
            </ul>
        </div>

</ASP:PANEL>
        
<ASP:PANEL ID="pnlLoginempty" runat="server" Visible="false" CssClass="loginfo">
    <a href="login.aspx">Đăng nhập</a> &nbsp; | &nbsp; <a href="register.aspx">Đăng ký</a>
</ASP:PANEL>
