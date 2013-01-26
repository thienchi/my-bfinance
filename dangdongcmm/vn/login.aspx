<%@ Page Language="C#" MasterPageFile="MasterHome.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="dangdongcmm.login" %>
<%@ REGISTER TagPrefix="UC" TagName="Login" Src="usercontrol/uclogin.ascx" %>

<ASP:CONTENT ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="title2bound">Đăng nhập Fivemind.vn</div>
    <div style="width:540px;">
        <UC:LOGIN ID="Login" runat="server" />
    </div>
    <br />&nbsp;

</ASP:CONTENT>